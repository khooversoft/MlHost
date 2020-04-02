using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake.Models;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace Toolbox.Repository
{
    public static class RepositoryExtensions
    {
        public static BlobInfo ConvertTo(this BlobItem subject)
        {
            subject.VerifyNotNull(nameof(subject));

            return new BlobInfo
            {
                Name = subject.Name,
                LastModified = subject.Properties.LastModified,
                CreatedOn = subject.Properties.CreatedOn,
                ETag = subject.Properties.ETag?.ToString(),
                ContentHash = subject.Properties.ContentHash,
                ContentType = subject.Properties.ContentType,
                ContentLength = subject.Properties.ContentLength,
            };
        }

        public static DatalakePathItem ConvertTo(this PathItem subject)
        {
            return new DatalakePathItem
            {
                Name = subject.Name,
                IsDirectory = subject.IsDirectory,
                LastModified = subject.LastModified,
                ETag = subject.ETag.ToString(),
                ContentLength = subject.ContentLength,
                Owner = subject.Owner,
                Group = subject.Group,
                Permissions = subject.Permissions,
            };
        }

        public static DatalakePathProperties ConvertTo(this PathProperties subject)
        {
            return new DatalakePathProperties
            {
                LastModified = subject.LastModified,
                ContentEncoding = subject.ContentEncoding,
                ETag = subject.ETag.ToString(),
                ContentType = subject.ContentType,
                ContentLength = subject.ContentLength,
                CreatedOn = subject.CreatedOn,
            };
        }

        public static void Verify(this BlobStoreOption subject)
        {
            subject.AccountName.VerifyNotNull(nameof(subject.AccountName));
            subject.AccountKey.VerifyNotNull(nameof(subject.AccountKey));
            subject.ContainerName.VerifyNotNull(nameof(subject.ContainerName));
        }
    }
}
