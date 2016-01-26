using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace EFLogging
{
    public class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger();
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
                File.AppendAllText(@"C:\temp\log.txt", formatter(state, exception));
                Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScopeImpl(object state)
            {
                return null;
            }
        } 
    }
}
