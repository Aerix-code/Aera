namespace Aera.Commands
{
    internal class WcCommand : ICommand
    {
        public string Name => "wc";
        public string Description => "Count lines, words, and characters";
        public string Usage => "Usage: wc [-l] [-w] [-c] [file...]";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (!TryParse(args, out var options, out var files))
                return;

            if (files.Length == 0)
            {
                tool.WriteLineColored(
                    "wc: no input files (use a pipe or specify files)",
                    "Red");
                return;
            }

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    tool.WriteLineColored($"wc: file not found: {file}", "Red");
                    continue;
                }

                var text = File.ReadAllText(file);
                var counts = Count(text);

                WriteResult(counts, options, tool);
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            if (!TryParse(args, out var options, out _))
                return;

            var counts = Count(input);
            WriteResult(counts, options, tool);
        }

        /* ================= HELPERS ================= */

        private static (int lines, int words, int chars) Count(string text)
        {
            var chars = text.Length;

            var lines = text.Count(c => c == '\n');

            var words = text
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            return (lines, words, chars);
        }

        private static void WriteResult(
            (int lines, int words, int chars) counts,
            WcOptions options,
            ShellContext tool)
        {
            var anyFlag =
                options.Lines || options.Words || options.Chars;

            if (!anyFlag)
            {
                tool.WriteLine($"{counts.lines} {counts.words} {counts.chars}");
                return;
            }

            if (options.Lines)
                tool.Write($"{counts.lines} ");
            if (options.Words)
                tool.Write($"{counts.words} ");
            if (options.Chars)
                tool.Write($"{counts.chars} ");

            tool.WriteLine(string.Empty);
        }

        private static bool TryParse(
            string[] args,
            out WcOptions options,
            out string[] files)
        {
            options = new WcOptions();
            var fileList = new List<string>();

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.Contains('l')) options.Lines = true;
                    if (arg.Contains('w')) options.Words = true;
                    if (arg.Contains('c')) options.Chars = true;
                }
                else
                {
                    fileList.Add(arg);
                }
            }

            files = fileList.ToArray();
            return true;
        }

        private struct WcOptions
        {
            public bool Lines;
            public bool Words;
            public bool Chars;
        }
    }
}
