using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostCli.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolbox.Tools;

namespace MlHostCli.Test.Project
{
    [TestClass]
    public class VsProjectTests
    {
        [TestMethod]
        public void GivenProjectFileWithNone_WhenRead_ShouldHaveNoEmbeddedFiles()
        {
            string file = FileTools.WriteResourceToTempFile("TestProject-none.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-none.csproj");

            var vsProject = new VsProject(file).Read();

            vsProject.EmbeddedResources.Count.Should().Be(0);

            File.Delete(file);
        }

        [TestMethod]
        public void GivenProjectFileWithOne_WhenRead_ShouldHaveOneEmbeddedFiles()
        {
            string file = FileTools.WriteResourceToTempFile("TestProject-one.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-one.csproj");

            var vsProject = new VsProject(file).Read();

            vsProject.EmbeddedResources.Count.Should().Be(1);
            vsProject.EmbeddedResources.First().Should().Be(@"MlPackage\RunModel.mlPackage");

            File.Delete(file);
        }

        [TestMethod]
        public void GivenProjectFileWithTwo_WhenRead_ShouldHaveTwoEmbeddedFiles()
        {
            string file = FileTools.WriteResourceToTempFile("TestProject-two.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-two.csproj");

            var vsProject = new VsProject(file).Read();

            vsProject.EmbeddedResources.Count.Should().Be(2);

            vsProject.EmbeddedResources.Contains(@"MlPackage\RunModel.mlPackage").Should().BeTrue();
            vsProject.EmbeddedResources.Contains(@"MlPackage\RunModel2.mlPackage").Should().BeTrue();

            File.Delete(file);
        }

        [TestMethod]
        public void GivenProjectFileWithNone_WhenUpdate_ShouldHaveNewEmbeddedFiles()
        {
            const string packagePath = @"MlPackage\RunModel.mlPackage";
            string file = FileTools.WriteResourceToTempFile("TestProject-none.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-none.csproj");

            var vsProject = new VsProject(file).Read();
            vsProject.EmbeddedResources.Count.Should().Be(0);

            vsProject.EmbeddedResources.Add(packagePath);

            vsProject.Write().Should().BeTrue();

            var vsProjectUpdated = new VsProject(file).Read();
            vsProjectUpdated.EmbeddedResources.Count.Should().Be(1);
            vsProjectUpdated.EmbeddedResources.First().Should().Be(packagePath);

            File.Delete(file);
        }

        [TestMethod]
        public void GivenProjectFileWithNone_WhenUpdateWithFluent_ShouldHaveNewEmbeddedFiles()
        {
            const string packagePath = @"MlPackage\RunModel.mlPackage";
            string file = FileTools.WriteResourceToTempFile("TestProject-none.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-none.csproj");

            var vsProject = new VsProject(file)
                .Read()
                .Add(packagePath)
                .Write()
                .Should().BeTrue();

            var vsProjectUpdated = new VsProject(file).Read();
            vsProjectUpdated.EmbeddedResources.Count.Should().Be(1);
            vsProjectUpdated.EmbeddedResources.First().Should().Be(packagePath);

            File.Delete(file);
        }

        [TestMethod]
        public void GivenProjectFileWithOne_WhenRemovedAndAdded_ShouldHaveNewEmbeddedFiles()
        {
            const string packagePath = @"MlPackage\RunModel.mlPackage";
            const string packagePath2 = @"MlPackage\RunModel2.mlPackage";
            const string packagePath_add = @"MlPackage\RunModel-add.mlPackage";

            string file = FileTools.WriteResourceToTempFile("TestProject-two.csproj", nameof(VsProjectTests), typeof(VsProjectTests), "MlHostCli.Test.TestData.TestProject-two.csproj");

            var vsProject = new VsProject(file).Read();
            vsProject.EmbeddedResources.Count.Should().Be(2);

            vsProject.EmbeddedResources.Remove(packagePath);
            vsProject.EmbeddedResources.Add(packagePath_add);

            vsProject.Write().Should().BeTrue();

            var vsProjectUpdated = new VsProject(file).Read();
            vsProject.EmbeddedResources.Count.Should().Be(2);

            vsProject.EmbeddedResources.Contains(packagePath2).Should().BeTrue();
            vsProject.EmbeddedResources.Contains(packagePath_add).Should().BeTrue();

            File.Delete(file);
        }
    }
}
