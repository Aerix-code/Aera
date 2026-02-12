namespace Aera.Commands
{
    internal class TreeCommand : ICommand
    {
        public string Name => "tree";
        public string Description => "Displays directory tree";
        public string Usage => "Usage: tree <tag> <value>";

        public bool AcceptsPipeInput => false; // tree should not read stdin
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Contains("--help"))
            {
                ShowHelp(tool);
                return;
            }

            bool dirsOnly = false;
            bool fullPath = false;
            int maxDepth = int.MaxValue;

            string path = Directory.GetCurrentDirectory();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-d":
                        dirsOnly = true;
                        break;

                    case "-f":
                        fullPath = true;
                        break;

                    case "-L":
                        if (i + 1 >= args.Length || !int.TryParse(args[i + 1], out maxDepth))
                        {
                            tool.WriteLineColored("tree: invalid depth for -L", "Red");
                            return;
                        }
                        i++; // consume depth
                        break;

                    default:
                        if (args[i].StartsWith("-"))
                        {
                            tool.WriteLineColored($"tree: unknown option '{args[i]}'", "Red");
                            return;
                        }

                        path = Path.GetFullPath(args[i]);
                        break;
                }
            }

            if (!Directory.Exists(path))
            {
                tool.WriteLineColored("tree: directory not found", "Red");
                return;
            }

            tool.WriteLineColored(fullPath ? path : Path.GetFileName(path), "DarkCyan");
            PrintTree(path, "", 0, maxDepth, dirsOnly, fullPath, tool);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("tree: does not accept piped input");
        }

        private void ShowHelp(ShellContext tool)
        {
            tool.WriteLine("Usage: tree [options] [path]");
            tool.WriteLine("");
            tool.WriteLine("Options:");
            tool.WriteLine("  -d            Directories only");
            tool.WriteLine("  -L <depth>    Limit display depth");
            tool.WriteLine("  -f            Show full paths");
            tool.WriteLine("  --help        Show this help message");
        }

        private void PrintTree(
            string path,
            string indent,
            int depth,
            int maxDepth,
            bool dirsOnly,
            bool fullPath,
            ShellContext tool)
        {
            if (depth >= maxDepth)
                return;

            string[] dirs;
            string[] files;

            try
            {
                dirs = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            }
            catch
            {
                tool.WriteLineColored($"{indent}└── [access denied]", "Red");
                return;
            }

            var entries = dirsOnly
                ? dirs
                : dirs.Concat(files);

            var list = entries.ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                var isLast = i == list.Length - 1;
                var isDir = Directory.Exists(list[i]);

                var name = fullPath
                    ? list[i]
                    : Path.GetFileName(list[i]);

                tool.WriteLineColored(
                    $"{indent}{(isLast ? "└── " : "├── ")}{name}",
                    isDir ? "Yellow" : "Gray"
                );

                if (isDir)
                {
                    PrintTree(
                        list[i],
                        indent + (isLast ? "    " : "│   "),
                        depth + 1,
                        maxDepth,
                        dirsOnly,
                        fullPath,
                        tool
                    );
                }
            }
        }
    }
}
