using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Repository
{
    public interface IDatalakeRepository
    {
        Task Delete(string path, CancellationToken token);

        Task Download(string path, Stream toStream, CancellationToken token);

        Task<bool> Exist(string path, CancellationToken token);

        Task<DatalakePathProperties> GetPathProperties(string path, CancellationToken token);

        Task<IReadOnlyList<DatalakePathItem>> Search(string? path, Func<DatalakePathItem, bool> filter, bool recursive, CancellationToken token);

        Task<byte[]> Read(string path, CancellationToken token);

        Task Upload(Stream fromStream, string toPath, bool force, CancellationToken token);

        Task Write(string path, byte[] data, bool force, CancellationToken token);

        Task DeleteDirectory(string path, CancellationToken token);
    }
}