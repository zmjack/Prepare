using DotNetCli;
using NStandard;
using NStandard.Runtime;
using System;
using System.IO;

namespace Prepare.Cli;

[Command("All", Abbreviation = "all", Description = "Execute design-time tasks from the current project.")]
public class AllCommand : Command
{
    public AllCommand(CmdContainer container, string[] args) : base(container, args) { }

    [CmdProperty("config", Abbreviation = "c", Description = "Specify configuration.")]
    public string Config { get; set; } = "Debug";

    [CmdProperty("verbose", Abbreviation = "verb", Description = "Output more information for building.")]
    public bool Verbose { get; set; } = false;

    public override void Run()
    {
        var projectInfo = Container.Project ?? throw new InvalidOperationException("No project information.");
        var targetBinFolder = Path.GetFullPath($"{projectInfo.ProjectRoot}/bin/{Config}/{projectInfo.TargetFramework}");
        var targetAssemblyName = projectInfo.AssemblyName;
        var assemblyContext = new AssemblyContext(DotNetFramework.Parse(projectInfo.TargetFramework), projectInfo.Sdk);
        assemblyContext.LoadMain($"{targetBinFolder}/{targetAssemblyName}.dll");

        var designTimePrepareFactoryInterface = assemblyContext.GetType($"{nameof(Prepare)}.{nameof(IDesignTimePrepareFactory)},{nameof(Prepare)}");
        var prepareProjectType = assemblyContext.GetType($"{nameof(Prepare)}.{nameof(PrepareProject)},{nameof(Prepare)}");
        var factoryTypes = assemblyContext.MainAssembly.GetTypesWhichImplements(designTimePrepareFactoryInterface);

        var prepareProject = prepareProjectType.CreateInstance();
        foreach (var prop in prepareProjectType.GetProperties())
        {
            switch (prop.Name)
            {
                case "ProjectRoot": prop.SetValue(prepareProject, projectInfo.ProjectRoot); break;
                case "ProjectName": prop.SetValue(prepareProject, projectInfo.ProjectName); break;
                case "AssemblyName": prop.SetValue(prepareProject, projectInfo.AssemblyName); break;
                case "RootNamespace": prop.SetValue(prepareProject, projectInfo.RootNamespace); break;
                case "TargetFramework": prop.SetValue(prepareProject, projectInfo.TargetFramework); break;
                case "CliPackagePath": prop.SetValue(prepareProject, projectInfo.CliPackagePath); break;
                case "Sdk": prop.SetValue(prepareProject, projectInfo.Sdk); break;
            }
        }

        foreach (var factoryType in factoryTypes)
        {
            var method = factoryType.GetMethod(nameof(IDesignTimePrepareFactory.Prepare));
            var factory = factoryType.CreateInstance();
            method.Invoke(factory, [prepareProject, Arguments.OriginArgs]);
        }
    }
}
