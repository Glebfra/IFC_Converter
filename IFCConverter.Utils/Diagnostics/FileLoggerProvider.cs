using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Utils.Diagnostics
{
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private readonly List<string> _messages = new List<string>();

        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this, categoryName);
        }

        public void Dispose()
        {
        }

        internal void Write(LogLevel level, string category, string message, Exception exception)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{category}] [{message}]";
            if (exception != null)
                entry += Environment.NewLine + exception;

            lock (_lock)
            {
                _messages.Add(entry);
            }
        }

        public void Save()
        {
            lock (_lock)
            {
                File.WriteAllLines(_filePath, _messages);
            }
        }

        private sealed class FileLogger : ILogger
        {
            private readonly string _category;
            private readonly FileLoggerProvider _provider;

            public FileLogger(FileLoggerProvider provider, string category)
            {
                _provider = provider;
                _category = category;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                    return;

                if (formatter == null)
                    throw new ArgumentNullException(nameof(formatter));

                _provider.Write(logLevel, _category, formatter(state, exception), exception);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel != LogLevel.None;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}