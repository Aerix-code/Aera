using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Aera
{
    internal class DuCommand : ICommand
    {
        public string Name => "du";
        public string Description => "Shows directory disk usage";
        public string Usage => "Usage: du [directory] [-b|-k|-m|-g|-t|-h] [-s] [-d N]";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            string dir = Directory.GetCurrentDirectory();
            char unit = 'b';
            bool human = false;
            bool summary = false;
            int depthLimit = int.MaxValue;

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("-"))
                {
                    if (arg == "-h") human = true;
                    else if (arg == "-s") summary = true;
                    else if (arg == "-d")
                    {
                        if (i + 1 >= args.Length || !int.TryParse(args[i + 1], out depthLimit))
                        {
                            tool.WriteLineColored("Invalid depth value.", "Red");
                            return;
                        }
                        i++;
                    }
                    else if (arg.Length == 2 && "bkmgt".Contains(char.ToLower(arg[1])))
                    {
                        unit = char.ToLower(arg[1]);
                    }
                    else
                    {
                        tool.WriteLineColored($"Invalid option: {arg}", "Red");
                        return;
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

            try
            {
                long total = CalculateDirectory(dir, tool, dir, 0, depthLimit, summary, human, unit);
                
                if (summary)
                    PrintSize(total, dir, tool, human, unit);
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"Error: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            Execute(args.Append(input.Trim()).ToArray(), tool);
        }

        /* ================= CORE SIZE LOGIC ================= */

        private long CalculateDirectory(
            string root,
            ShellContext tool,
            string current,
            int depth,
            int depthLimit,
            bool summary,
            bool human,
            char unit)
        {
            long size = 0;

            if (depth > depthLimit)
                return 0;

            try
            {
                foreach (var file in Directory.EnumerateFiles(current))
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch { }
                }

                foreach (var dir in Directory.EnumerateDirectories(current))
                {
                    size += CalculateDirectory(root, tool, dir, depth + 1, depthLimit, summary, human, unit);
                }
            }
            catch { }

            if (!summary && depth <= depthLimit)
                PrintSize(size, current, tool, human, unit);

            return size;
        }

        /* ================= PRINTING ================= */

        private void PrintSize(long bytes, string path, ShellContext tool, bool human, char unit)
        {
            if (human)
            {
                var (val, label) = HumanReadable(bytes);
                tool.WriteLine($"{val:0.##} {label}\t{path}");
            }
            else
            {
                double converted = ConvertUnit(bytes, unit);
                tool.WriteLine($"{converted:0.##} {UnitLabel(unit)}\t{path}");
            }
        }

        /* ================= CONVERSIONS ================= */

        private (double, string) HumanReadable(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int index = 0;

            while (size >= 1024 && index < units.Length - 1)
            {
                size /= 1024;
                index++;
            }

            return (size, units[index]);
        }

        private double ConvertUnit(long bytes, char unit)
        {
            return unit switch
            {
                'k' => bytes / 1024.0,
                'm' => bytes / Math.Pow(1024, 2),
                'g' => bytes / Math.Pow(1024, 3),
                't' => bytes / Math.Pow(1024, 4),
                _ => bytes
            };
        }

        private string UnitLabel(char unit)
        {
            return unit switch
            {
                'k' => "KB",
                'm' => "MB",
                'g' => "GB",
                't' => "TB",
                _ => "bytes"
            };
        }
    }
}
