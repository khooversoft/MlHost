using FluentAssertions;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MlHostCli.Test
{
    public class ModelIdTests
    {
        [Fact]
        public void GivenValidNodeId_WhenConstructed_ShouldBeCorrect()
        {
            var modelId = new ModelId("modelname", "versionid");

            modelId.ModelName.Should().Be("modelname");
            modelId.VersionId.Should().Be("versionid");

            var modelId2 = new ModelId("modelname/versionid");

            modelId2.ModelName.Should().Be("modelname");
            modelId2.VersionId.Should().Be("versionid");
        }

        [Fact]
        public void GivenNotValidNodeId_WhenConstructed_ShouldBeCorrect()
        {
            Action act = () => new ModelId("3modelName/versionid");
            act.Should().Throw<ArgumentException>();

            Action act2= () => new ModelId("3modelName", "versionid");
            act2.Should().Throw<ArgumentException>();

            Action act3 = () => new ModelId("modelname/2versionId");
            act2.Should().Throw<ArgumentException>();

            Action act4 = () => new ModelId("modelname", "2versionId");
            act2.Should().Throw<ArgumentException>();

            Action act5 = () => new ModelId("modelname/-versionId");
            act5.Should().Throw<ArgumentException>();

            Action act6 = () => new ModelId("modelname", "-versionId");
            act6.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenTwoModelId_WhenContructedSame_ShouldBeEqual()
        {
            var modelId1 = new ModelId("model/version");
            var modelId2 = new ModelId("model/version");

            (modelId1 == modelId2).Should().BeTrue();
            (modelId1 != modelId2).Should().BeFalse();

            var modelId3 = new ModelId("model/version2");
            (modelId1 == modelId3).Should().BeFalse();
            (modelId1 != modelId3).Should().BeTrue();
        }
    }
}
