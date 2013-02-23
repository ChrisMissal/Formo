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
    public class BindTests
    {
        private dynamic configuration;

        [SetUp]
        public void SetUp()
        {
            configuration = new Configuration();
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
