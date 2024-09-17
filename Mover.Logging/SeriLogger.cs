using Serilog.Formatting.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Logging
{
    public class SeriLogger : ISeriLogger
    {
        private static readonly ILogger _logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(new JsonFormatter(), $"Logs/ErrorLog-{DateTime.Now:yyyyMMdd}.json", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
            .CreateLogger();


        public SeriLogger()
        {
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }

        public void Information(string message)
        {
            _logger.Information(message);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }
    }
}
