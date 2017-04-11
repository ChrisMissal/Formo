using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Formo
{
    internal class SettingsBinder
    {
        internal T WithSettings<T>(T instance, Configuration configuration)
        {
            var type = typeof (T);
            var properties = from prop in type.GetProperties()
                             select prop;

            foreach (var propertyInfo in properties)
            {
                object value;
                if (TryGetValue(propertyInfo, configuration, out value))
                    propertyInfo.SetValue(instance, value, null);
            }
            return instance;
        }

        private static bool TryGetValue(PropertyInfo propertyInfo, Configuration configuration, out object result)
        {
            result = null;

            var type = propertyInfo.PropertyType;
            var reflectedType = propertyInfo.ReflectedType;
            var keys = new[]
                {
                    reflectedType.Name + propertyInfo.Name,
                    propertyInfo.Name,
                };

            var vals = from key in keys
                       let attempt = GetValueOrNull(type, key, configuration) 
                       where attempt != null
                       select attempt;

            foreach (var value in vals)
            {
                result = value;
                return true;
            }
            return false;
        }

        private static object GetValueOrNull(Type type, string key, Configuration configuration)
        {
            try
            {
                return type == typeof(ConnectionStringSettings) ? configuration.ConnectionString.Get(key)
                                                                : configuration.ConvertValue(type, configuration.Get(key));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

    }
}
