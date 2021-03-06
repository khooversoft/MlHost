﻿using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake.Models;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

namespace Toolbox.Repository
{
    public static class StoreExtensions
    {
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
    }
}
