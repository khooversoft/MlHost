using FluentAssertions;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MlHostApi.Test.Types
{
    public class ModelIdTests
    {
        [Fact]
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

        [Fact]
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
    }
}
