using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EFLogging
{
    class SqlCommandLoggingProvider :
        ILoggerProvider,
        ILogger
    {
        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == DbLoggerCategory.Database.Command.Name)
            {
                return this;
            }

            return NullLogger.Instance;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id != RelationalStrings.LogRelationalLoggerExecutedCommand.EventId.Id)
            {
                return;
            }

            Trace.WriteLine($@"Executed EF SQL command:
{state.ToString()}");
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => null;

        public void Dispose()
        {
        }
    }
}