using Autofac;
using Autofac.Core;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using MlProcess.Application;
using MlProcess.Models;
using MlProcess.Services;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Activities
{
    internal class BuildJournalFile
    {
        private readonly IOption _option;
        private readonly ILogger<BuildJournalFile> _logger;
        private readonly ILifetimeScope _container;

        public BuildJournalFile(IOption option, ILogger<BuildJournalFile> logger, ILifetimeScope container)
        {
            _option = option;
            _logger = logger;
            _container = container;
        }

        public async Task Build(ExecContext execContext)
        {
            _logger.LogInformation($"{nameof(BuildJournalFile)}: Reading input file {_option.Input}");
            using var reader = new StreamReader(_option.Input);

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            (await csv.ReadAsync()).VerifyAssert(x => true, "No data in input file");
            csv.ReadHeader().VerifyAssert(x => true, "Could not read headers");

            using RequestJournalWriter requestJournalWriter = _container.Resolve<RequestJournalWriter>().Open(execContext);
            _logger.LogInformation($"{nameof(BuildJournalFile)}: Building processing journal {execContext.InputJournalFile}");

            int id = -1;

            while (await csv.ReadAsync())
            {
                id++;
                string question = csv.GetField(_option.QuestionColumnName);

                if (question.ToNullIfEmpty() == null)
                {
                    _logger.LogInformation($"{nameof(BuildJournalFile)}: Skipping row {id} because question is empty");
                    continue;
                }

                await requestJournalWriter.Write(id, question);

                if (_option.MaxCount != null && id + 1 >= _option.MaxCount) break;
            }

            _logger.LogInformation($"{nameof(BuildJournalFile)}: Read {id + 1} lines");
        }
    }
}
