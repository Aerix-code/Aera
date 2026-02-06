using System;
using System.Collections.Generic;
using System.IO;

namespace Aera
{
    internal class GrepCommand : ICommand
    {
        public string Name => "grep";
        public string Description => "Search for lines matching a pattern";
        public string Usage => "Usage: grep [-i] [-v] <pattern> [file...]";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (!TryParse(args, tool, out var options, out var pattern, out var files))
                return;

            if (files.Count == 0)
            {
                tool.WriteLineColored(
                    "grep: no input files (use a pipe or specify files)",
                    "Red");
                return;
            }

            var comparison = options.IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    tool.WriteLineColored($"grep: file not found: {file}", "Red");
                    continue;
                }

                foreach (var line in File.ReadLines(file))
                {
                    if (Matches(line, pattern, comparison, options.Invert))
                        tool.WriteLine(line);
                }
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            if (!TryParse(args, tool, out var options, out var pattern, out _))
                return;

            var comparison = options.IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            foreach (var raw in input.Split('\n'))
            {
                var line = raw.TrimEnd('\r');

                if (Matches(line, pattern, comparison, options.Invert))
                    tool.WriteLine(line);
            }
        }

        /* ================= HELPERS ================= */

        private static bool Matches(
            string line,
            string pattern,
            StringComparison comparison,
            bool invert)
        {
            bool found = line.IndexOf(pattern, comparison) >= 0;
            return invert ? !found : found;
        }

        private static bool TryParse(
            string[] args,
            ShellContext tool,
            out GrepOptions options,
            out string pattern,
            out List<string> files)
        {
            options = new GrepOptions();
            files = new List<string>();
            pattern = null;

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.Contains('i')) options.IgnoreCase = true;
                    if (arg.Contains('v')) options.Invert = true;
                }
                else if (pattern == null)
                {
                    pattern = arg;
                }
                else
                {
                    files.Add(arg);
                }
            }

            if (pattern == null)
            {
                tool.WriteLineColored("grep: missing search pattern", "Red");
                return false;
            }

            return true;
        }

        private struct GrepOptions
        {
            public bool IgnoreCase;
            public bool Invert;
        }
    }
}
