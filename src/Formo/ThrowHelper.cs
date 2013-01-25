using System;

namespace Formo
{
    internal class ThrowHelper
    {
        internal static Exception FailedCast(Type attemptedType, object value, string optionalMessage = null)
        {
            var message = "Unable to cast setting value '{0}' to '{1}'"
                .FormatWith(value ?? "(null)", attemptedType);

            if (optionalMessage != null)
                message += (Environment.NewLine + "> " + optionalMessage + Environment.NewLine);

            return new InvalidCastException(message);
        }
    }
}