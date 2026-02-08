using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Aera
{
    internal class LsCommand : ICommand
    {
        public string Name => "ls";
        public string Description => "Lists files and folders";
        public string Usage => "Usage: ls [directory] [-a] [-l] [-h] [-r] [-t] [-s]";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            string dir = Directory.GetCurrentDirectory();

            bool showHidden = false;
            bool longFormat = false;
            bool human = false;
            bool reverse = false;
            bool sortTime = false;
            bool showSize = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    foreach (var flag in arg.Skip(1))
                    {
                        switch (flag)
                        {
                            case 'a': showHidden = true; break;
                            case 'l': longFormat = true; break;
                            case 'h': human = true; break;
                            case 'r': reverse = true; break;
                            case 't': sortTime = true; break;
                            case 's': showSize = true; break;
                            default:
                                tool.WriteLineColored($"Invalid option: -{flag}", "Red");
                                return;
                        }
                    }
                }
                else
                {
                    dir = arg;
                }
            }

            if (!Directory.Exists(dir))
            {
                tool.WriteLineColored("Directory not found.", "Red");
                return;
            }

            tool.WriteLineColored($"Directory: {dir}", "DarkCyan");

            try
            {
                var entries = new List<FileSystemInfo>();

                entries.AddRange(new DirectoryInfo(dir).GetDirectories());
                entries.AddRange(new DirectoryInfo(dir).GetFiles());

                if (!showHidden)
                    entries = entries.Where(e => !IsHidden(e)).ToList();

                if (sortTime)
                    entries = entries.OrderByDescending(e => e.LastWriteTime).ToList();
                else
                    entries = entries.OrderBy(e => e.Name).ToList();

                if (reverse)
                    entries.Reverse();

                foreach (var entry in entries)
                {
                    if (entry is DirectoryInfo d)
                        PrintDirectory(d, tool, longFormat, human, showSize);
                    else if (entry is FileInfo f)
                        PrintFile(f, tool, longFormat, human, showSize);
                }
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"Error: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("ls: cannot accept piped input", "Red");
        }

        /* ================= PRINTING ================= */

        private void PrintDirectory(DirectoryInfo dir, ShellContext tool, bool longFormat, bool human, bool showSize)
        {
            if (longFormat)
            {
                tool.WriteLineColored(
                    $"{FormatTime(dir.LastWriteTime)} <DIR>\t{dir.Name}",
                    "Yellow");
            }
            else
            {
                tool.WriteLineColored($"[DIR]  {dir.Name}", "Yellow");
            }
        }

        private void PrintFile(FileInfo file, ShellContext tool, bool longFormat, bool human, bool showSize)
        {
            if (longFormat)
            {
                string size = human
                    ? HumanReadable(file.Length)
                    : $"{file.Length}";

                tool.WriteLine($"{FormatTime(file.LastWriteTime)} {size}\t{file.Name}");
            }
            else if (showSize)
            {
                tool.WriteLine($"[FILE] {file.Length}\t{file.Name}");
            }
            else
            {
                tool.WriteLine($"[FILE] {file.Name}");
            }
        }

        /* ================= HELPERS ================= */

        private bool IsHidden(FileSystemInfo info)
        {
            if (OperatingSystem.IsWindows())
                return info.Attributes.HasFlag(FileAttributes.Hidden);

            return info.Name.StartsWith(".");
        }

        private string FormatTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm");
        }

        private string HumanReadable(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int i = 0;

            while (size >= 1024 && i < units.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size:0.##}{units[i]}";
        }
    }
}
