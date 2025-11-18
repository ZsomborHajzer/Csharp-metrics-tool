public class MetricAnalyzer
{
    private List<Document> _documents { get; set; }
    private string _projectPath { get; set; }
    private List<IMetric> _metrics { get; set; } = new List<IMetric>();

    public MetricAnalyzer(List<Document> documents, string projectPath)
    {
        _documents = documents;
        _projectPath = projectPath;
    }

    // Iterate through every metric assigned in the metrics list and add their outcome to the outcomes list and later return them
    public void RunMetrics()
    {
        List<string> outcomes = new List<string>();

        foreach (var metric in _metrics)
        {
            outcomes.Add(metric.Evaluate(_documents));
        }

        foreach (var outcome in outcomes)
        {
            Console.WriteLine(outcome);
        }
    }

    public void AddMetric(IMetric metric)
    {
        _metrics.Add(metric);
    }

    public static void ResetMetrics()
    {
        return;
    }

    public void CollectClassData()
    {
        return;
    }
}
