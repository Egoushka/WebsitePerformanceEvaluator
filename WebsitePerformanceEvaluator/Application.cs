namespace WebsitePerformanceEvaluator;

public class Application
{
    private TaskRunner TaskRunner { get; }
    public Application(TaskRunner taskRunner)
    {
        TaskRunner = taskRunner;
    }

    public void Run()
    {
        TaskRunner.Start();
    }

}