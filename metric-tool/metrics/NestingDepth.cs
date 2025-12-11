using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class NestingDepth : IMetric
{
    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        List<int> depths = new List<int>();
        foreach(var doc in docs)
        {
            var root = doc.SyntaxTree.GetRoot();
            depths.Add(MaxNestingDepth(root, 0));
        }
        return $"Max nesting depth {depths.Max()} \nAverage nesting depth {depths.Average():0.##}";
    }

    int MaxNestingDepth(SyntaxNode node, int currentDepth)
    {
        int depth = currentDepth;

        // If this node counts as a nesting construct, go one level deeper
        if (node is IfStatementSyntax ||
            node is ForStatementSyntax ||
            node is WhileStatementSyntax ||
            node is SwitchStatementSyntax ||
            node is TryStatementSyntax ||
            node is BlockSyntax)
        {
            currentDepth++;
        }

        foreach (var child in node.ChildNodes())
        {
            int maxChildDepth = MaxNestingDepth(child, currentDepth);
            if (maxChildDepth > depth)
            {
                depth = maxChildDepth;
            }
        }

        return depth;
    }
}