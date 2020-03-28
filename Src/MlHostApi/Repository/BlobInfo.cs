using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Repository
{
    public class BlobInfo
    {
        public string? Name { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public string? ETag { get; set; }
        public byte[]? ContentHash { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }
    }
}
