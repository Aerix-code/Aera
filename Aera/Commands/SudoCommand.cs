namespace Aera.Commands
{
    internal class SudoCommand(CommandManager mgr) : ICommand
    {
        public string Name => "sudo";
        public string Description => "Executes a command with elevated privileges";
        public string Usage => "Usage: sudo <command>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLine("Usage: sudo <command>");
                return;
            }
            // validate sudo
            if (!tool.AuthenticateSudo())
                return;

            var previousSudo = tool.IsSudo;

            try
            {
                tool.IsSudo = true;

                var commandName = args[0];
                string[] commandArgs = args.Skip(1).ToArray();

                mgr.ExecuteSudo(commandName, commandArgs, tool);
            }
            finally
            {
                tool.IsSudo = previousSudo;
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("sudo: does not accept piped input");
        }
    }
}