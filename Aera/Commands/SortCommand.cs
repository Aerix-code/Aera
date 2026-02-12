namespace Aera
{
    internal class SortCommand : ICommand
    {
        public string Name => "sort";
        public string Description => "Sorts lines alphabetically";
        public string Usage => "Usage: sort [file]";

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

            if (!File.Exists(args[0]))
            {
                tool.WriteLineColored("File not found.", "Red");
                return;
            }

            foreach (var line in File.ReadLines(args[0]).OrderBy(x => x))
                tool.WriteLine(line);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            foreach (var line in input.Split(Environment.NewLine).OrderBy(x => x))
                tool.WriteLine(line);
        }
    }
}