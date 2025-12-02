public class HalsteadEffort : IMetric
{
    private double _effort;
    private readonly Func<List<Document>, string> _func;
    
    public HalsteadEffort()
    {
        _func = Implementation;
    }
    
    public string Evaluate(List<Document> docs)
    {
        return _func(docs);
    }
    
    private string Implementation(List<Document> docs)
    {
        _effort = 0;
        
       
        HashSet<string> uniqueOperators = new HashSet<string>();
        HashSet<string> uniqueOperands = new HashSet<string>();
        int totalOperators = 0;
        int totalOperands = 0;
        
        
        string[] operators = new[] {
            "=", "+", "-", "*", "/", "%", "++", "--", "==", "!=", ">", "<", ">=", "<=",
            "&&", "||", "!", "&", "|", "^", "~", "<<", ">>", "+=", "-=", "*=", "/=", "%=",
            "&=", "|=", "^=", "<<=", ">>=", "??", "?", ":", "=>", ".", "->", "(", ")", "[", "]",
            "{", "}", ";", ",", "new", "if", "else", "while", "for", "foreach", "do", "switch",
            "case", "break", "continue", "return", "throw", "try", "catch", "finally", "using",
            "class", "struct", "interface", "enum", "public", "private", "protected", "internal",
            "static", "void", "int", "string", "bool", "double", "float", "var", "async", "await"
        };
        
        foreach (var doc in docs)
        {
            foreach (var line in doc.DocumentLines)
            {
                string trimmedLine = line.Trim();
            

                if (string.IsNullOrWhiteSpace(trimmedLine) || 
                    trimmedLine.StartsWith("//") || 
                    trimmedLine.StartsWith("/*") ||
                    trimmedLine.StartsWith("*"))
                    continue;
                
                foreach (var op in operators)
                {
                    int count = CountOccurrences(trimmedLine, op);
                    if (count > 0)
                    {
                        uniqueOperators.Add(op);
                        totalOperators += count;
                    }
                }
                
        
                var words = System.Text.RegularExpressions.Regex.Split(trimmedLine, @"\W+");
                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word) && !operators.Contains(word))
                    {
                    
                        if (char.IsLetterOrDigit(word[0]) || word[0] == '_')
                        {
                            uniqueOperands.Add(word);
                            totalOperands++;
                        }
                    }
                }
            }
        }
        
     
        int n1 = uniqueOperators.Count;  
        int n2 = uniqueOperands.Count;   
        int N1 = totalOperators;         
        int N2 = totalOperands;          
        
        if (n1 == 0 || n2 == 0)
        {
            _effort = 0;
            return "Halstead Effort: 0.00 (insufficient data)";
        }
        
        
        double vocabulary = n1 + n2;                                    
        double length = N1 + N2;                                       
        double calculatedLength = n1 * Math.Log(n1, 2) + n2 * Math.Log(n2, 2); 
        double volume = length * Math.Log(vocabulary, 2);               
        double difficulty = (n1 / 2.0) * (N2 / (double)n2);            
        _effort = volume * difficulty;                                 
        
        return $"Halstead Effort: {_effort:F2}";
    }
    
    private int CountOccurrences(string text, string pattern)
    {
        int count = 0;
        int index = 0;
        
        while ((index = text.IndexOf(pattern, index)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        
        return count;
    }
}