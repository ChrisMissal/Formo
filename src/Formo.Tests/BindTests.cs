using System;
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
    }
}
