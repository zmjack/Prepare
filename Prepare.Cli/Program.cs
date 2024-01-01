using DotNetCli;
using NStandard;
using System;
using System.IO;
using System.Reflection;

namespace Prepare.Cli
{
    public class Program
    {
        public static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();
        public static readonly string CLI_VERSION = ThisAssembly.GetName().Version.ToString();
        public static readonly CmdContainer CmdContainer = new("prepare", ThisAssembly);

        static void Main(string[] args)
        {
            CmdContainer.Project = Project.GetFromDirectory(Directory.GetCurrentDirectory());

            PrintWelcome();
            CmdContainer.PrintProjectInfo();
            CmdContainer.Run(args);
        }

        public static void PrintWelcome()
        {
            Console.WriteLine($@"
{"ヽ(*^▽^)ノ".Center(60)}

Prepare .NET Command-line Tools {CLI_VERSION}");
        }

    }
}
