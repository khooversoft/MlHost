using Microsoft.Extensions.Logging;
using MlHostCli.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class BuildSwaggerActivity
    {
        private const string _masterSwaggerId = "MlHostCli.Config.SwaggerMaster.json";
        private readonly IOption _option;
        private readonly ILogger<BuildSwaggerActivity> _logger;

        public BuildSwaggerActivity(IOption option, ILogger<BuildSwaggerActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public Task Write(CancellationToken token)
        {
            string json = typeof(BuildSwaggerActivity).GetResourceAsString(_masterSwaggerId);

            JObject swaggerMaster = JObject.Parse(json);

            swaggerMaster["info"]!["title"] = _option.PropertyResolver!.Resolve(swaggerMaster["info"]!["title"]!.ToString());
            swaggerMaster["info"]!["description"] = _option.PropertyResolver!.Resolve(swaggerMaster["info"]!["description"]!.ToString());
            swaggerMaster["info"]!["x-apiPath"] = _option.PropertyResolver!.Resolve(swaggerMaster["info"]!["x-apiPath"]!.ToString());
            swaggerMaster["host"] = _option.PropertyResolver!.Resolve(swaggerMaster["host"]!.ToString()).ToLower();

            string path = _option.PropertyResolver!.Resolve(_option.SwaggerFile!);
            _logger.LogInformation($"Writing swagger file {path} for {_option.Environment} - {_option.ModelName}.");
            _logger.LogInformation($"info-title = {swaggerMaster["info"]!["title"]}.");
            _logger.LogInformation($"info-description = {swaggerMaster["info"]!["description"]}.");
            _logger.LogInformation($"host = {swaggerMaster["host"]!}.");

            using StreamWriter file = File.CreateText(path);
            using JsonTextWriter writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };

            swaggerMaster.WriteTo(writer);

            return Task.CompletedTask;
        }
    }
}
