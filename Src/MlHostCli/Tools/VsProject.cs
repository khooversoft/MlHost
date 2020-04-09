using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Toolbox.Tools;

namespace MlHostCli.Tools
{
    internal class VsProject
    {
        private readonly string _filePath;
        private const string _projectText = "Project";
        private const string _itemGroupText = "ItemGroup";
        private const string _embeddedResourceText = "EmbeddedResource";
        private const string _includeText = "Include";

        public VsProject(string filePath)
        {
            _filePath = filePath;
        }

        public HashSet<string> EmbeddedResources { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public VsProject Add(string value)
        {
            value.VerifyNotEmpty(nameof(value));
            EmbeddedResources.Add(value);

            return this;
        }
        public VsProject Read()
        {
            _filePath.VerifyAssert(x => File.Exists(x), $"{_filePath} does not exist");

            XDocument projectDocument = XDocument.Load(_filePath);

            EmbeddedResources = projectDocument
                .Element(_projectText)
                .Elements(_itemGroupText)
                .Elements(_embeddedResourceText)
                .Attributes(_includeText)
                .Select(x => x.Value)
                .Func(x => new HashSet<string>(x, StringComparer.OrdinalIgnoreCase));

            return this;
        }

        public bool Write()
        {
            _filePath.VerifyAssert(x => File.Exists(x), $"{_filePath} does not exist");

            XDocument projectDocument = XDocument.Load(_filePath);

            if (!HasChanged(projectDocument)) return false;

            RemoveDeleted(projectDocument);
            AddNew(projectDocument);
            projectDocument = CleanUp(projectDocument);

            projectDocument.Save(_filePath);
            return true;
        }

        private HashSet<String> GetEmbeddedResources(XDocument projectDocument) => projectDocument
                .Element(_projectText)
                .Elements(_itemGroupText)
                .Elements(_embeddedResourceText)
                .Attributes(_includeText)
                .Select(x => x.Value)
                .Func(x => new HashSet<string>(x, StringComparer.OrdinalIgnoreCase));

        private bool HasChanged(XDocument projectDocument) =>
            Enumerable.SequenceEqual(
                EmbeddedResources.OrderBy(x => x, StringComparer.OrdinalIgnoreCase),
                GetEmbeddedResources(projectDocument).OrderBy(x => x, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase) == false;

        private void RemoveDeleted(XDocument projectDocument)
        {
            IReadOnlyList<string> removedItems = GetEmbeddedResources(projectDocument)
                .Except(EmbeddedResources, StringComparer.OrdinalIgnoreCase)
                .ToList();

            projectDocument
                .Element(_projectText)
                .Elements(_itemGroupText)
                .Elements(_embeddedResourceText)
                .Where(x => x.Attributes(_includeText).Where(x => removedItems.Contains(x.Value, StringComparer.OrdinalIgnoreCase)).Any())
                .Remove();
        }

        private void AddNew(XDocument projectDocument)
        {
            IReadOnlyList<string> addItems = EmbeddedResources
                .Except(GetEmbeddedResources(projectDocument), StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Find the first item group that has embedded resources or just get the first item group if none found
            XElement itemGroup =
                projectDocument
                    .Element(_projectText)
                    .Elements(_itemGroupText)
                    .Where(x => x.Elements(_embeddedResourceText).Count() > 0)
                    .First()
                ??
                projectDocument
                    .Element(_projectText)
                    .Element(_itemGroupText)
                    .VerifyNotNull($"No {_itemGroupText} found to add embeddedReference");

            addItems
                .ForEach(x => itemGroup.Add(new XElement(_embeddedResourceText, new XAttribute(_includeText, x))));
        }

        private XDocument CleanUp(XDocument projectDocument)
        {
            // Remove empty nodes
            projectDocument
                .Element(_projectText)
                .Elements(_itemGroupText)
                .Where(x => x.Nodes().Count() == 0)
                .Remove();

            // Format to default
            using (Stream memoryStream = new MemoryStream())
            {
                projectDocument.Save(memoryStream);
                memoryStream.Position = 0;

                projectDocument = XDocument.Load(memoryStream, LoadOptions.PreserveWhitespace);
            }

            projectDocument
                .Element(_projectText)
                .Elements()
                .ToList()
                .ForEach(x => x.AddAfterSelf(new XText(Environment.NewLine + " ")));

            return projectDocument;
        }
    }
}
