using Aera.Commands;

namespace Aera
{
    internal class Program
    {
        public static string User = "";
        public static string Py = "";

        static void Main(string[] args)
        {
            var manager = new CommandManager();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var tool = new ShellContext();

            /* ================= ARGUMENT FLAGS ================= */

            bool skipLogin = args.Contains("--skip-login");

            /* ================= USER FILE PATH ================= */

            string file = @"user.ss";
            string fullPath = Path.GetFullPath(file);
            User = fullPath;
            
            /* =================== PY FILE PATH =================== */
            
            string pyfile = @"nano.py";
            string pyfullPath = Path.GetFullPath(pyfile);
            Py = pyfullPath;

            /* ================= REGISTER COMMANDS ================= */

            // ───────────────────── Core / Meta ─────────────────────
            manager.Register(new HelpCommand(manager));
            manager.Register(new ExitCommand());
            manager.Register(new ClearCommand());
            manager.Register(new ManCommand());

            // ───────────── Identity / Privilege ─────────────
            manager.Register(new WhoAmICommand());
            manager.Register(new UserInfoCommand());
            manager.Register(new SudoCommand(manager));

            // ───────────── Filesystem: Navigation ─────────────
            manager.Register(new PwdCommand());
            manager.Register(new CdCommand());

            // ───────────── Filesystem: Inspection ─────────────
            manager.Register(new LsCommand());
            manager.Register(new TreeCommand());
            manager.Register(new CatCommand());
            manager.Register(new GrepCommand());
            manager.Register(new WcCommand());
            manager.Register(new StatCommand());
            manager.Register(new FindCommand());
            manager.Register(new HeadCommand());
            manager.Register(new TailCommand());
            manager.Register(new DuCommand());

            // ───────────── Filesystem: Mutation ─────────────
            manager.Register(new TouchCommand());
            manager.Register(new MkdirCommand());
            manager.Register(new RmCommand());
            manager.Register(new CpCommand());
            manager.Register(new MvCommand());
            manager.Register(new NanoCommand());

            // ───────────────────── Text Processing ─────────────────────
            manager.Register(new SortCommand());
            manager.Register(new UniqCommand());

            // ───────────────────── Environment ─────────────────────
            manager.Register(new EnvCommand());
            manager.Register(new WhichCommand());

            // ───────────────────── Utilities ─────────────────────
            manager.Register(new DateCommand());
            manager.Register(new TimeCommand());
            manager.Register(new FastFetchCommand());

            // ───────────── Output / Piping Helpers ─────────────
            manager.Register(new WriteCommand());
            manager.Register(new HelloCommand());
            manager.Register(new EchoCommand());

            /* ================= USER BOOTSTRAP ================= */

            string[] userCredentials;

            tool.WriteLine("Welcome to Aera CLI!");
            Thread.Sleep(1500);

            manager.Execute("clear", tool);

            if (File.Exists(Program.User))
            {
                string[] lines = File.ReadAllLines(Program.User);
    
                // Check if file has valid content (at least 2 non-empty lines)
                if (lines.Length >= 2 && lines.All(line => !string.IsNullOrWhiteSpace(line)))
                {
                    userCredentials = lines;
                    tool.LoadUserCredentials(userCredentials);

                    if (!skipLogin)
                    {
                        tool.Login();
                    }
                }
                else
                {
                    // File exists but is empty or invalid - create new user
                    userCredentials = tool.CreateUser();
                }
            }
            else
            {
                // File doesn't exist - create new user
                userCredentials = tool.CreateUser();
            }

            /* ================= RESTORE CONSOLE IF SKIPPED LOGIN ================= */

            if (skipLogin && File.Exists("console.txt"))
            {
                Console.Clear();

                try
                {
                    var content = File.ReadAllText("console.txt");
                    Console.WriteLine(content);
                }
                catch
                {
                    // Fail silently — console restore is optional
                }
            }

            /* ================= START SESSION ================= */

            var username = tool.GetUsername();

            // Only fetch on normal startup
            if (!skipLogin)
            {
                tool.WriteLineColored("    _      _____   ____       _    \n   / \\    | ____| |  _ \\     / \\   \n  / _ \\   |  _|   | |_) |   / _ \\  \n / ___ \\  | |___  |  _ <   / ___ \\ \n/_/   \\_\\ |_____| |_| \\_\\ /_/   \\_\\", "green");
                manager.Execute("fetch", tool);
            }

            while (true)
            {
                tool.WriteColored($"{username}> ", "Cyan");

                var input = tool.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                manager.Execute(input, tool);
            }
        }
    }
}