using System;
using System.Collections.Generic;

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

        internal static Exception FailedSettingLookup(string key, string section)
        {
            var sectionMessage = string.IsNullOrWhiteSpace(section) ? "" : " in section '{1}'".FormatWith(section);
            var message = "Unable to find setting by the key '{0}'{1}".FormatWith(key ?? "(null)", sectionMessage);

            return new KeyNotFoundException(message);
        }
    }
}