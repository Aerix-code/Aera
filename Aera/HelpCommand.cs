namespace Aera
{
    internal class HelpCommand : ICommand
    {
        private readonly CommandManager manager;
        public HelpCommand(CommandManager mgr) => manager = mgr;

        public string Name => "help";
        public string Description => "Lists all available commands";
        public string Usage => "Usage: help <command>(optional)";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => new[] { "man" };

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                manager.ShowAll(tool);
                return;
            }

            if (args.Contains("--help"))
            {
                manager.ShowCommandHelp(this, tool);
                return;
            }

            var name = args[0].ToLower();

            if (!manager.TryGet(name, out var cmd))
            {
                tool.WriteLine($"No manual entry for '{name}'.");
                return;
            }

            manager.ShowCommandHelp(cmd, tool);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("help(man): cannot be used in a pipe", "Red");
        }
    }
}
