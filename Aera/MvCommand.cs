using System;
using System.IO;

namespace Aera
{
    internal class MvCommand : ICommand
    {
        public string Name => "mv";
        public string Description => "Moves or renames files and directories";
        public string Usage => "Usage: mv <source> <destination>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (!tool.IsSudo)
            {
                tool.WriteLineColored("mv: permission denied (sudo required)", "Red");
                return;
            }

            if (args.Length != 2)
            {
                tool.WriteLine("Usage: mv <source> <destination>");
                return;
            }

            string source = args[0];
            string dest = args[1];

            try
            {
                if (File.Exists(source))
                {
                    MoveFile(source, dest);
                    return;
                }

                if (Directory.Exists(source))
                {
                    MoveDirectory(source, dest);
                    return;
                }

                tool.WriteLine($"mv: cannot stat '{source}': No such file or directory");
            }
            catch (UnauthorizedAccessException)
            {
                tool.WriteLineColored("mv: permission denied", "Red");
            }
            catch (IOException ex)
            {
                tool.WriteLineColored($"mv: {ex.Message}", "Red");
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"mv: {ex.Message}", "Red");
            }
        }

        private void MoveFile(string source, string dest)
        {
            if (Directory.Exists(dest))
                dest = Path.Combine(dest, Path.GetFileName(source));

            if (File.Exists(dest))
                File.Delete(dest); // overwrite (force)

            File.Move(source, dest);
        }

        private void MoveDirectory(string source, string dest)
        {
            if (Directory.Exists(dest))
                dest = Path.Combine(dest, Path.GetFileName(source));

            Directory.Move(source, dest);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("mv: cannot be used in a pipe", "Red");
        }
    }
}
