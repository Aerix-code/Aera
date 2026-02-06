using System;
using System.IO;

namespace Aera
{
    internal class MkdirCommand : ICommand
    {
        public string Name => "mkdir";
        public string Description => "Creates a directory";
        public string Usage => "Usage: mkdir <directory>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 1)
            {
                tool.WriteLine("Usage: mkdir <directory>");
                return;
            }

            string path = Path.GetFullPath(args[0]);

            if (Directory.Exists(path))
            {
                tool.WriteLineColored("mkdir: directory already exists", "Yellow");
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
                tool.WriteLineColored("Directory created.", "Green");
            }
            catch (UnauthorizedAccessException)
            {
                tool.WriteLineColored("mkdir: permission denied", "Red");
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"mkdir: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("mkdir: cannot be used in a pipe", "Red");
        }
    }
}
