namespace Aera.Commands
{
    internal class TailCommand : ICommand
    {
        public string Name => "tail";
        public string Description => "Shows last lines of a file";
        public string Usage => "Usage: tail <file> [lineCount]";

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

            var lines = File.ReadLines(file).TakeLast(count);

            foreach (var line in lines)
                tool.WriteLine(line);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            int count = args.Length > 0 && int.TryParse(args[0], out var c) ? c : 10;

            var lines = input.Split(Environment.NewLine)
                .TakeLast(count);

            foreach (var line in lines)
                tool.WriteLine(line);
        }
    }
}