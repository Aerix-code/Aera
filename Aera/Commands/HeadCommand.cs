namespace Aera
{
    internal class HeadCommand : ICommand
    {
        public string Name => "head";
        public string Description => "Shows first lines of a file";
        public string Usage => "Usage: head <file> [lineCount]";

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

            string file = args[0];
            int count = args.Length > 1 && int.TryParse(args[1], out var c) ? c : 10;

            if (!File.Exists(file))
            {
                tool.WriteLineColored("File not found.", "Red");
                return;
            }

            foreach (var line in File.ReadLines(file).Take(count))
                tool.WriteLine(line);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            int count = args.Length > 0 && int.TryParse(args[0], out var c) ? c : 10;

            var lines = input.Split(Environment.NewLine)
                .Take(count);

            foreach (var line in lines)
                tool.WriteLine(line);
        }
    }
}