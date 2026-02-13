namespace Aera.Commands
{
    internal class CatCommand : ICommand
    {
        public string Name => "cat";
        public string Description => "Prints file contents";
        public string Usage => "Usage: cat <file>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 1)
            {
                tool.WriteLine("Usage: cat <file>");
                return;
            }

            var path = Path.GetFullPath(args[0]);

            if (!File.Exists(path))
            {
                tool.WriteLineColored("cat: file not found", "Red");
                return;
            }

            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
            {
                tool.WriteLineColored("cat: cannot read a directory", "Red");
                return;
            }

            foreach (var line in File.ReadLines(path))
                tool.WriteLine(line);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            if (args.Length != 0)
            {
                tool.WriteLineColored("cat: cannot combine piped input with files", "Red");
                return;
            }

            if (!string.IsNullOrWhiteSpace(input))
                tool.WriteLine(input.TrimEnd());
        }
    }
}