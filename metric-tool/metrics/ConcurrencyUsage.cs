using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

class ConcurrencyUsage : IMetric
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
            // Bad, needs to be corrected
            total += doc.SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Count(n => n.ToString().StartsWith("Task.Run") ||
            n.ToString().StartsWith("Task.Factory.StartNew") ||
            n.ToString().StartsWith("Parallel") ||
            n.ToString().StartsWith("new Thread") ||
            n.ToString().StartsWith("ThreadPool.QueueUserWorkItem"));
        }
        return $"Starting concurrency statements = {total}";
    }
}
