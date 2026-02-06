using System;

namespace Aera
{
    internal class EchoCommand : ICommand
    {
        public string Name => "echo";
        public string Description => "Writes text to the console";
        public string Usage => "Usage: echo <text>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLine(string.Empty);
                return;
            }

            tool.WriteLine(string.Join(" ", args));
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.Write(input);
        }
    }
}
