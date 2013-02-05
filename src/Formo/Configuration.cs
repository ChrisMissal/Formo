using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace Formo
{
    public class Configuration : DynamicObject
    {
        private static readonly List<TypeConverter> conversions = new List<TypeConverter>();

        public Configuration(params TypeConverter[] customConvters) : this(customConvters.AsEnumerable())
        {
        }

        public Configuration(IEnumerable<TypeConverter> customConverters)
        {
            conversions.AddRange(customConverters);
        }

        private static object ConvertValue(Type destinationType, object value)
        {
            var typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (typeConverter.IsValid(value))
                return typeConverter.ConvertFrom(value);

            var converter = conversions.FirstOrDefault(x => x.IsValid(value));
            if (converter != null)
                return converter.ConvertFrom(value);

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
    }
}
