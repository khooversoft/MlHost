using Autofac;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using MlHostApi.Models;
using MlProcess.Application;
using MlProcess.Models;
using MlProcess.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Activities
{
    internal class AppendResults
    {
        private readonly IOption _option;
        private readonly IJson _json;
        private readonly ILogger<AppendResults> _logging;
        private readonly ILifetimeScope _container;

        public AppendResults(IOption option, IJson json, ILogger<AppendResults> logger, ILifetimeScope container)
        {
            _option = option;
            _json = json;
            _logging = logger;
            _container = container;
        }

        public async Task Build(ExecContext execContext)
        {
            _logging.LogInformation("Building results");

            IReadOnlyDictionary<int, ResponseResult> journal = await _container.Resolve<ResponseJournalReader>().Read(execContext);

            using var csvReaderStream = new StreamReader(_option.Input);
            using var csvReader = new CsvReader(csvReaderStream, CultureInfo.InvariantCulture);

            using var csvWriterStream = new StreamWriter(_option.Output);
            using var csvWriter = new CsvWriter(csvWriterStream, CultureInfo.InvariantCulture);

            var modelNames = _option.Models
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToList();

            int id = -1;
            int columnCount = await WriteHeader(csvReader, csvWriter, modelNames);

            var emptyList = Enumerable.Empty<ModelResult>().ToList();

            while (await csvReader.ReadAsync())
            {
                id++;

                Enumerable.Range(0, columnCount)
                    .Select(x => csvReader.GetField(x))
                    .ForEach(x => csvWriter.WriteField(x));

                IReadOnlyList<ModelResult> modelResults = journal.ContainsKey(id) ? journal[id].ModelResults! : emptyList;

                modelNames
                    .GroupJoin(modelResults, o => o, i => i.ModelName, (o, i) => i)
                    .SelectMany(x => x)
                    .Select(x => getLabel(x))
                    .ForEach(x => csvWriter.WriteField(x));

                await csvWriter.NextRecordAsync();
            }

            static string getLabel(ModelResult modelResult)
            {
                var str = modelResult.Intents
                    ?.OrderByDescending(x => x.Score)
                    ?.Select(x => x.Label)
                    ?.FirstOrDefault() ?? string.Empty;

                return str;
            }
        }

        private async Task<int> WriteHeader(CsvReader csvReader, CsvWriter csvWriter, IEnumerable<string> appendHeaders)
        {
            (await csvReader.ReadAsync()).VerifyAssert(x => x == true, $"No data in {_option.Input}");
            csvReader.ReadHeader().VerifyAssert(x => true, "Could not read headers");
            int columnCount = csvReader.Context.HeaderRecord.Length;

            Enumerable.Range(0, columnCount)
                .Select(x => csvReader.GetField(x))
                .ForEach(x => csvWriter.WriteField(x));

            appendHeaders
                .ForEach(x => csvWriter.WriteField(x));

            await csvWriter.NextRecordAsync();

            return columnCount;
        }
    }
}
