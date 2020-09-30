using System.Collections.Generic;
using Toolbox.Services;

namespace MlProcess.Application
{
    internal class Option : IOption
    {
        public bool Help { get; set; }

        public string? ConfigFile { get; set; }

        public bool Run { get; set; }

        public bool Append { get; set; }

        public string Input { get; set; } = null!;

        public string QuestionColumnName { get; set; } = null!;

        public string Output { get; set; } = null!;

        public int Tasks { get; set; } = 5;

        public string? LogFile { get; set; }

        public int SampleRate { get; set; } = 5;

        public int? MaxCount { get; set; }

        public IList<ModelOption> Models { get; set; } = null!;

        public ISecretFilter? SecretFilter { get; set; }
    }
}
