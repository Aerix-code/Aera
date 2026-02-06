using System;
using System.IO;

namespace Aera
{
    internal class RmCommand : ICommand
    {
        public string Name => "rm";
        public string Description => "Removes files or directories";
        public string Usage => "Usage: rm [-r] <target>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (!tool.IsSudo)
            {
                tool.WriteLineColored("rm: permission denied (sudo required)", "Red");
                return;
            }

            bool recursive = false;
            int index = 0;

            if (args.Length > 0 && args[0] == "-r")
            {
                recursive = true;
                index = 1;
            }

            if (args.Length - index != 1)
            {
                tool.WriteLine("Usage: rm [-r] <target>");
                return;
            }

            string target = args[index];

            bool isFile = File.Exists(target);
            bool isDir = Directory.Exists(target);

            if (!isFile && !isDir)
            {
                tool.WriteLineColored($"rm: cannot remove '{target}': No such file or directory", "Red");
                return;
            }

            if (isDir && !recursive)
            {
                tool.WriteLineColored($"rm: cannot remove '{target}': Is a directory", "Red");
                return;
            }

            string what = isDir ? "directory" : "file";
            string msg = $"Remove {what} '{target}' permanently?";

            if (!tool.Confirm(msg, defaultYes: false))
            {
                tool.WriteLineColored("rm: operation cancelled", "Yellow");
                return;
            }

            try
            {
                if (isFile)
                {
                    File.Delete(target);
                    tool.WriteLineColored("File deleted", "green");
                }
                else
                {
                    Directory.Delete(target, recursive: true);
                    tool.WriteLineColored("Directory deleted", "green");
                }
            }
            catch (UnauthorizedAccessException)
            {
                tool.WriteLineColored("rm: permission denied", "Red");
            }
            catch (IOException ex)
            {
                tool.WriteLineColored($"rm: {ex.Message}", "Red");
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"rm: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("rm: refusing to read from pipe (destructive command)", "Red");
        }
    }
}
