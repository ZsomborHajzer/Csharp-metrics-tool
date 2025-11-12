using System;

public static class StringFilters
{
    public static readonly Func<string, bool> NotEmpty = line => !string.IsNullOrWhiteSpace(line);

    public static readonly Func<string, bool> LongerThan3 = line => line.Length > 3;

    public static readonly Func<string, bool> RemoveImports = line => !line.StartsWith("using");

    public static List<Func<string, bool>> GetFilters(params Func<string, bool>[] selected)
    {
        return new List<Func<string, bool>>(selected);
    }
}
