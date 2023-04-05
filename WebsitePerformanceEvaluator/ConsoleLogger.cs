using WebsitePerformanceEvaluator.Core.Interfaces;

namespace WebsitePerformanceEvaluator;

public class ConsoleLogger : ILogger
{
    public void Debug(string message)
    {
        Console.WriteLine($"[DEBUG] {message}");
    }

    public void Information(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    public void Warning(string message)
    {
        Console.WriteLine($"[WARNING] {message}");
    }

    public void Error(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    public void Fatal(string message)
    {
        Console.WriteLine($"[FATAL] {message}");
    }
}