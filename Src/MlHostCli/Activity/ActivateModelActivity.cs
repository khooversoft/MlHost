using MlHostApi.Models;
using MlHostApi.Repository;
using MlHostApi.Types;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostCli.Activity
{
    internal class ActivateModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;

        public ActivateModelActivity(IOption option, IModelRepository modelRepository)
        {
            _option = option;
            _modelRepository = modelRepository;
        }

        public async Task Activate(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName, _option.VersionId);

            await VerifyModelExist(modelId, token);
            await UpdateConfiguration(modelId, token);
        }

        private async Task VerifyModelExist(ModelId modelId, CancellationToken token)
        {
            IReadOnlyList<string> list = await _modelRepository.Search(modelId, $"^{modelId.Root}\\/{modelId.ModelName}\\/{modelId.VersionId}$", token);
            if (list.Count != 1) throw new ArgumentException($"Model {modelId} does not exist to activate");
        }

        private async Task UpdateConfiguration(ModelId modelId, CancellationToken token)
        {
            HostConfigurationModel model = await _modelRepository.ReadConfiguration(token);

            var dict = model.HostAssignments.ToDictionary(x => x.HostName, x => x.ModelId, StringComparer.OrdinalIgnoreCase);
            dict[_option.HostName] = modelId.ToString();

            model = new HostConfigurationModel()
            {
                HostAssignments = dict
                    .Select(x => new HostAssignment { HostName = x.Key, ModelId = x.Value })
                    .ToList(),
            };

            await _modelRepository.WriteConfiguration(model, token);
        }
    }
}
