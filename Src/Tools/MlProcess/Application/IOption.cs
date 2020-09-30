using System.Collections.Generic;
using Toolbox.Models;
using Toolbox.Services;

namespace MlProcess.Application
{
    internal interface IOption
    {
        bool Help { get; }

        string? ConfigFile { get; }

        bool Run { get; }

        bool Append { get; }

        string Input { get; }

        string QuestionColumnName { get; }

        string Output { get; }

        int Tasks { get; }

        string? LogFile { get; }

        int SampleRate { get; }

        int? MaxCount { get; }

        IList<ModelOption> Models { get; }

        ISecretFilter? SecretFilter { get; }
    }
}