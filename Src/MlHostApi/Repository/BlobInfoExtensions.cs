using Azure.Storage.Blobs.Models;
using MlHostApi.Tools;

namespace MlHostApi.Repository
{
    public static class BlobInfoExtensions
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
    }
}
