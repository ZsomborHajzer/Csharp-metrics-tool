Console.WriteLine("Please submit the absolute path of your project: ");

var projectPath = Console.ReadLine();

if (projectPath == null || projectPath.Length == 0)
{
    throw new Exception("Invalid project Path");
}

Validator validator = new Validator(projectPath);

var files = new List<string>();

try
{
    files = validator.GetCsharpFiles();
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}

var filters = StringFilters.GetFilters(StringFilters.NotEmpty, StringFilters.RemoveImports);

FileParser parser = new FileParser(files, filters);

List<Document> documents = parser.ParseFiles();

MetricAnalyzer metricAnalyzer = new MetricAnalyzer(documents, projectPath);

metricAnalyzer.AddMetric(new LinesOfCode());

metricAnalyzer.RunMetrics();