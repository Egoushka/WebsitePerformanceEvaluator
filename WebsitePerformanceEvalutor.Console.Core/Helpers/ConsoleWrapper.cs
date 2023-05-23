namespace WebsitePerformanceEvalutor.Console.Core.Helpers;

public class ConsoleWrapper
{
    public virtual string ReadLine()
    {
        return System.Console.ReadLine();
    }

    public virtual void WriteLine(string message = "")
    {
        System.Console.WriteLine(message);
    }
}