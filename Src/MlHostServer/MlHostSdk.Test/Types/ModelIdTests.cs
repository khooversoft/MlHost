using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostSdk.Types;
using System;

namespace MlHostApi.Test.Types
{
    [TestClass]
    public class ModelIdTests
    {
        [TestMethod]
        public void GivenValidModelId_WhenConstructedWithMinimum_ShouldPass()
        {
            var modelId = new ModelId("modelName", "versionId");

            modelId.ModelName.Should().Be("modelname");
            modelId.VersionId.Should().Be("versionid");

            var modelId2 = new ModelId("modelName/versionId");

            modelId2.ModelName.Should().Be("modelname");
            modelId2.VersionId.Should().Be("versionid");

            (modelId == modelId2).Should().BeTrue();
            (modelId != modelId2).Should().BeFalse();
            (modelId != null).Should().BeTrue();
            (modelId == null).Should().BeFalse();
            (null != modelId).Should().BeTrue();
            (null == modelId).Should().BeFalse();
        }

        [TestMethod]
        public void GivenValidModelId_WhenConstructedWithRoot_ShouldPass()
        {
            var modelId = new ModelId("root", "modelName", "versionId");

            modelId.Root.Should().Be("root");
            modelId.ModelName.Should().Be("modelname");
            modelId.VersionId.Should().Be("versionid");

            var modelId2 = new ModelId("root/modelName/versionId");

            modelId2.Root.Should().Be("root");
            modelId2.ModelName.Should().Be("modelname");
            modelId2.VersionId.Should().Be("versionid");

            (modelId == modelId2).Should().BeTrue();
            (modelId != modelId2).Should().BeFalse();
            (modelId != null).Should().BeTrue();
            (modelId == null).Should().BeFalse();
        }


        [TestMethod]
        public void GivenValidNodeId_WhenConstructed_ShouldBeCorrect()
        {
            var modelId = new ModelId("modelname", "versionid");

            modelId.ModelName.Should().Be("modelname");
            modelId.VersionId.Should().Be("versionid");

            var modelId2 = new ModelId("modelname/versionid");

            modelId2.ModelName.Should().Be("modelname");
            modelId2.VersionId.Should().Be("versionid");
        }

        [TestMethod]
        public void GivenNotValidNodeId_WhenConstructed_ShouldBeCorrect()
        {
            Action act = () => new ModelId("3modelName/versionid");
            act.Should().Throw<ArgumentException>();

            Action act2 = () => new ModelId("3modelName", "versionid");
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

        [TestMethod]
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
