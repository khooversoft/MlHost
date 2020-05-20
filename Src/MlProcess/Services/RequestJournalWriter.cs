using MlProcess.Application;
using MlProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class RequestJournalWriter : IDisposable
    {
        private readonly IOption _option;
        private readonly IJson _json;
        private StreamWriter? _writer;

        public RequestJournalWriter(IOption option, IJson json)
        {
            _option = option;
            _json = json;
        }

        public RequestJournalWriter Open(ExecContext execContext)
        {
            _writer = new StreamWriter(execContext.InputJournalFile);
            return this;
        }

        public async Task Write(int requestId, string question)
        {
            requestId.VerifyAssert(x => x >= 0, $"{nameof(requestId)} is out of range");
            question.VerifyNotEmpty(nameof(question));

            var request = new RequestJournal
            {
                RequestId = requestId,
                Question = question,
            };

            await _writer
                .VerifyNotNull("Journal is not open")
                .WriteLineAsync(_json.Serialize(request));
        }

        public void Dispose() => Interlocked.Exchange(ref _writer, null!)?.Dispose();
    }
}
