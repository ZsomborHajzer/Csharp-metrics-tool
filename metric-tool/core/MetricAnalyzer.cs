using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public void AnalyzeClasses(string[] files)
    {
        var allClasses = new Dictionary<string, ClassInfo>();

        foreach (var file in files)
        {
            var code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var cls in classes)
            {
                var className = cls.Identifier.Text;
                var info = new ClassInfo { Name = className };

                if (cls.BaseList != null)
                {
                    foreach (var baseType in cls.BaseList.Types)
                    {
                        var typeName = baseType.Type.ToString();
                        if (!typeName.StartsWith("I"))
                        {
                            info.BaseClass = typeName;
                        }
                    }
                }

                var fields = cls.DescendantNodes().OfType<FieldDeclarationSyntax>();
                foreach (var field in fields)
                {
                    var typeName = field.Declaration.Type.ToString();
                    AddDependency(info, typeName);
                }

                var properties = cls.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                foreach (var prop in properties)
                {
                    var typeName = prop.Type.ToString();
                    AddDependency(info, typeName);
                }

                var methods = cls.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var method in methods)
                {
                    var returnType = method.ReturnType.ToString();
                    AddDependency(info, returnType);

                    foreach (var param in method.ParameterList.Parameters)
                    {
                        var paramType = param.Type?.ToString();
                        if (paramType != null)
                            AddDependency(info, paramType);
                    }
                }

                allClasses[className] = info;
            }
        }

        var codebaseClasses = new HashSet<string>(allClasses.Keys);
        foreach (var cls in allClasses.Values)
        {
            cls.Dependencies.RemoveWhere(dep => !codebaseClasses.Contains(dep));

            if (cls.BaseClass != null && !codebaseClasses.Contains(cls.BaseClass))
            {
                cls.BaseClass = null;
            }
        }

        List<string> outcomes = new List<string>();

        int totalClasses = allClasses.Count;
        outcomes.Add($"Total Classes: {totalClasses}");

        double avgDependencies = allClasses.Count > 0
            ? allClasses.Values.Average(c => c.Dependencies.Count)
            : 0;
        outcomes.Add($"Average Dependencies per Class: {avgDependencies:F2}");

        int maxDependencies = allClasses.Count > 0
            ? allClasses.Values.Max(c => c.Dependencies.Count)
            : 0;
        outcomes.Add($"Maximum Dependencies: {maxDependencies}");

        int minDependencies = allClasses.Count > 0
            ? allClasses.Values.Min(c => c.Dependencies.Count)
            : 0;
        outcomes.Add($"Minimum Dependencies: {minDependencies}");

        int classesWithInheritance = allClasses.Values.Count(c => c.BaseClass != null);
        outcomes.Add($"Classes with Inheritance: {classesWithInheritance}");

        int classesWithNoDependencies = allClasses.Values.Count(c => c.Dependencies.Count == 0);
        outcomes.Add($"Classes with No Dependencies: {classesWithNoDependencies}");

        int highCouplingClasses = allClasses.Values.Count(c => c.Dependencies.Count > 5);
        outcomes.Add($"Classes with High Coupling (>5 dependencies): {highCouplingClasses}");

        foreach (var outcome in outcomes)
        {
            Console.WriteLine(outcome);
        }
    }

    private void AddDependency(ClassInfo info, string typeName)
    {
        if (typeName.Contains("<"))
        {
            var genericType = typeName.Substring(typeName.IndexOf("<") + 1);
            genericType = genericType.TrimEnd('>');
            typeName = genericType.Split(',')[0].Trim();
        }

        var primitives = new[] { "void", "int", "string", "bool", "double", "float", "decimal", "long", "byte", "char", "object", "var" };
        if (!primitives.Contains(typeName.ToLower()) && !typeName.StartsWith("System.") && !string.IsNullOrWhiteSpace(typeName))
        {
            info.Dependencies.Add(typeName);
        }
    }
}