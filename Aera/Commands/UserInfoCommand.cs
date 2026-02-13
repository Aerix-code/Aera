namespace Aera.Commands
{
    internal class UserInfoCommand : ICommand
    {
        public string Name => "userinfo";
        public string Description => "Displays user information";
        public string Usage => "Usage: <sudo>(optional) userinfo";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 0)
            {
                tool.WriteLine("Usage: userinfo");
                return;
            }

            tool.ShowUser(tool.IsSudo);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("userinfo: does not accept piped input");
        }
    }
    internal class WhoAmICommand : ICommand
    {
        public string Name => "whoami";
        public string Description => "Displays user name";
        public string Usage => "Usage: whoami";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 0)
            {
                tool.WriteLine("Usage: whoami");
                return;
            }

            tool.WriteLineColored(tool.GetUsername(), "Blue");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("whoami: does not accept piped input");
        }
    }
}
