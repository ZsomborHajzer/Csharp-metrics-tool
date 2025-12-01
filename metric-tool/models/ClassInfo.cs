class ClassInfo
{
    public string Name { get; set; }
    public string BaseClass { get; set; }
    public HashSet<string> Dependencies { get; set; } = new HashSet<string>();
}