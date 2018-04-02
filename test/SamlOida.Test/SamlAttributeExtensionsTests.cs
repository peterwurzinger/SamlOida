using SamlOida.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace SamlOida.Test
{
    public class SamlAttributeExtensionsTests
    {

        [Fact]
        public void GetShouldReturnSingleMatchOnNameProperty()
        {
            var attributes = new HashSet<SamlAttribute>()
            {
                new SamlAttribute("Name", "Test"),
                new SamlAttribute("FirstName", "Herbert")
            };

            attributes.Get("FirstName");
        }

        [Fact]
        public void GetShouldReturnSingleMatchOnFirstNamePropertyIfNameNotFound()
        {
            var attributes = new HashSet<SamlAttribute>()
            {
                new SamlAttribute("Name", "Test"),
                new SamlAttribute
                {
                    FriendlyName = "FirstName",
                    Values = { "Herbert"}
                }
            };

            attributes.Get("FirstName");
        }

        [Fact]
        public void GetShouldThrowExceptionOnDuplicate()
        {
            var attributes = new HashSet<SamlAttribute>()
            {
                new SamlAttribute("Name", "Test"),
                new SamlAttribute("Name", "Herbert")
            };

            Assert.Throws<InvalidOperationException>(() => attributes.Get("Name"));
        }

        [Fact]
        public void GetShouldReturnNullIfNeitherNameNorFriendlyNameFound()
        {
            var attributes = new HashSet<SamlAttribute>()
            {
                new SamlAttribute("Name", "Test"),
                new SamlAttribute("FirstName", "Herbert")
            };

            Assert.Null(attributes.Get("NotExistentProperty"));
        }
    }
}
