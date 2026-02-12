namespace Aera.Commands
{
    internal class StatCommand : ICommand
    {
        public string Name => "stat";
        public string Description => "Display file or directory metadata";
        public string Usage => "Usage: stat <path>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored(Usage, "Red");
                return;
            }

            string path = args[0];

            if (File.Exists(path))
            {
                PrintFile(new FileInfo(path), tool);
                return;
            }

            if (Directory.Exists(path))
            {
                PrintDirectory(new DirectoryInfo(path), tool);
                return;
            }

            tool.WriteLineColored("Path not found.", "Red");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            Execute(new[] { input.Trim() }, tool);
        }

        private void PrintFile(FileInfo file, ShellContext tool)
        {
            tool.WriteLineColored("File Information", "DarkCyan");
            tool.WriteLine($"Name: {file.Name}");
            tool.WriteLine($"Full Path: {file.FullName}");
            tool.WriteLine($"Size: {file.Length} bytes");
            tool.WriteLine($"Created: {file.CreationTime}");
            tool.WriteLine($"Modified: {file.LastWriteTime}");
            tool.WriteLine($"Attributes: {file.Attributes}");
        }

        private void PrintDirectory(DirectoryInfo dir, ShellContext tool)
        {
            tool.WriteLineColored("Directory Information", "DarkCyan");
            tool.WriteLine($"Name: {dir.Name}");
            tool.WriteLine($"Full Path: {dir.FullName}");
            tool.WriteLine($"Created: {dir.CreationTime}");
            tool.WriteLine($"Modified: {dir.LastWriteTime}");
            tool.WriteLine($"Attributes: {dir.Attributes}");
        }
    }
}
