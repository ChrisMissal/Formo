using System;

namespace Formo
{
    internal class ThrowHelper
    {
        internal static Exception FailedCast(Type attemptedType, object value, string optionalMessage = null, Exception ex = null)
        {
            var message = "Unable to cast setting value '{0}' to '{1}'"
                .FormatWith(value ?? "(null)", attemptedType);

            if (optionalMessage != null)
                message += (Environment.NewLine + "> " + optionalMessage + Environment.NewLine);

            return new InvalidCastException(message, ex);
        }

        internal static Exception KeyNotFound(string key)
        {
            var message = "Unable to locate a value for '{0}' from configuration file".FormatWith(key);
            return new InvalidOperationException(message);
        }
    }
}