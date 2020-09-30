using MlProcess.Application;
using MlProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class ResponseJournalWriter : IDisposable
    {
        private readonly IOption _option;
        private readonly IJson _json;
        private StreamWriter? _writer;
        private ActionBlock<string>? _writeJournalBlock;

        public ResponseJournalWriter(IOption option, IJson json)
        {
            _option = option;
            _json = json;
        }

        public Task<ResponseJournalWriter> Open(ExecContext execContext)
        {
            _writer = new StreamWriter(execContext.OutputJournalFile);

            _writeJournalBlock = new ActionBlock<string>(async x => await _writer.WriteLineAsync(x));

            return Task.FromResult(this);
        }

        public void Post(ResponseJournal responseJournal, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            _writeJournalBlock
                .VerifyNotNull("Not open")
                .Post(_json.Serialize(responseJournal));
        }

        public async Task Close()
        {
            ActionBlock<string>? actionBlock = Interlocked.Exchange(ref _writeJournalBlock, null!);
            if (actionBlock != null)
            {
                actionBlock.Complete();
                await actionBlock.Completion;
            }

            Interlocked.Exchange(ref _writer, null!)?.Dispose();
        }

        public void Dispose() => Close().Wait();
    }
}
