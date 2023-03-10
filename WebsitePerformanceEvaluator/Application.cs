namespace WebsitePerformanceEvaluator;

public class Application
{
    private TaskRunner TaskRunner { get; }
    public Application(TaskRunner taskRunner)
    {
        TaskRunner = taskRunner;
    }

    public async Task Run()
    {
        await TaskRunner.Start();
    }
}