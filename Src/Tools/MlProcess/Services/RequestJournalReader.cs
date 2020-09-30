using MlProcess.Application;
using MlProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class RequestJournalReader
    {
        private readonly IOption _option;
        private readonly IJson _json;

        public RequestJournalReader(IOption option, IJson json)
        {
            _option = option;
            _json = json;
        }

        public async Task ReadAndProcess(ExecContext execContext, Func<RequestJournal, Task> post)
        {
            var runBlock = new ActionBlock<RequestJournal>(async x => await post(x), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _option.Tasks });
            using StreamReader reader = new StreamReader(execContext.InputJournalFile.VerifyNotEmpty(nameof(execContext.InputJournalFile)));

            while (!execContext.Token.IsCancellationRequested)
            {
                string? inputLine = await reader.ReadLineAsync();
                if (inputLine == null) break;

                RequestJournal journalRecord = _json.Deserialize<RequestJournal>(inputLine);
                runBlock.Post(journalRecord);
            }

            runBlock.Complete();
            await runBlock.Completion;
        }
    }
}
