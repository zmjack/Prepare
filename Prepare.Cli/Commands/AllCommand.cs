using DotNetCli;
using NStandard;
using NStandard.Runtime;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prepare.Cli
{
    [Command("All", Abbreviation = "all", Description = "Execute design-time tasks from the current project.")]
    public class AllCommand : Command
    {
        public AllCommand(CmdContainer container, string[] args) : base(container, args) { }

        [CmdProperty("verbose", Abbreviation = "verb", Description = "Output more information for building.")]
        public bool Verbose { get; set; } = false;

        public override void Run()
        {
            var projectInfo = Container.ProjectInfo;

            if (projectInfo is null) throw new InvalidOperationException("No project information.");

            var project = projectInfo.Value;
            var targetBinFolder = Path.GetFullPath($"{project.ProjectRoot}/bin/Debug/{project.TargetFramework}");
            var targetAssemblyName = project.AssemblyName;
            var assemblyContext = new AssemblyContext(DotNetFramework.Parse(project.TargetFramework), project.Sdk);
            assemblyContext.LoadMain($"{targetBinFolder}/{targetAssemblyName}.dll");

            var designTimePrepareFactoryInterface = assemblyContext.GetType($"{nameof(Prepare)}.{nameof(IDesignTimePrepareFactory)},{nameof(Prepare)}");
            var prepareProjectType = assemblyContext.GetType($"{nameof(Prepare)}.{nameof(PrepareProject)},{nameof(Prepare)}");
            var factoryTypes = assemblyContext.MainAssembly.GetTypesWhichImplements(designTimePrepareFactoryInterface);

            var prepareProject = prepareProjectType.CreateInstance();
            foreach (var prop in prepareProjectType.GetProperties())
            {
                switch (prop.Name)
                {
                    case "ProjectRoot": prop.SetValue(prepareProject, projectInfo.Value.ProjectRoot); break;
                    case "ProjectName": prop.SetValue(prepareProject, projectInfo.Value.ProjectName); break;
                    case "AssemblyName": prop.SetValue(prepareProject, projectInfo.Value.AssemblyName); break;
                    case "RootNamespace": prop.SetValue(prepareProject, projectInfo.Value.RootNamespace); break;
                    case "TargetFramework": prop.SetValue(prepareProject, projectInfo.Value.TargetFramework); break;
                    case "CliPackagePath": prop.SetValue(prepareProject, projectInfo.Value.CliPackagePath); break;
                    case "Sdk": prop.SetValue(prepareProject, projectInfo.Value.Sdk); break;
                }
            }

            foreach (var factoryType in factoryTypes)
            {
                var method = factoryType.GetMethod(nameof(IDesignTimePrepareFactory.Prepare));
                var factory = factoryType.CreateInstance();
                method.Invoke(factory, new object[] { prepareProject, Arguments.OriginArgs });
            }
        }
    }
}
