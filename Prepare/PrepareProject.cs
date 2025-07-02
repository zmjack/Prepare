namespace Prepare;

public struct PrepareProject
{
    public string ProjectRoot { get; set; }

    public string ProjectName { get; set; }

    public string AssemblyName { get; set; }

    public string RootNamespace { get; set; }

    public string TargetFramework { get; set; }

    public string CliPackagePath { get; set; }

    public string Sdk { get; set; }
}
