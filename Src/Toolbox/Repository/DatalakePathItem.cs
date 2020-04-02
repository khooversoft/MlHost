using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Repository
{
    public class DatalakePathItem
    {
        public string? Name { get; set; }

        public bool? IsDirectory { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public string? ETag { get; set; }

        public long? ContentLength { get; set; }

        public string? Owner { get; set; }

        public string? Group { get; set; }

        public string? Permissions { get; set; }
    }
}
