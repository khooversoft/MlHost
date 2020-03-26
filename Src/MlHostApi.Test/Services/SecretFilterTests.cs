using FluentAssertions;
using MlHostApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MlHostApi.Test.Services
{
    public class SecretFilterTests
    {
        [Fact]
        public void GivenNoSecrets_WhenFiltered_ShouldHaveNoChanges()
        {
            ISecretFilter filter = new SecretFilter(Enumerable.Empty<string>());

            const string subject = "There is no secrets";
            string? result = filter.FilterSecrets(subject);

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(subject);
        }

        [Fact]
        public void GivenOneSecrets_WhenFiltered_SecretShouldBeFiltered()
        {
            ISecretFilter filter = new SecretFilter(new[] { "skey" });

            const string subject = "There is skey secrets";
            const string subjectResult = "There is *** secrets";
            string? result = filter.FilterSecrets(subject);

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(subjectResult);
        }

        [Fact]
        public void GivenOneSecret_WhenNotPresentInData_ShouldNotFilter()
        {
            ISecretFilter filter = new SecretFilter(new[] { "skey" });

            const string subject = "There is secret secrets";
            string? result = filter.FilterSecrets(subject);

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(subject);
        }

        [Theory]
        [InlineData("There is 38383dkdsdf== secrets", "There is *** secrets")]
        [InlineData("There 38383dkdsdf== is key1034 secrets", "There *** is *** secrets")]
        public void GivenMultipleSecrets_WhenFiltered_SecretShouldBeFiltered(string subject, string expectedResult)
        {
            ISecretFilter filter = new SecretFilter(new[] { "38383dkdsdf==", "key1034" });

            string? result = filter.FilterSecrets(subject);

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(expectedResult);
        }
    }
}
