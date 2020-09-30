using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;

namespace Toolbox.Test.Tools
{
    [TestClass]
    public class PropertyResolverTests
    {
        [TestMethod]
        public void GivenProperty_WhenResolved_ShouldMatch()
        {
            var properties = new Dictionary<string, string>
            {
                ["modelName"] = "ML-Model-Sentiment",
            };

            IPropertyResolver propertyResolver = new PropertyResolver(properties);

            string subject = "This is {modelName} name";
            string expectedResult = "This is ML-Model-Sentiment name";

            string result = propertyResolver.Resolve(subject);
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void GivenProperty_WhenResolvedFailed_ShouldMatch()
        {
            var properties = new Dictionary<string, string>
            {
                ["modelName"] = "ML-Model-Sentiment",
            };

            IPropertyResolver propertyResolver = new PropertyResolver(properties);

            string subject = "This is {modelName-fake} name";
            string expectedResult = "This is {modelName-fake} name";

            string result = propertyResolver.Resolve(subject);
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void GivenMultipleProperty_WhenResolvedFailed_ShouldMatch()
        {
            var properties = new Dictionary<string, string>
            {
                ["modelName"] = "Emotion",
                ["environment"] = "acpt",
            };

            IPropertyResolver propertyResolver = new PropertyResolver(properties);

            string subject = "https://{environment}-va-ml-host-{modelName}.ase-mobile.dev.premera.net";
            string expectedResult = "https://acpt-va-ml-host-Emotion.ase-mobile.dev.premera.net";

            string result = propertyResolver.Resolve(subject);
            result.Should().Be(expectedResult);
        }
    }
}
