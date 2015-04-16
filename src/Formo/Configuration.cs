using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Formo
{
    public class Configuration : DynamicObject
    {
        private const string AppSettingsSectionName = "appSettings";
        private readonly NameValueCollection _section;
        private readonly CultureInfo _cultureInfo;
        private readonly List<TypeConverter> conversions = new List<TypeConverter>();
        private readonly ConnectionStringsConfiguration _connectionStringsConfiguration;

        protected readonly string _sectionName;

        public Configuration(CultureInfo cultureInfo) : this(null, cultureInfo, null)
        {
        }

        public Configuration(string sectionName = null, CultureInfo cultureInfo = null) : this(sectionName, cultureInfo, null)
        {
        }

        public Configuration(string sectionName, params TypeConverter[] customConveters) : this(sectionName, null, customConveters)
        {
        }

        public Configuration(string sectionName, CultureInfo cultureInfo, params TypeConverter[] customConvters) : this(sectionName, cultureInfo, customConvters.AsEnumerable())
        {
        }

        public Configuration(string sectionName, CultureInfo cultureInfo, IEnumerable<TypeConverter> customConverters = null)
        {
            _sectionName = sectionName ?? AppSettingsSectionName;
            _section = (NameValueCollection)ConfigurationManager.GetSection(_sectionName);
            _connectionStringsConfiguration = new ConnectionStringsConfiguration(ConfigurationManager.ConnectionStrings);
            _cultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            if (customConverters != null)
            {
                conversions.AddRange(customConverters);
            }
        }

        internal object ConvertValue(Type destinationType, object value)
        {
            if (value == null)
                return null;

            if(destinationType.IsInstanceOfType(value))
                return value;

            var typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (typeConverter.CanConvertFrom(value.GetType()))
                return typeConverter.ConvertFrom(null, _cultureInfo, value);

            var converter = conversions.FirstOrDefault(x => x.CanConvertFrom(value.GetType()));
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
            result = GetValue(binder.Name);

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var generic = GetGenericType(binder);

            var value = GetValue(binder.Name).OrFallbackTo(args);

            result = generic != null ? ConvertValue(generic, value) : value;

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

            return Bind(instance);
        }

        public T Bind<T>(T instance)
        {
            var binder = new SettingsBinder();

            return binder.WithSettings(instance, this);
        }

        public IEnumerable<T> BindPairs<T, TKey, TValue>(Expression<Func<T, TKey>> keyExpression, Expression<Func<T, TValue>> valueExpression) where T : new()
        {
            var keyConverter = TypeDescriptor.GetConverter(keyExpression.ReturnType);
            var valConverter = TypeDescriptor.GetConverter(valueExpression.ReturnType);

            foreach (var key in _section.AllKeys)
            {
                var k = keyConverter.ConvertFrom(key);
                var v = valConverter.ConvertFrom(_section[key]);

                var instance = Activator.CreateInstance<T>();

                ((PropertyInfo)((MemberExpression)valueExpression.Body).Member)
                    .SetValue(instance, v, null);
                ((PropertyInfo)((MemberExpression)keyExpression.Body).Member)
                    .SetValue(instance, k, null);

                yield return instance;
            }
        }

        public dynamic ConnectionString
        {
            get
            {
                return _connectionStringsConfiguration;
            }
        }
    }
}
