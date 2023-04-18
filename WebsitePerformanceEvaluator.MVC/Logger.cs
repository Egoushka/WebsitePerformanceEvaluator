using ILogger = WebsitePerformanceEvaluator.Infrustructure.Interfaces.ILogger;

namespace WebsitePerformanceEvaluator.MVC;

public class Logger : Infrustructure.Interfaces.ILogger
{
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }
    public void Error(string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}";
        
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }
}