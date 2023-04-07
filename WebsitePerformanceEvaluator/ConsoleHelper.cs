using ConsoleTableExt;

namespace WebsitePerformanceEvaluator;

public class ConsoleHelper
{
    public void PrintTable(IEnumerable<string> headers, IEnumerable<string> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .ExportAndWriteLine();
    }

    public void PrintTable(IEnumerable<string> headers, IEnumerable<Tuple<string, long?>> rows)
    {
        ConsoleTableBuilder
            .From(rows.ToList())
            .WithColumn(headers.ToList())
            .WithFormatter(1, value => $"{value} ms")
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .ExportAndWriteLine();
    }
}