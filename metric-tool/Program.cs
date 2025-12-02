Console.WriteLine("Please submit the absolute path of your project: ");

List<string> outcomes = new List<string>();

List<string> ProjectPaths = [
    "/home/zsombor/Workspace/University/Academic/repos/Final-Year-Project-Management-System",
    "/home/zsombor/Workspace/University/Academic/repos/jellyfin",
    "/home/zsombor/Workspace/University/Academic/repos/University-Management-System",
    //"/home/zsombor/Workspace/University/Academic/repos/REMS",
    "/home/zsombor/Workspace/University/Academic/repos/PDFsharp",
    "/home/zsombor/Workspace/University/Academic/repos/Student-Management-System"];

foreach (var path in ProjectPaths)
{
    outcomes.Add("----------------------------------------------------------------------");
    outcomes.Add(path);
    if (path == null || path.Length == 0)
    {
        throw new Exception("Invalid project Path");
    }

    Validator validator = new Validator(path);

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

    MetricAnalyzer metricAnalyzer = new MetricAnalyzer(documents, path);

    metricAnalyzer.AddMetric(new LinesOfCode());

    metricAnalyzer.AddMetric(new CyclomaticComplexity());

    metricAnalyzer.AddMetric(new HalsteadEffort());

    metricAnalyzer.RunMetrics();

    metricAnalyzer.AnalyzeClasses(files.ToArray());

    outcomes.AddRange(metricAnalyzer.GetOutcomes());

}

foreach (var outcome in outcomes)
{
    Console.WriteLine(outcome);
}



