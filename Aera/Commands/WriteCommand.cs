namespace Aera.Commands
{
    internal class WriteCommand : ICommand
    {
        public string Name => "write";
        public string Description => "Writes formatted output to the console";
        public string Usage => "Usage: write <type> <text>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length < 2)
            {
                tool.WriteLine("Usage: write <type> <text>");
                tool.WriteLine("Types: error, warning, success, text");
                return;
            }

            var type = args[0].ToLower();
            var text = string.Join(" ", args, 1, args.Length - 1);

            Write(type, text, tool);
        }

        // pipe support
        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            var type = args.Length > 0 ? args[0].ToLower() : "text";

            Write(type, input.TrimEnd(), tool);
        }

        private void Write(string type, string text, ShellContext tool)
        {
            switch (type)
            {
                case "error":
                    tool.WriteColored("ERROR: ", "Red");
                    tool.WriteLine(text);
                    break;

                case "warning":
                    tool.WriteColored("WARNING: ", "Yellow");
                    tool.WriteLine(text);
                    break;

                case "success":
                    tool.WriteColored("SUCCESS: ", "Green");
                    tool.WriteLine(text);
                    break;

                case "text":
                    tool.WriteLine(text);
                    break;

                default:
                    tool.WriteLine($"write: unknown type '{type}'");
                    tool.WriteLine("Types: error, warning, success, text");
                    break;
            }
        }
    }
}
