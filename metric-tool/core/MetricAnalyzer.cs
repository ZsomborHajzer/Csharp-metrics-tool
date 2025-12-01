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

    // Iterate through every metric assigned in the metrics list and add their outcome to the outcomes list and later return them
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
                
                // Get base class
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
                
                // Get field and property dependencies
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
                
                // Get method parameter and return type dependencies
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
        
        // Filter dependencies to only include classes from the codebase
        var codebaseClasses = new HashSet<string>(allClasses.Keys);
        
        foreach (var cls in allClasses.Values)
        {
            // Filter dependencies
            cls.Dependencies.RemoveWhere(dep => !codebaseClasses.Contains(dep));
            
            // Filter base class
            if (cls.BaseClass != null && !codebaseClasses.Contains(cls.BaseClass))
            {
                cls.BaseClass = null;
            }
        }
        
        // Output results
        Console.WriteLine("=== CLASS ANALYSIS ===\n");
        
        foreach (var kvp in allClasses.OrderBy(x => x.Key))
        {
            var cls = kvp.Value;
            Console.WriteLine($"Class: {cls.Name}");
            
            if (cls.BaseClass != null)
                Console.WriteLine($"  Inherits from: {cls.BaseClass}");
            
            Console.WriteLine($"  Dependencies: {cls.Dependencies.Count}");
            
            if (cls.Dependencies.Any())
            {
                Console.WriteLine("  Uses:");
                foreach (var dep in cls.Dependencies.OrderBy(x => x))
                {
                    Console.WriteLine($"    - {dep}");
                }
            }
            Console.WriteLine();
        }
        
        //Output associations
        // Console.WriteLine("\n=== ASSOCIATIONS ===\n");
        // foreach (var kvp in allClasses.OrderBy(x => x.Key))
        // {
        //     var cls = kvp.Value;
            
        //     if (cls.BaseClass != null)
        //         Console.WriteLine($"{cls.Name} --inherits--> {cls.BaseClass}");
            
        //     foreach (var dep in cls.Dependencies.OrderBy(x => x))
        //     {
        //         Console.WriteLine($"{cls.Name} --uses--> {dep}");
        //     }
        // }
    }

    private void AddDependency(ClassInfo info, string typeName)
    {

        if (typeName.Contains("<"))
        {
            var genericType = typeName.Substring(typeName.IndexOf("<") + 1);
            genericType = genericType.TrimEnd('>');
            typeName = genericType.Split(',')[0].Trim();
        }
        
        // Filter out primitive types and common system types
        var primitives = new[] { "void", "int", "string", "bool", "double", "float", 
                                "decimal", "long", "byte", "char", "object", "var" };
        
        if (!primitives.Contains(typeName.ToLower()) && 
            !typeName.StartsWith("System.") &&
            !string.IsNullOrWhiteSpace(typeName))
        {
            info.Dependencies.Add(typeName);
        }

    }
}
