using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostApi.Tools;
using System;

namespace MlHostApi.Test.Tools
{
    [TestClass]
    public class VerifyVectorTests
    {
        [DataTestMethod]
        [DataRow("a-1")]
        [DataRow("a339-vbe")]
        [DataRow("a339-vbe1")]
        [DataRow("ab")]
        [DataRow("mymodel-temp")]
        public void GivenValidVector_WhenTested_ShouldPass(string value)
        {
            value.VerifyStoreVector("message");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("1")]
        [DataRow("a")]
        [DataRow("-a")]
        [DataRow("-1")]
        [DataRow("1-")]
        [DataRow("1--")]
        [DataRow("b34-")]
        [DataRow("b-34-")]
        [DataRow("myModel-temp")]
        [DataRow("3339-Vbe1")]
        public void GiveninvalidVector_WhenTested_ShouldThrowException(string value)
        {
            Action act = () => value.VerifyStoreVector("message");

            act.Should().Throw<ArgumentException>();
        }
    }
}
