using System.Text;
using Xunit;

namespace WebsitePerformanceEvaluator.Tests;

public class ConsoleHelperTests
{
    private readonly ConsoleHelper _consoleHelper;
    private readonly StringWriter _writer;
    
    public ConsoleHelperTests()
    {
        _consoleHelper = new ConsoleHelper();
        _writer = new StringWriter(new StringBuilder());
        
        Console.SetOut(_writer);
    }
    [Fact]
    public void PrintTable_ValidData_CheckIsHeadersPrinted()
    {
        // Arrange
        var headers = new List<string> { "Header1", "Header2" };
        var rows = new List<string> { "Row1", "Row2" };

        // Act
        _consoleHelper.PrintTable(headers, rows);

        // Assert
        var output = _writer.ToString();
        
        Assert.Contains("Header1", output);
        Assert.Contains("Header2", output);
    }
    [Fact]
    public void PrintTable_ValidData_CheckIsRowsPrinted()
    {
        // Arrange
        var headers = new List<string> { "Header1", "Header2" };
        var rows = new List<string> { "Row1", "Row2" };

        // Act
        _consoleHelper.PrintTable(headers, rows);

        // Assert
        var output = _writer.ToString();
        
        Assert.Contains("Row1", output);
        Assert.Contains("Row2", output);
    }

    [Fact]
    public void PrintTable_ValidData_CheckIsTimeResponsePrinted()
    {
        // Arrange
        var headers = new List<string> { "Header1", "Header2" };
        var rows = new List<Tuple<string, long?>> { 
            Tuple.Create("Row1", (long?)100), 
            Tuple.Create("Row2", (long?)200) 
        };

        // Act
        _consoleHelper.PrintTable(headers, rows);

        // Assert
        var output = _writer.ToString();
        
        Assert.Contains("ms", output);
        Assert.Contains("100", output);
        Assert.Contains("200", output);
    }
}