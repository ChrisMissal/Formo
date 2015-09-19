using System.Reflection;

namespace Formo.Cloud
{
    public class CloudConfiguration : Configuration
    {
        private static readonly MethodInfo GetSettingMethod;

        static CloudConfiguration()
        {
            var assembly = Assembly.Load("Microsoft.WindowsAzure.Configuration");
            var type = assembly.GetType("Microsoft.Azure.CloudConfigurationManager")
                ?? assembly.GetType("Microsoft.WindowsAzure.CloudConfigurationManager");
            GetSettingMethod = type.GetMethod("GetSetting", BindingFlags.Static | BindingFlags.Public);
        }

        protected override string GetValue(string name)
        {
            return (string) GetSettingMethod.Invoke(null, new object[] {name});
        }
    }
}}