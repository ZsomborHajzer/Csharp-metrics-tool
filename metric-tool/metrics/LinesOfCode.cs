public class LinesOfCode : IMetric
{
    private int _lineCount;
    private readonly Func<List<Document>, string> _func;

    public LinesOfCode()
    {
        _func = Implementation;
    }

    public string Evaluate(List<Document> docs)
    {
        return _func(docs);
    }

    private string Implementation(List<Document> docs)
    {
        _lineCount = 0;

        foreach (var doc in docs)
        {
            _lineCount += doc.DocumentLines.Count;
        }

         return $"Lines of Code (including comments): {_lineCount}";
    }
}

