namespace Aera
{
    internal class HelloCommand : ICommand
    {
        public string Name => "hello";
        public string Description => "Prints a random hello message"; // i got bored
        public string Usage => "Usage: hello";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => new[] { "hi", "hey", "hai" };

        public void Execute(string[] args, ShellContext tool)
        {
            var rnd = new Random();
            var hey = rnd.Next(0, 9);
            string[] heys = { "HAII", "Welcome to Aera", "Welcome", "Salutations", "Greetings", "Hello and welcome", "Good-day", "It's a pleasure meeting you", "Greeted be thy"};
            tool.WriteLine(heys[hey]);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.Write(input);
            tool.WriteLine(string.Empty);
            Execute(args, tool);
        }
    }
}
