using System;
using System.IO;
using System.Linq;

namespace Aera
{
    internal class DuCommand : ICommand
    {
        public string Name => "du";
        public string Description => "Shows directory disk usage";
        public string Usage => "Usage: du [directory] [-b|-k|-m|-g|-t]";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            string dir = Directory.GetCurrentDirectory();
            char unit = 'b'; // default = bytes

            foreach (var arg in args)
            {
                if (arg.StartsWith("-") && arg.Length == 2)
                {
                    char flag = char.ToLower(arg[1]);

                    if ("bk mgt".Replace(" ", "").Contains(flag))
                        unit = flag;
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
                long sizeBytes = Directory
                    .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                    .Sum(f => new FileInfo(f).Length);

                double converted = ConvertSize(sizeBytes, unit);

                tool.WriteLine($"{converted:0.##} {UnitLabel(unit)}");
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

        private double ConvertSize(long bytes, char unit)
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