using ConsoleTableExt;

namespace WebsitePerformanceEvaluator;

public class ConsoleHelper
{
    public static void PrintTable(IEnumerable<string> headers, IEnumerable<string> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .ExportAndWriteLine();
    }

    public static void PrintTable(List<string> headers, List<Tuple<string, int>> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .ExportAndWriteLine();
    }
}