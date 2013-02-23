using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace Formo
{
    public class Configuration : DynamicObject
    {
        private readonly CultureInfo _cultureInfo;
        private static readonly List<TypeConverter> conversions = new List<TypeConverter>();

        public Configuration(CultureInfo cultureInfo, params TypeConverter[] customConvters) : this(cultureInfo, customConvters.AsEnumerable())
        {
        }

        public Configuration(params TypeConverter[] customConvters) : this(CultureInfo.CurrentCulture, customConvters.AsEnumerable())
        {
        }

        public Configuration(CultureInfo cultureInfo, IEnumerable<TypeConverter> customConverters)
        {
            _cultureInfo = cultureInfo;
            conversions.AddRange(customConverters);
        }

        internal object ConvertValue(Type destinationType, object value)
        {
            if (value == null)
                return null;

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

        private static string GetValue(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public T Bind<T>()
        {
            var instance = Activator.CreateInstance<T>();
            var binder = new SettingsBinder();

            return binder.WithSettings(instance, this);
        }
    }
}
