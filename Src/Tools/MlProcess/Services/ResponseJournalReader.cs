using MlProcess.Application;
using MlProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class ResponseJournalReader
    {
        private readonly IOption _option;
        private readonly IJson _json;

        public ResponseJournalReader(IOption option, IJson json)
        {
            _option = option;
            _json = json;
        }

        public async Task<IReadOnlyDictionary<int, ResponseResult>> Read(ExecContext execContext)
        {
            execContext.VerifyNotNull(nameof(execContext));

            IReadOnlyList<ResponseJournal> responses = await ReadResponses(execContext);

            return responses
                .GroupBy(x => x.RequestId)
                .Select(x =>
                    new ResponseResult
                    {
                        RequestId = x.Key,
                        Question = x.First().Question,
                        ModelResults = x.Select(y =>
                            new ModelResult
                            {
                                ModelName = y.ModelName,
                                Intents = y.Intents.ToList(),
                            }).ToList(),
                    })
                .ToDictionary(x => x.RequestId, x => x);
        }

        private async Task<IReadOnlyList<ResponseJournal>> ReadResponses(ExecContext execContext)
        {
            var list = new List<ResponseJournal>();

            using var reader = new StreamReader(execContext.OutputJournalFile.VerifyNotEmpty(nameof(execContext.OutputJournalFile)));

            while (!execContext.Token.IsCancellationRequested)
            {
                string? inputLine = await reader.ReadLineAsync();
                if (inputLine == null) break;

                ResponseJournal journalRecord = _json.Deserialize<ResponseJournal>(inputLine);
                list.Add(journalRecord);
            }

            return list;
        }
    }
}
