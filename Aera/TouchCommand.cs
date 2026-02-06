using System;
using System.IO;
using System.Linq;

namespace Aera
{
    internal class TouchCommand : ICommand
    {
        public string Name => "touch";
        public string Description => "Creates files or updates timestamps";
        public string Usage => "Usage: touch <file>";

        public bool AcceptsPipeInput => false;   // touch should not read stdin
        public bool IsDestructive => false;       // creating files is allowed

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0 || args.Contains("--help"))
            {
                ShowHelp(tool);
                return;
            }

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                    continue;

                string path = Path.GetFullPath(arg);

                try
                {
                    if (File.Exists(path))
                    {
                        File.SetLastWriteTime(path, DateTime.Now);
                        tool.WriteLineColored($"Updated: {Path.GetFileName(path)}", "Yellow");
                    }
                    else
                    {
                        using (File.Create(path)) { }
                        tool.WriteLineColored($"Created: {Path.GetFileName(path)}", "Green");
                    }
                }
                catch (Exception ex)
                {
                    tool.WriteLineColored($"touch: {ex.Message}", "Red");
                }
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("touch: does not accept piped input");
        }

        private void ShowHelp(ShellContext tool)
        {
            tool.WriteLine("Usage: touch <file> [file...]");
            tool.WriteLine("");
            tool.WriteLine("Creates empty files or updates modification times.");
        }
    }
}
