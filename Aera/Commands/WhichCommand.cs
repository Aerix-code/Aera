namespace Aera.Commands
{
    internal class WhichCommand : ICommand
    {
        public string Name => "which";
        public string Description => "Locate executable in PATH";
        public string Usage => "Usage: which <command>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored(Usage, "Red");
                return;
            }

            string cmd = args[0];
            var paths = (Environment.GetEnvironmentVariable("PATH") ?? "")
                .Split(Path.PathSeparator);

            string[] extensions = OperatingSystem.IsWindows()
                ? new[] { ".exe", ".bat", ".cmd", "" }
                : new[] { "" };

            foreach (var p in paths)
            {
                foreach (var ext in extensions)
                {
                    var full = Path.Combine(p, cmd + ext);
                    if (File.Exists(full))
                    {
                        tool.WriteLine(full);
                        return;
                    }
                }
            }

            tool.WriteLineColored("Command not found.", "Red");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            Execute(new[] { input.Trim() }, tool);
        }
    }
}