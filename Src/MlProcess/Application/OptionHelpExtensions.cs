using Microsoft.Extensions.Logging;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Tools;

namespace MlProcess.Application
{
    internal static class OptionHelpExtensions
    {
        public static IReadOnlyList<string> GetHelp(this IOption _)
        {
            return new[]
            {
                "ML Process - Process data with ML APIs",
                "",
                "Help                       : Display help",
                "",
                "Run ML processes on data",
                "  Run                            : Run process",
                "  ConfigFile={file}              : Read configuration file",
                "",                               
                "  Input={file}                   : Input data file to be processed",
                "  HasHeader                      : Input file has a header for the first row",
                "  ColumnNumber={columnNumber}    : Output column number",
                "",                               
                "  Output={name}                  : Output file from process.  Default = *.processed",
                "",
                "  Model:n:Api={uri}              : Model's version",
                "  Model:n:Name={name}            : Model's name (column in output)",
                "",
                "  Tasks={n}                      : Number of tasks to use.  Default = 5.",
                "  LogFile={path}                 : Log file to log (without extensions). (optional)",
                "  SampleRate={seconds}           : Sample rate in seconds Default = 5.",
                "  MaxCount={count}               : Process maximum count, default is no limit.",
                "",
                "Append data to source",
                "  Append                         : Use the result journal to append to output file.",
                "  Input={file}                   : Input data file to be processed",
                "  Output={name}                  : Output file from process.  Default = *.processed",
            };
        }

        public static void DumpConfigurations(this IOption option, ILogger logger)
        {
            const int maxWidth = 80;

            option.GetConfigValues()
                .Select(x => "  " + x)
                .Prepend(new string('=', maxWidth))
                .Prepend("Current configurations")
                .Prepend(string.Empty)
                .Append(string.Empty)
                .Append(string.Empty)
                .ForEach(x => logger.LogInformation(x));
        }
    }
}
