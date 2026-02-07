using System.Text;

namespace Aera
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new CommandManager();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Register commands

            // ───────────────────── Core / Meta ─────────────────────
            manager.Register(new HelpCommand(manager));    // help | man — list all commands or show command manuals
            manager.Register(new ExitCommand());           // exit | close | shutdown — terminate the CLI
            manager.Register(new ClearCommand());          // clear — clear the console screen

            // ───────────── Identity / Privilege ─────────────
            manager.Register(new WhoAmICommand());         // whoami — display the current username
            manager.Register(new UserInfoCommand());       // userinfo — show user info (password masked without sudo)
            manager.Register(new SudoCommand(manager));    // sudo — execute a command with elevated privileges

            // ───────────── Filesystem: Navigation ─────────────
            manager.Register(new PwdCommand());            // pwd — print the current working directory
            manager.Register(new CdCommand());             // cd — change the current directory

            // ───────────── Filesystem: Inspection ─────────────
            manager.Register(new LsCommand());             // ls — list files and directories in the current directory
            manager.Register(new TreeCommand());           // tree — display directory structure recursively
            manager.Register(new CatCommand());            // cat — output the contents of a file
            manager.Register(new GrepCommand());           // grep — search for lines matching a pattern
            manager.Register(new WcCommand());             // wc — count lines, words, and characters

            // ───────────── Filesystem: Mutation ─────────────
            manager.Register(new TouchCommand());          // touch — create empty files or update timestamps
            manager.Register(new MkdirCommand());           // mkdir — create a new directory
            manager.Register(new RmCommand());             // rm — delete files or directories (destructive)
            manager.Register(new CpCommand());             // cp — copy files or directories
            manager.Register(new MvCommand());             // mv — move or rename files and directories

            // ───────────────────── Utilities ─────────────────────
            manager.Register(new DateCommand());           // date — display the current date
            manager.Register(new TimeCommand());           // time — display the current time
            manager.Register(new FastFetchCommand());      // fastfetch | fetch - displays device info in a square

            // ───────────── Output / Piping Helpers ─────────────
            manager.Register(new WriteCommand());          // write — print formatted messages to the console
            manager.Register(new HelloCommand());          // hello | hi | hey | hai — print a greeting to the console
            manager.Register(new EchoCommand());           // echo — write text to the console

            string[] UserCredentials = new string[2];
            var tool = new ShellContext();

            tool.WriteLine("Welcome to Aera CLI!");
            Thread.Sleep(1500);
            manager.Execute("clear", tool);
            if (File.Exists("user.ss"))
                {
                    UserCredentials = File.ReadAllLines("user.ss");
                    tool.LoadUserCredentials(UserCredentials);
                    tool.Login();
                }
                else
                    UserCredentials = tool.CreateUser();
            string n = tool.GetUsername();
            manager.Execute("fetch", tool);
            while (true)
            {
                tool.WriteColored($"{n}> ", "Cyan");
                string input = tool.ReadLine();
                manager.Execute(input, tool);
            }

        }
    }
}
