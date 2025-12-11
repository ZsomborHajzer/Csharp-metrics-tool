using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AsyncAwaitCounter : IMetric
{
    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        int total = 0;
        foreach (var doc in docs)
        {
            total += doc.SyntaxTree.GetRoot().DescendantNodes().OfType<AwaitExpressionSyntax>().Count();
        }
        return $"Async/Awaits = {total}";
    }
}

