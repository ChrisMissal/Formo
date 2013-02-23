using Microsoft.WindowsAzure;

namespace Formo.Cloud
{
    public class CloudConfiguration : Configuration
    {
        protected override string GetValue(string name)
        {
            return CloudConfigurationManager.GetSetting(name);
        }
    }
}
