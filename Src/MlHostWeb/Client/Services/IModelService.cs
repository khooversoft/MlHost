using System.Collections.Generic;

namespace MlHostWeb.Client.Services
{
    public interface IModelService
    {
        IReadOnlyList<ModelItem> Items { get; }

        ModelItem GetModel(string versionId);
    }
}