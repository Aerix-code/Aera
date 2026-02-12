using System.Collections;

namespace Aera.Commands
{
    internal class EnvCommand : ICommand
    {
        public string Name => "env";
        public string Description => "Displays environment variables";
        public string Usage => "Usage: env";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                tool.WriteLine($"{entry.Key}={entry.Value}");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            Execute(args, tool);
        }
    }
}