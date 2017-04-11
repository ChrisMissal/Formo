using System;
using System.Configuration;
using NUnit.Framework;
using Shouldly;

namespace Formo.Tests
{
    public class WebsiteSettings
    {
        public string Herp { get; set; }
        public string Derp { get; set; }
        public int SomeInteger { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ConnectionStringSettings RemoteConnection { get; set; }
        public ConnectionStringSettings LocalConnection { get; set; }
    }

    [TestFixture]
    public class BindTests_Default : BindTests
    {
    }

    [TestFixture]
    public class BindTests_AppSettings : BindTests
    {
        public BindTests_AppSettings() : base("appSetings")
        {
        }
    }

    [TestFixture]
    public class BindTests_CustomSection : BindTests
    {
        public BindTests_CustomSection() : base("customSection")
        {
        }
    }

    public class BindTests
    {
        private readonly string _sectionName;
        private dynamic configuration;

        public BindTests()
        {
        }

        public BindTests(string sectionName)
        {
            _sectionName = sectionName;
        }

        [SetUp]
        public void SetUp()
        {
            if (_sectionName == null)
            {
                configuration = new Configuration();
            }
            else
            {
                configuration = new Configuration(_sectionName);
            }
        }

        [Test]
        public void Bind_should_assign_standalone_property_from_settings()
        {
            WebsiteSettings settings = new Configuration().Bind<WebsiteSettings>();

            settings.Herp.ShouldBe("herp", Case.Sensitive);
            settings.Derp.ShouldBe("derp", Case.Sensitive);
            settings.SomeInteger.ShouldBe(123);
            settings.ExpirationDate.ShouldBe(new DateTime(2011, 4, 16));
        }

        [Test]
        public void Bind_should_assign_values_to_connection_strings()
        {
            WebsiteSettings settings = new Configuration().Bind<WebsiteSettings>();
            settings.RemoteConnection.ShouldNotBe(null);
            settings.LocalConnection.ShouldNotBe(null);
        }
        
        [Test]
        public void Bind_should_assign_correct_values_to_connection_strings()
        {
            WebsiteSettings settings = new Configuration().Bind<WebsiteSettings>();
            settings.RemoteConnection.ConnectionString.ShouldBe(@"Data Source=.\SQLEXPRESS;Initial Catalog=NorthWind;Integrated Security=True");
            settings.LocalConnection.ConnectionString.ShouldBe(@"localhost");
        }

        [Test]
        public void Bind_should_assign_correct_values_to_provider_names()
        {
            WebsiteSettings settings = new Configuration().Bind<WebsiteSettings>();
            settings.RemoteConnection.ProviderName.ShouldBe(@"System.Data.SqlClient");
            settings.LocalConnection.ProviderName.ShouldBeEmpty();
        }
    }
}
