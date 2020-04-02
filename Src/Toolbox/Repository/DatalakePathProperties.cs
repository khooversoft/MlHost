using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Repository
{
    public class DatalakePathProperties
    {
        public DateTimeOffset LastModified { get; set; }

        public string? ContentEncoding { get; set; }

        /// <summary>
        /// The ETag contains a value that you can use to perform operations conditionally.
        /// If the request version is 2011-08-18 or newer, the ETag value will be in quotes.
        /// </summary>
        public string? ETag { get; set; }

        public string? ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
