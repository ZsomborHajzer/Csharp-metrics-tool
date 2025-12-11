using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileParser
{
    public string? CurrentFile { get; private set; }
    public List<string> FilePaths { get; }
    public List<Func<string, bool>> Filters { get; }

    public FileParser(List<string> filePaths, List<Func<string, bool>> filters)
    {
        FilePaths = filePaths ?? throw new ArgumentNullException(nameof(filePaths));
        Filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }

    // Parses all files and returns a list of documents
    public List<Document> ParseFiles()
    {
        List<Document> documents = new List<Document>();
        foreach (string filePath in FilePaths)
        {
            documents.Add(CreateDocument(filePath));
        }
        return documents;
    }

    // Creates a document from a file after applying filters
    public Document CreateDocument(string filePath)
    {
        CurrentFile = filePath;
        List<string> lines = File.ReadLines(filePath).ToList();
        var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
        lines = ApplyFilters(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException($"The file '{filePath}' had no code in it.");
        }
            return new Document(lines, tree, filePath);
    }

    // Applies each filter function to the lines
    private List<string> FilterLines(List<string> lines)
    {
        foreach (var filter in Filters)
        {
            lines = lines.Where(filter).ToList();
        }
        return lines;
    }

    // Applies filters if any exist, otherwise returns original lines
    private List<string> ApplyFilters(List<string> lines)
    {
        return (Filters != null && Filters.Count > 0)
            ? FilterLines(lines)
            : lines;
    }
}