using System.IO;

namespace Aera
{
    internal class PwdCommand : ICommand
    {
        public string Name => "pwd";
        public string Description => "Shows the current directory";
        public string Usage => "Usage: pwd";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            tool.WriteLine(Directory.GetCurrentDirectory());
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("pwd: cannot be used in a pipe", "Red");
        }
    }
}
