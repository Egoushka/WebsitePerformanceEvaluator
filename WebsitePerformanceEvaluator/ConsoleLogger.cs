using Serilog;
using Serilog.Events;

namespace WebsitePerformanceEvaluator;

public class ConsoleLogger : ILogger
{
    private readonly ILogger _logger;

    public ConsoleLogger()
    {
        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    public void Debug(string messageTemplate, params object[] propertyValues)
    {
        _logger.Debug(messageTemplate, propertyValues);
    }

    public void Error(Exception ex, string messageTemplate, params object[] propertyValues)
    {
        _logger.Error(ex, messageTemplate, propertyValues);
    }

    public void Fatal(Exception ex, string messageTemplate, params object[] propertyValues)
    {
        _logger.Fatal(ex, messageTemplate, propertyValues);
    }

    public void Information(string messageTemplate, params object[] propertyValues)
    {
        _logger.Information(messageTemplate, propertyValues);
    }

    public void Verbose(string messageTemplate, params object[] propertyValues)
    {
        _logger.Verbose(messageTemplate, propertyValues);
    }

    public void Warning(string messageTemplate, params object[] propertyValues)
    {
        _logger.Warning(messageTemplate, propertyValues);
    }

    public void Write(LogEvent logEvent)
    {
        _logger.Write(logEvent);
    }

    public bool IsEnabled(LogEventLevel level)
    {
        return _logger.IsEnabled(level);
    }
}