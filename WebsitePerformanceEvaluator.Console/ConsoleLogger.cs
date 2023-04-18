using WebsitePerformanceEvaluator.Infrastructure.Interfaces;

namespace WebsitePerformanceEvaluator.Console;

public class ConsoleLogger : ILogger
{
    public void Error(string message)
    {
        System.Console.WriteLine($"[ERROR] {message}");
    }
}