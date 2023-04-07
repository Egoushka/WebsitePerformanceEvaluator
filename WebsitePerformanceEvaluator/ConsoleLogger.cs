using WebsitePerformanceEvaluator.Core.Interfaces;

namespace WebsitePerformanceEvaluator;

public class ConsoleLogger : ILogger
{
    public void Error(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }
}