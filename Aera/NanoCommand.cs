namespace Aera
{
    // hypothetical
    
    // save current console to console.txt,
    // clear console,
    // run second program Nano,
    // Nano runs and does what it should,
    // Nano closes starts aera with a variable that skips login and loads console.txt,
    // console.txt loads and makes console content reappear as if nothing happened.
    
    internal class NanoCommand : ICommand
    {
        public string Name => "nano <file>";
        public string Description => "Edit document contents";
        public string Usage => "Usage: nano";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            tool.WriteLineColored("This command is under construction", "yellow");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("nano: cannot be used in a pipe", "Red");
        }
    }
}