using DotNetCli;

namespace Prepare.Cli.Test;

public class UnitTest1
{
    private static CmdContainer CmdContainer { get; } = new("prepare", Program.ThisAssembly, Project.GetFromDirectory(Path.Combine(Directory.GetCurrentDirectory(), "../../..")));

    [Fact]
    public void Test1()
    {
        CmdContainer.Run(["all", "-c", "Release"]);
    }

}
