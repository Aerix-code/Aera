namespace Aera.Commands
{
    internal class CdCommand : ICommand
    {
        public string Name => "cd";
        public string Description => "Changes the current directory";
        public string Usage => "Usage: cd <directory>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 1)
            {
                tool.WriteLine("Usage: cd <directory>");
                return;
            }

            try
            {
                Directory.SetCurrentDirectory(args[0]);
                // intentionally silent on success
            }
            catch (DirectoryNotFoundException)
            {
                tool.WriteLineColored("cd: no such directory", "Red");
            }
            catch (UnauthorizedAccessException)
            {
                tool.WriteLineColored("cd: permission denied", "Red");
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"cd: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("cd: cannot be used in a pipe", "Red");
        }
    }
}
