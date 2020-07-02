using Autofac;
using Microsoft.Extensions.Logging;
using MlHostApi.Models;
using MlProcess.Application;
using MlProcess.Models;
using MlProcess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlProcess.Activities
{
    internal class RunModels
    {
        private readonly IOption _option;
        private readonly IJson _json;
        private readonly ILogger<RunModels> _logger;
        private readonly IMetricSampler _metricSampler;
        private readonly ILifetimeScope _container;
        private readonly Dictionary<string, HttpRest> _httpEndpoints;

        public RunModels(IOption option, IJson json, ILogger<RunModels> logger, IMetricSampler metricSampler, ILifetimeScope container)
        {
            _option = option;
            _json = json;
            _logger = logger;
            _metricSampler = metricSampler;
            _container = container;

            _httpEndpoints = _option.Models
                .ToDictionary(x => x.Name, x => container.Resolve<HttpRest>(new NamedParameter("uri", x.Uri)), StringComparer.OrdinalIgnoreCase);
        }

        public async Task Run(ExecContext execContext)
        {
            _logger.LogInformation($"{nameof(RunModels)}: Processing data");
            _metricSampler.Start();

            RequestJournalReader requestJournalReader = _container.Resolve<RequestJournalReader>();
            using ResponseJournalWriter responseJournalWriter = await _container.Resolve<ResponseJournalWriter>().Open(execContext);

            _logger.LogInformation($"{nameof(RunModels)}: Output to journal {execContext.OutputJournalFile}");

            await requestJournalReader.ReadAndProcess(execContext, async x => await InvokeModels(x, responseJournalWriter, execContext.Token));
            await responseJournalWriter.Close();

            _metricSampler.Stop();
            _logger.LogInformation($"{nameof(RunModels)}: Completed processing of data");
        }

        private async Task InvokeModels(RequestJournal journalRecord, ResponseJournalWriter writeJournalWriter, CancellationToken token)
        {
            var tasks = new List<Task>();
            if (token.IsCancellationRequested) return;

            foreach (var model in _option.Models)
            {
                tasks.Add(InvokeModel(journalRecord, model, writeJournalWriter, token));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private async Task InvokeModel(RequestJournal journalRecord, ModelOption apiOption, ResponseJournalWriter writeJournalWriter, CancellationToken token)
        {
            try
            {
                PredictResponse response = await _httpEndpoints[apiOption.Name].Invoke(journalRecord.Question!, token);
                _metricSampler.Add(apiOption.Name);

                var newJournal = new ResponseJournal
                {
                    RequestId = journalRecord.RequestId,
                    Question = journalRecord.Question,
                    ModelName = apiOption.Name,
                    Intents = response.Intents?.ToList() ?? response.Intent?.ToList(),
                };

                writeJournalWriter.Post(newJournal, token);
            }
            catch (TaskCanceledException) { }
            catch (CircuitBreakerException) { }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Request to model failed, skipping {journalRecord.RequestId} for model {apiOption.Name}");

                // Swallow error because there will be no recording to the response journal
            }
        }
    }
}
