using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EFLogging
{
    public class MyFilteredLoggerProvider : ILoggerProvider
    {
        private static string[] _categories =
        {
            typeof(Microsoft.Data.Entity.Storage.Internal.RelationalCommandBuilderFactory).FullName,
            typeof(Microsoft.Data.Entity.Storage.Internal.SqlServerConnection).FullName
        };

        public ILogger CreateLogger(string categoryName)
        {
            if( _categories.Contains(categoryName))
            {
                return new MyLogger();
            }

            return new NullLogger();
        }

        public void Dispose()
        { }

        private class MyLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            {
                Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScopeImpl(object state)
            {
                return null;
            }
        }

        private class NullLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            { }

            public IDisposable BeginScopeImpl(object state)
            {
                return null;
            }
        }
    }
}
