using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostApi.Repository
{
    public interface IBlobRepository
    {
        Task Delete(string path, CancellationToken token);

        Task Download(string path, Stream toStream, CancellationToken token);

        Task<bool> Exist(string path, CancellationToken token);

        Task<byte[]> Read(string path, CancellationToken token);

        Task<IReadOnlyList<string>> Search(string prefix, Func<string, bool> filter, CancellationToken token);

        Task Upload(Stream fromStream, string toPath, bool force, CancellationToken token);

        Task Write(string path, byte[] data, CancellationToken token);
    }
}