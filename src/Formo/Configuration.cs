using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace Formo
{
    public class Configuration : DynamicObject
    {
        private static readonly IDictionary<Type, Func<object, object>> typeConversions;

        static Configuration()
        {
            typeConversions = new Dictionary<Type, Func<object, object>>
                {
                    { typeof(bool), ToBool },
                    { typeof(int), ToInt },
                    { typeof(decimal), ToDecimal },
                    { typeof(DateTime), ToDateTime },
                };
        }

        private static object ToBool(object value)
        {
            return (bool) Convert.ChangeType(value, TypeCode.Boolean);
        }

        private static object ToDateTime(object value)
        {
            return (DateTime) Convert.ChangeType(value, TypeCode.DateTime);
        }

        private static object ToDecimal(object value)
        {
            return (decimal) Convert.ChangeType(value, TypeCode.Decimal);
        }

        private static object ToInt(object value)
        {
            return (int) Convert.ChangeType(value, TypeCode.Int32);
        }

        private static object ConvertValue(Type destinationType, object value)
        {
            Func<object, object> func;
            if (typeConversions.TryGetValue(destinationType, out func))
                return func(value);

            var optionalMessage = "This is most likely because a SettingsConverter hasn't been " +
                                  "defined for the type '{0}'.".FormatWith(destinationType);

            throw ThrowHelper.FailedCast(destinationType, value, optionalMessage);
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
