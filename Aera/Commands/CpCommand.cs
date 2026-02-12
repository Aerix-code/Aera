namespace Aera.Commands
{
    internal class CpCommand : ICommand
    {
        public string Name => "cp";
        public string Description => "Copies files or directories";
        public string Usage => "Usage: cp [-r] <source> <destination>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            var recursive = false;
            var index = 0;

            if (args.Length > 0 && args[0] == "-r")
            {
                recursive = true;
                index = 1;
            }

            if (args.Length - index != 2)
            {
                tool.WriteLine("Usage: cp [-r] <source> <destination>");
                return;
            }

            var source = args[index];
            var dest = args[index + 1];

            try
            {
                if (File.Exists(source))
                {
                    CopyFile(source, dest);
                    return;
                }

                if (Directory.Exists(source))
                {
                    if (!recursive)
                    {
                        tool.WriteLine($"cp: -r not specified; omitting directory '{source}'");
                        return;
                    }

                    CopyDirectory(source, dest);
                    return;
                }

                tool.WriteLine($"cp: cannot stat '{source}': No such file or directory");
            }
            catch (Exception ex)
            {
                tool.WriteLine($"cp: {ex.Message}");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("cp: does not accept piped input");
        }

        private void CopyFile(string source, string dest)
        {
            if (Directory.Exists(dest))
                dest = Path.Combine(dest, Path.GetFileName(source));

            File.Copy(source, dest, overwrite: true);
        }

        private void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);

            foreach (var file in Directory.GetFiles(source))
            {
                var target = Path.Combine(dest, Path.GetFileName(file));
                File.Copy(file, target, true);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                var target = Path.Combine(dest, Path.GetFileName(dir));
                CopyDirectory(dir, target);
            }
        }
    }
}
