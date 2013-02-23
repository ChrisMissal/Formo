using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Formo.Tests
{
    [TestFixture]
    public class When_forced_to_use_a_string_key_AppSettings : When_forced_to_use_a_string_key
    {
        public When_forced_to_use_a_string_key_AppSettings()
            : base("appSettings")
        {

        }
    }

    [TestFixture]
    public class When_using_typed_configuration_values_AppSettings : When_using_typed_configuration_values
    {
        public When_using_typed_configuration_values_AppSettings()
            : base("appSettings")
        {

        }
    }

    [TestFixture]
    public class When_key_is_in_configuration_file_AppSettings : When_key_is_in_configuration_file
    {
        public When_key_is_in_configuration_file_AppSettings()
            : base("appSettings")
        {

        }
    }

    [TestFixture]
    public class When_key_isnt_in_configuration_file_AppSettings : When_key_isnt_in_configuration_file
    {
        public When_key_isnt_in_configuration_file_AppSettings()
            : base("appSettings")
        {

        }
    }
}
