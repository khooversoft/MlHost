﻿using System;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class ModelUrl
    {
        private readonly string? _baseAddress;

        public ModelUrl()
        {
        }

        public ModelUrl(string baseAddress)
        {
            baseAddress.VerifyNotEmpty(nameof(baseAddress));

            _baseAddress = baseAddress;
        }

        public string GetRequestUrl() => CreateUrl("api/submit");

        public string GetLogsUrl() => CreateUrl("api/ping/logs");

        public string GetBatchRequestUrl() => CreateUrl("api/submit");

        public string GetPingUrl() => CreateUrl("api/ping");

        public string GetPingRunningUrl() => CreateUrl("api/ping/running");

        public string GetPingReadyUrl() => CreateUrl("api/ping/ready");

        private string CreateUrl(string path) => _baseAddress.ToNullIfEmpty() switch
        {
            string v => new UriBuilder(v) { Path = path }.ToString(),

            _ => path,
        };
    }
}