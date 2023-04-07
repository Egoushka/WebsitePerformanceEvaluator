namespace WebsitePerformanceEvaluator;

public class ConsoleWrapper
{
    public virtual string ReadLine()
    {
        return Console.ReadLine();
    }

    public virtual void WriteLine(string message = "")
    {
        Console.WriteLine(message);
    }
}