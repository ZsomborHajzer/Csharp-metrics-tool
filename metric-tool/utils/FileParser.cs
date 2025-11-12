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

    public List<Document> ParseFiles()
    {
        List<Document> documents = new List<Document>();

        foreach (string filePath in FilePaths)
        {
            documents.Add(CreateDocument(filePath));
        }

        return documents;
    }

    public Document CreateDocument(string filePath)
    {
        CurrentFile = filePath;

        List<string> lines = File.ReadLines(filePath).ToList();

        lines = ApplyFilters(lines);

        if (lines.Count == 0)
        {
            throw new ArgumentException($"The file '{filePath}' had no code in it.");
        }

        return new Document(lines, filePath);
    }

    private List<string> FilterLines(List<string> lines)
    {
        foreach (var filter in Filters)
        {
            lines = lines.Where(filter).ToList();
        }
        return lines;
    }

    private List<string> ApplyFilters(List<string> lines)
    {
        return (Filters != null && Filters.Count > 0)
            ? FilterLines(lines)
            : lines;
    }
}
