namespace Aera
{
    internal class FindCommand : ICommand
    {
        public string Name => "find";
        public string Description => "Search for files recursively";
        public string Usage => "Usage: find <pattern>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored(Usage, "Red");
                return;
            }

            string pattern = args[0];
            string startDir = Directory.GetCurrentDirectory();

            try
            {
                foreach (var result in SafeEnumerate(startDir, pattern))
                {
                    tool.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"Error: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("find does not support pipe input.", "Red");
        }

        private IEnumerable<string> SafeEnumerate(string root, string pattern)
        {
            var stack = new Stack<string>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                string current = stack.Pop();

                string[] files = Array.Empty<string>();
                string[] dirs = Array.Empty<string>();

                try
                {
                    files = Directory.GetFiles(current);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    dirs = Directory.GetDirectories(current);
                }
                catch
                {
                    // ignored
                }

                foreach (var file in files)
                {
                    if (Path.GetFileName(file)
                        .Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }

                foreach (var dir in dirs)
                    stack.Push(dir);
            }
        }
    }
}
