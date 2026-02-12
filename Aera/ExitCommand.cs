namespace Aera
{
    internal class ExitCommand : ICommand
    {
        public string Name => "exit";
        public string Description => "Closes the CLI";
        public string Usage => "Usage: exit";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;

        public string[] Aliases => new[] { "exit", "close", "shutdown" };

        public void Execute(string[] args, ShellContext tool)
        {
            Environment.Exit(0);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("exit: cannot be used in a pipe", "Red");
        }
    }
}
