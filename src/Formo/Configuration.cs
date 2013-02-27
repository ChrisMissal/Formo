using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace Formo
{
    public class Configuration : DynamicObject
    {
        private const string AppSettingsSectionName = "appSettings";
        private readonly string _prefix;
        private readonly NameValueCollection _section;
        private readonly CultureInfo _cultureInfo;
        private readonly List<TypeConverter> _converters = new List<TypeConverter>();
        private readonly string _sectionName;

        public Configuration(CultureInfo cultureInfo) : this(null, cultureInfo, null)
        {
        }

        public Configuration(string sectionName = null, CultureInfo cultureInfo = null) : this(sectionName, cultureInfo, null)
        {
        }

        public Configuration(string sectionName, CultureInfo cultureInfo, params TypeConverter[] customConvters) : this(sectionName, cultureInfo, customConvters.AsEnumerable())
        {
        }

        public Configuration(string sectionName, CultureInfo cultureInfo, IEnumerable<TypeConverter> customConverters = null)
            : this((NameValueCollection)ConfigurationManager.GetSection(sectionName ?? AppSettingsSectionName), cultureInfo ?? CultureInfo.CurrentCulture, customConverters
            )
        {
            _sectionName = sectionName;
        }

        private Configuration(NameValueCollection section, CultureInfo cultureInfo, IEnumerable<TypeConverter> customConverters, string prefix = "")
        {
            _section = section;
            _cultureInfo = cultureInfo;
            if (customConverters != null)
            {
                _converters.AddRange(customConverters);
            }
            _prefix = prefix;
        }

        internal object ConvertValue(Type destinationType, object value)
        {
            if (value == null)
                return null;

            var typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (typeConverter.CanConvertFrom(value.GetType()))
                return typeConverter.ConvertFrom(null, _cultureInfo, value);

            var converter = _converters.FirstOrDefault(x => x.CanConvertFrom(value.GetType()));
            if (converter != null)
                return converter.ConvertFrom(null, _cultureInfo, value);

            var optionalMessage = "This is most likely because a TypeConverter hasn't been " +
                                  "defined for the type '{0}'.".FormatWith(destinationType);

            throw ThrowHelper.FailedCast(destinationType, value, optionalMessage);
        }

        public object Get(string key)
        {
            return GetValue(key);
        }

        public T Get<T>(string key)
        {
            return (T) ConvertValue(typeof (T), Get(key));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = GetPrefix(_prefix, binder.Name);
            result = GetValue(name);

            var isNamespaced = IsNamespaced(name);
            if (!isNamespaced && !string.IsNullOrWhiteSpace(_prefix))
                throw ThrowHelper.FailedSettingLookup(name, _sectionName);

            if (result == null && isNamespaced)
                result = new Configuration(_section, _cultureInfo, _converters, name);

            return true;
        }

        private static string GetPrefix(string prefix, string name)
        {
            return string.IsNullOrWhiteSpace(prefix) ? name : string.Join(".", new[] { prefix, name });
        }

        private bool IsNamespaced(string key)
        {
            return _section.AllKeys.Any(x => x.StartsWith(key));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var generic = GetGenericType(binder);
            var name = GetPrefix(_prefix, binder.Name);

            var value = GetValue(name).OrFallbackTo(args);
            result = generic != null ? ConvertValue(generic, value) : value;

            var isNamespaced = IsNamespaced(name);
            if (result == null && !isNamespaced && !string.IsNullOrWhiteSpace(_prefix))
                throw ThrowHelper.FailedSettingLookup(name, _sectionName);

            if (result == null && isNamespaced)
                result = new Configuration(_section, _cultureInfo, _converters, name);

            return true;
        }

        private static Type GetGenericType(InvokeMemberBinder binder)
        {
            var csharpBinder = binder.GetType().GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
            var typeArgs = (csharpBinder.GetProperty("TypeArguments").GetValue(binder, null) as IList<Type>);

            return typeArgs.FirstOrDefault();
        }

        protected virtual string GetValue(string name)
        {
            return _section[name];
        }

        public T Bind<T>() where T : new()
        {
            var instance = Activator.CreateInstance<T>();
            var binder = new SettingsBinder();

            return binder.WithSettings(instance, this);
        }
    }
}
