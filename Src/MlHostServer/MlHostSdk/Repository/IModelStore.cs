﻿using MlHostSdk.Models;
using MlHostSdk.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;

namespace MlHostSdk.Repository
{
    public interface IModelStore
    {
        Task Upload(string modelVersionFile, ModelId modelId, bool force, CancellationToken token);

        Task Download(ModelId modelId, string toFile, CancellationToken token);

        Task<bool> Delete(ModelId modelId, CancellationToken token);

        Task<bool> Exist(ModelId modelId, CancellationToken token);

        Task<DatalakePathProperties> GetPathProperties(ModelId modelId, CancellationToken token);

        Task<IReadOnlyList<DatalakePathItem>> Search(string? prefix, string pattern, CancellationToken token);

        Task<T?> Read<T>(string path, CancellationToken token) where T : class;

        Task Write<T>(string path, T value, CancellationToken token) where T : class;
    }
}