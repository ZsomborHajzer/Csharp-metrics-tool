using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LambdaFunctionCounter : IMetric
{
    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        int total = 0;
        foreach(var doc in docs)
        {
            total += doc.SyntaxTree.GetRoot().DescendantNodes().Count(n =>
                n is SimpleLambdaExpressionSyntax ||
                n is ParenthesizedLambdaExpressionSyntax ||
                n is AnonymousMethodExpressionSyntax);

            total += doc.SyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Count(m => m.Modifiers.Any(mod => mod.IsKind(SyntaxKind.AsyncKeyword)));
        }
        return $"Lambdas = {total}";
    }
}
