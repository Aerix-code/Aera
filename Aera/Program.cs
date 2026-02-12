namespace Aera
{
    internal class Program
    {
        public static string User = "";
        static void Main(string[] args)
        {
            var manager = new CommandManager();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            var tool = new ShellContext();
            
            // set up user.ss full path
            string fullPath;
            string file = @"user.ss";
            fullPath = Path.GetFullPath(file);
            User = fullPath;

            // Register commands

            // ───────────────────── Core / Meta ─────────────────────
            manager.Register(new HelpCommand(manager)); // help | man — list all commands or show command manuals
            manager.Register(new ExitCommand()); // exit | close | shutdown — terminate the CLI
            manager.Register(new ClearCommand()); // clear — clear the console screen

            // ───────────── Identity / Privilege ─────────────
            manager.Register(new WhoAmICommand()); // whoami — display the current username
            manager.Register(new UserInfoCommand()); // userinfo — show user info (password masked without sudo)
            manager.Register(new SudoCommand(manager)); // sudo — execute a command with elevated privileges

            // ───────────── Filesystem: Navigation ─────────────
            manager.Register(new PwdCommand()); // pwd — print the current working directory
            manager.Register(new CdCommand()); // cd — change the current directory

            // ───────────── Filesystem: Inspection ─────────────
            manager.Register(new LsCommand()); // ls — list files and directories in the current directory
            manager.Register(new TreeCommand()); // tree — display directory structure recursively
            manager.Register(new CatCommand()); // cat — output the contents of a file
            manager.Register(new GrepCommand()); // grep — search for lines matching a pattern
            manager.Register(new WcCommand()); // wc — count lines, words, and characters
            manager.Register(new StatCommand()); // stat — display file or directory metadata
            manager.Register(new FindCommand()); // find — recursive file search
            manager.Register(new HeadCommand()); // head — display first N lines of file or piped input
            manager.Register(new TailCommand()); // tail — display last N lines of file or piped input
            manager.Register(new DuCommand()); // du — calculate directory disk usage

            // ───────────── Filesystem: Mutation ─────────────
            manager.Register(new TouchCommand()); // touch — create empty files or update timestamps
            manager.Register(new MkdirCommand()); // mkdir — create a new directory
            manager.Register(new RmCommand()); // rm — delete files or directories (destructive)
            manager.Register(new CpCommand()); // cp — copy files or directories
            manager.Register(new MvCommand()); // mv — move or rename files and directories

            // ───────────────────── Text Processing ─────────────────────
            manager.Register(new SortCommand()); // sort — sort lines alphabetically
            manager.Register(new UniqCommand()); // uniq — remove duplicate lines

            // ───────────────────── Environment ─────────────────────
            manager.Register(new EnvCommand()); // env — display environment variables
            manager.Register(new WhichCommand()); // which — locate executable in PATH

            // ───────────────────── Utilities ─────────────────────
            manager.Register(new DateCommand()); // date — display the current date
            manager.Register(new TimeCommand()); // time — display the current time
            manager.Register(new FastFetchCommand()); // fastfetch | fetch — displays device info in a square

            // ───────────── Output / Piping Helpers ─────────────
            manager.Register(new WriteCommand()); // write — print formatted messages to the console
            manager.Register(new HelloCommand()); // hello | hi | hey | hai — print a greeting to the console
            manager.Register(new EchoCommand()); // echo — write text to the console


            string[] userCredentials;

            tool.WriteLine("Welcome to Aera CLI!");
            Thread.Sleep(1500);
            manager.Execute("clear", tool);
            if (File.Exists("user.ss"))
            {
                userCredentials = File.ReadAllLines("user.ss");
                tool.LoadUserCredentials(userCredentials);
                tool.Login();
            }
            else
                userCredentials = tool.CreateUser();
            var n = tool.GetUsername();
            manager.Execute("fetch", tool);
            while (true)
            {
                tool.WriteColored($"{n}> ", "Cyan");
                var input = tool.ReadLine();
                manager.Execute(input, tool);
            }
        }
    }
}
