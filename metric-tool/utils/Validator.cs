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