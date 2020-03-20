using MlHostApi.Repository;
using MlHostApi.Tools;
using MlHostApi.Types;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostCli.Activity
{
    internal class DeleteModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;

        public DeleteModelActivity(IOption option, IModelRepository modelRepository)
        {
            _option = option;
            _modelRepository = modelRepository;
        }

        public Task Delete(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName, _option.VersionId);

            return _modelRepository.Delete(modelId, token);
        }
    }
}
