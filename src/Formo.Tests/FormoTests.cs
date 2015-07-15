using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Formo.Tests.TestModels;
using NUnit.Framework;
using Shouldly;

namespace Formo.Tests
{
    public class When_forced_to_use_a_string_key : ConfigurationTestBase
    {
        public When_forced_to_use_a_string_key()
        {
            
        }
        public When_forced_to_use_a_string_key(string sectionName)
            : base(sectionName)
        {
        }

        [Test]
        public void Get_method_should_return_value()
        {
            var actual = configuration.Get("weird:key");

            Assert.That(actual, Is.EqualTo("some value"));
        }

        [Test]
        public void Get_method_should_work_with_type_parameters()
        {
            var actual = configuration.Get<int>("NumberOfRetries");

            Assert.That(actual, Is.TypeOf<int>());
        }
    }

    public class When_using_typed_configuration_values : ConfigurationTestBase
    {
        private dynamic germanConfiguration;
        private string _sectionName;
        public When_using_typed_configuration_values()
        {
            
        }
        public When_using_typed_configuration_values(string sectionName)
            : base(sectionName)
        {
            _sectionName = sectionName;
        }

        [SetUp]
        public void SetUp()
        {
            germanConfiguration = new Configuration(_sectionName, new CultureInfo("de"));
        }

        [Test]
        public void Method_should_convert_to_int()
        {
            var actual = configuration.NumberOfRetries<int>();

            Assert.That(actual, Is.EqualTo(12));
        }

        [Test]
        public void Method_should_convert_to_decimal()
        {
            var actual = configuration.AcceptableFailurePercentage<decimal>();

            Assert.That(actual, Is.EqualTo(1.05));
        }

        [Test]
        public void Method_should_convert_to_DateTime()
        {
            var actual = configuration.ApplicationBuildDate<DateTime>();

            Assert.That(actual, Is.EqualTo(new DateTime(1999, 11, 4, 6, 23, 0)));
        }

        [Test]
        public void Method_should_convert_to_DateTime_of_ConfiguredCulture()
        {
            var actual = germanConfiguration.GermanDate<DateTime>();

            Assert.That(actual, Is.EqualTo(new DateTime(2002, 1, 22)));
        }

        [Test]
        public void Method_should_convert_to_bool()
        {
            var actual = configuration.IsLoggingEnabled<bool>();

            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        public void Should_throw_nice_exception_when_could_not_parse()
        {
            var ex = Assert.Throws<InvalidCastException>(() => configuration.NonParsableInt<Int32>());

            Assert.That(ex.Message, Is.EqualTo(
                "Unable to cast setting value 'NOT_AN_INT' to 'System.Int32'" + Environment.NewLine +
                "> Could not obtain value 'NonParsableInt' from configuration file" + Environment.NewLine));
        }
    }

    public class When_key_is_in_configuration_file : ConfigurationTestBase
    {
        public When_key_is_in_configuration_file()
        {
            
        }
        public When_key_is_in_configuration_file(string sectionName)
            : base(sectionName)
        {
        }

        [Test]
        public void Property_should_return_expected_value()
        {
            Assert.AreEqual("a0c5837ebb094b578b436f03121bb022", configuration.ApiKey);
        }

        [Test]
        public void Method_should_return_expected_value()
        {
            Assert.AreEqual("a0c5837ebb094b578b436f03121bb022", configuration.ApiKey());
        }

        [Test]
        public void Method_should_ignore_defaults()
        {
            var actual = configuration.ApiKey("defaultvalue");
            Assert.AreNotEqual("defaultvalue", actual);
        }

        [Test]
        public void Method_should_ignore_many_defaults()
        {
            var actual = configuration.ApiKey("test", "another");
            Assert.AreNotEqual("test", actual);
            Assert.AreNotEqual("another", actual);
        }
    }

    public class When_key_isnt_in_configuration_file : ConfigurationTestBase
    {
        public When_key_isnt_in_configuration_file()
        {
            
        }
        public When_key_isnt_in_configuration_file(string sectionName)
            : base(sectionName)
        {
        }

        [Test]
        public void Property_should_be_null()
        {
            Assert.Null(configuration.Missing);
        }

        [Test]
        public void Method_should_be_null()
        {
            Assert.Null(configuration.Misssing());
        }

        [Test]
        public void Method_with_param_should_return_first()
        {
            Assert.AreEqual("blargh", configuration.Missing("blargh"));
        }

        [Test]
        public void Method_with_many_params_should_return_first_non_null()
        {
            string first = null;
            var second = default(string);
            var third = "i exist";
            Assert.AreEqual(third, configuration.Missing(first, second, third));
        }

        [Test]
        public void Method_looking_for_bool_should_behave_as_ConfigurationManager()
        {
            var key = "IsSettingMissing";
            var expected = ConfigurationManager.AppSettings[key];
            var actual = configuration.IsSettingMissing<bool>();

            Assert.AreSame(expected, actual);
        }
    }

    [TestFixture]
    public class When_getting_a_collection_from_missing_custom_section : ConfigurationTestBase
    {
        public When_getting_a_collection_from_missing_custom_section()
            : base("bindAllSection")
        {
        }

        [TestCase(101, "Chris")]
        [TestCase(102, "Marisol")]
        [TestCase(103, "Allison")]
        [TestCase(104, "Ryan")]
        [TestCase(105, "Ben")]
        [TestCase(106, "Laurie")]
        [TestCase(107, "Paige")]
        [TestCase(108, "Nitya")]
        public void BindPairs_should_return_a_collection_of_Users_with_properties_set(int id, string name)
        {
            var config = (Configuration) configuration;
            var collection = config.BindPairs<User, int, string>(x => x.ID, x => x.Name).ToArray();

            collection.Count().ShouldBe(8);
            collection.First(x => x.ID == id).Name.ShouldBe(name);
        }
    }

    [TestFixture]
    public class When_key_isnt_in_conguration : ConfigurationTestBase
    { 
        [Test]
        public void Method_with_string_argument_should_use_argument()
        {
            bool unspecified = configuration.NonExisting<bool>("true");

            Assert.That(unspecified, Is.True);
        }
        
        [Test]
        public void Method_with_same_type_bool_argument_should_use_argument()
        {
            bool unspecified = configuration.NonExisting<bool>(true);

            Assert.That(unspecified, Is.True);
        }
        
        [Test]
        public void Method_with_same_type_date_argument_should_use_default_argument()
        {
            DateTime unspecified = configuration.NonExisting<DateTime>(new DateTime(2014, 2, 10));

            Assert.That(unspecified, Is.EqualTo(new DateTime(2014, 2, 10)));
        }

        [Test]
        public void Method_with_inheritance_default_argument_should_use_it()
        {
            var instance = new TestImplementation();

            ITest unspecified = configuration.NonExisting<ITest>(instance);

            Assert.That(unspecified, Is.EqualTo(instance));
        }

        public interface ITest
        {
            void Test();
        }

        public class TestImplementation : ITest
        {
            public void Test()
            {
                Console.WriteLine("Testing");
            }
        }
    }

    public class ConfigurationTestBase
    {
        public ConfigurationTestBase(string sectionName)
        {
            configuration = new Configuration(sectionName);
        }

        public ConfigurationTestBase()
        {
            configuration = new Configuration();
        }

        protected dynamic configuration;
    }
}
