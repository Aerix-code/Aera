namespace Aera
{
    internal class ClearCommand : ICommand
    {
        public string Name => "clear";
        public string Description => "Clears the console screen";
        public string Usage => "Usage: clear";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();


        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 0)
            {
                tool.WriteLine("Usage: clear");
                return;
            }

            Console.Clear();
            Console.WriteLine("\x1b[3J"); // extra thing to fully clear the console since just Console.Clear(); doesn't work
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("clear: does not accept piped input");
        }
    }
}
