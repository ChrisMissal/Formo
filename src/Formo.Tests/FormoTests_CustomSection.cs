using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Formo.Tests
{
    [TestFixture]
    public class When_forced_to_use_a_string_key_CustomSection : When_forced_to_use_a_string_key
    {
        public When_forced_to_use_a_string_key_CustomSection()
            : base("customSection")
        {

        }
    }

    [TestFixture]
    public class When_using_typed_configuration_values_CustomSection : When_using_typed_configuration_values
    {
        public When_using_typed_configuration_values_CustomSection()
            : base("customSection")
        {

        }
    }

    [TestFixture]
    public class When_key_is_in_configuration_file_CustomSection : When_key_is_in_configuration_file
    {
        public When_key_is_in_configuration_file_CustomSection()
            : base("customSection")
        {

        }
    }

    [TestFixture]
    public class When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_false_CustomSection : When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_false
    {
        public When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_false_CustomSection()
            : base("customSection")
        {

        }
    }

    [TestFixture]
    public class When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_true_CustomSection : When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_true
    {
        public When_key_isnt_in_configuration_file_and_ThrowIfNull_set_to_true_CustomSection()
            : base("customSection")
        {

        }
    }
}
