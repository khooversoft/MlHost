using FluentAssertions;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MlHostApi.Test.Tools
{
    public class VerifyBlobVectorTests
    {
        [Theory]
        [InlineData("a-1")]
        [InlineData("a339-vbe")]
        [InlineData("a339-vbe1")]
        [InlineData("ab")]
        [InlineData("mymodel-temp")]
        public void GivenValidBlobVector_WhenTested_ShouldPass(string value)
        {
            value.VerifyBlobVector("message");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("a")]
        [InlineData("-a")]
        [InlineData("-1")]
        [InlineData("1-")]
        [InlineData("1--")]
        [InlineData("b34-")]
        [InlineData("b-34-")]
        [InlineData("myModel-temp")]
        [InlineData("3339-Vbe1")]
        public void GiveninvalidBlobVector_WhenTested_ShouldThrowException(string value)
        {
            Action act = () => value.VerifyBlobVector("message");

            act.Should().Throw<ArgumentException>();
        }
    }
}
