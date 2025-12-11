using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Density as Keyowrds/LOC? WIP
class KeywordDensity : IMetric
{
    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        float complexityKeywords = 0;
        float LOC = 0; //temporary, should be replaced with something else later
        foreach (var doc in docs)
        {
            complexityKeywords += doc.SyntaxTree.GetRoot().DescendantNodes().Count(n => n.IsKind(SyntaxKind.IfStatement) ||
                n.IsKind(SyntaxKind.ForStatement) ||
                n.IsKind(SyntaxKind.WhileStatement) ||
                n.IsKind(SyntaxKind.ForEachStatement) ||
                n.IsKind(SyntaxKind.SwitchStatement) ||
                n.IsKind(SyntaxKind.ConditionalExpression));

            LOC += doc.DocumentLines.Count();
        }

        return $"Complexity keyword density = {complexityKeywords / LOC}";
    }
}