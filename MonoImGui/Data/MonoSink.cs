using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;
using System.Text;

namespace MonoImGui.Data
{
    public static class MonoSinkExtensions
    {
        public static LoggerConfiguration MonoSink(this LoggerSinkConfiguration loggerConfiguration, IFormatProvider formatProvider = null, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            return loggerConfiguration.Sink(new MonoSink(formatProvider), restrictedToMinimumLevel);
        }
    }

    public class MonoSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        public static StringBuilder Output;

        public MonoSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
            Output = new StringBuilder();
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            Output.AppendLine(DateTimeOffset.Now.ToString("HH:mm:ss") + " " + message);

            Main.ScrollLogToBottom = true;
        }
    }
}
