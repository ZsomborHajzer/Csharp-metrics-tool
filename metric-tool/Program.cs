// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World! \n");

Console.WriteLine("Please submit the absolute path of your project: ");

var projectPath = Console.ReadLine();

if (projectPath == null || projectPath.Length == 0)
{
    throw new Exception("Invalid project Path");
}

Validator validator = new Validator(projectPath);

var files = new List<string>();

try
{
    files = validator.GetCsharpFiles();
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}


if (!(files.Count == 0))
{
    foreach (var file in files)
    {
        Console.WriteLine($"{file}");
    }
}



