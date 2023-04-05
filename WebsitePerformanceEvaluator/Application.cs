namespace WebsitePerformanceEvaluator;

public class Application
{
    private readonly TaskRunner _taskRunner;

    public Application(TaskRunner taskRunner)
    {
        _taskRunner = taskRunner;
    }

    public async Task Run()
    {
        await _taskRunner.Start();
    }
}