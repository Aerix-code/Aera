namespace Aera
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }

        bool AcceptsPipeInput { get; }
        bool IsDestructive { get; }

        string[] Aliases { get; }

        void Execute(string[] args, ShellContext tool);

        // Only called if AcceptsPipeInput == true
        void ExecutePipe(string input, string[] args, ShellContext tool);
    }
}
