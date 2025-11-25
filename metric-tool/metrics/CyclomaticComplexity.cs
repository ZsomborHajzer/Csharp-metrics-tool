using System.Reflection.Metadata;
using System.Text.RegularExpressions;

//Definitely not perfect yet, only rough approximation
//Actually really bad, don't use, found possibly better method will discuss in meeting
public class CyclomaticComplexity : IMetric
{

    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        int decisionPointsCount = 1;

        foreach (var doc in docs)
        {
            foreach (var line in doc.DocumentLines)
            {
                string[] subs = Regex.Split(line, @"[{}\(\)/\s]+");
                var keywordCount = subs.Count(x => x == "if" || x == "else" || x == "for" || x == "while" || x == "foreach");
                var operatorCount = Regex.Matches(line, @"&&|\|\||(\?<![a-zA-Z0-9_])|:").Count;
                decisionPointsCount += keywordCount + operatorCount;
            }
        }

         return $"CC = {decisionPointsCount}";
    }
}

