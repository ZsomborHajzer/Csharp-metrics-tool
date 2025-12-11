using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ParameterPerFunctionViolation : IMetric
{
    public string Evaluate(List<Document> docs)
    {
        return Implementation(docs, 4);
    }

    private string Implementation(List<Document> docs, int violationThreshold)
    {
        int violations = 0;
        foreach(var doc in docs)
        {
            var MethodDeclarationNodes = doc.SyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach(var method in MethodDeclarationNodes)
            {
                int methodCount = method.ParameterList.Parameters.Count();
                if (methodCount > violationThreshold)
                {
                    violations += methodCount - violationThreshold;
                }
            }
        }
        return $"All parameter violations = {violations}";
    }
}
