using System.Configuration;
using System.Dynamic;

namespace Formo
{
    internal class ConnectionStringsConfiguration : DynamicObject
    {
        private readonly ConnectionStringSettingsCollection _connectionStrings;

        internal ConnectionStringsConfiguration(ConnectionStringSettingsCollection connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _connectionStrings[binder.Name] ?? new ConnectionStringSettings();
            return true;
        }
    }
}