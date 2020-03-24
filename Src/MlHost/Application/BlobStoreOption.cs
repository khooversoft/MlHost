﻿using MlHost.Tools;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    internal class BlobStoreOption
    {
        public string? ContainerName { get; set; }

        public string? AccountName { get; set; }

        public string? AccountKey { get; set; }
    }
}
