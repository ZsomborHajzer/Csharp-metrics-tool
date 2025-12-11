using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

//Redone, now with Syntax Tree :D
public class CyclomaticComplexity : IMetric
{

    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs);
    }

    private string Implementation(List<Document> docs)
    {
        int SumCC = 0;
        int MaxCC = 0;
        string MaxDoc = "non existent";
        int AvgCC = 0;
        foreach (var doc in docs)
        {
            var root = doc.SyntaxTree.GetRoot();
            var cc =
            1
            + root.DescendantNodes().OfType<IfStatementSyntax>().Count()
            + root.DescendantNodes().OfType<ForStatementSyntax>().Count()
            + root.DescendantNodes().OfType<ForEachStatementSyntax>().Count()
            + root.DescendantNodes().OfType<WhileStatementSyntax>().Count()
            + root.DescendantNodes().OfType<DoStatementSyntax>().Count()
            + root.DescendantNodes().OfType<SwitchSectionSyntax>().Sum(sec => sec.Labels.Count - 1)
            + root.DescendantNodes().OfType<BinaryExpressionSyntax>().Count(n => n.Kind() == SyntaxKind.LogicalAndExpression || n.Kind() == SyntaxKind.LogicalOrExpression)
            + root.DescendantNodes().OfType<ConditionalExpressionSyntax>().Count()
            + root.DescendantNodes().OfType<CatchClauseSyntax>().Count()
            + root.DescendantNodes().OfType<SwitchExpressionArmSyntax>().Count();
            SumCC += cc;
            if (cc > MaxCC)
            {
                MaxCC = cc;
                MaxDoc = doc.filePath;
            }
        }
        AvgCC = SumCC / docs.Count();

        return $"Sum of all cyclomatic complexity = {SumCC} \nDocument with highest cyclomatic complexity is {MaxDoc} = {MaxCC} \nAverage cyclomatic complexity = {AvgCC}";
    }
}

