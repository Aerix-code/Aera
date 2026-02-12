namespace Aera
{
    internal class TimeCommand : ICommand
    {
        public string Name => "time";
        public string Description => "Displays the current time";
        public string Usage => "Usage: time";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            tool.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("time: does not accept piped input");
        }
    }
    internal class DateCommand : ICommand
    {
        public string Name => "date";
        public string Description => "Displays the current date";
        public string Usage => "Usage: date";


        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            tool.WriteLine(DateTime.Now.ToString("dd/MM/yyyy"));
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("date: does not accept piped input");
        }
    }
}
