using MlProcess.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Toolbox.Tools;

namespace MlProcess.Models
{
    internal class ExecContext
    {
        public ExecContext(CancellationToken cancellationToken, IOption option)
        {
            option.VerifyNotNull(nameof(option));
            option.Input.VerifyNotEmpty(nameof(option.Input));

            Token = cancellationToken;
            InputJournalFile = Path.ChangeExtension(option.Input, ".inputJournal");
            OutputJournalFile = Path.ChangeExtension(option.Input, ".outputJournal");

        }

        public CancellationToken Token { get; }

        public string InputJournalFile { get; }

        public string OutputJournalFile { get; }
    }
}
