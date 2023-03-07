using ConsoleTableExt;

namespace WebsitePerformanceEvaluator;

public static class ConsoleHelper
{
    public static void PrintTable(IEnumerable<string> headers, IEnumerable<string> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .ExportAndWriteLine();
    }

    public static void PrintTable(List<string> headers, List<Tuple<string, int>> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .WithFormatter(1, (value) => $"{value} ms")
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .ExportAndWriteLine();
    }
    public static string GetInput(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }
}