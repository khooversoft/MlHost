using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Logging
{
    public static class FileLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string loggingFolder, string baseLogFileName, int limit = 10)
        {
            builder.AddProvider(new FileLoggerProvider(loggingFolder, baseLogFileName, limit));
            return builder;
        }
    }
}
