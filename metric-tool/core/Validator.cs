using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Validator
{
    private string projectPath { get; set; }

    public Validator(string projectPath)
    {
        this.projectPath = projectPath;
    }

    // Checks if the project path is valid and contains C# files
    private bool ValidatePath()
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            return false;
        }
        if (!Directory.Exists(projectPath))
        {
            return false;
        }
        var csFiles = Directory.EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        if (!csFiles.Any())
        {
            return false;
        }
        return true;
    }

    // Returns all C# files in the project path
    public List<string> GetCsharpFiles()
    {
        if (!ValidatePath())
        {
            throw new ArgumentException("The project path submitted was not valid");
        }
        var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories).ToList();
        return csFiles;
    }
}