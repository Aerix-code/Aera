using System.Runtime.InteropServices;

namespace Aera
{
    internal class FastFetchCommand : ICommand
    {
        public string Name => "fastfetch";
        public string Description => "Prints system info in a square";
        public string Usage => "Usage: fastfetch";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => new[] { "fetch" };

        public void Execute(string[] args, ShellContext tool)
        {
            string[] userCredentials = File.ReadAllLines(Program.User);

            tool.LoadUserCredentials(userCredentials);

            var (totalRam, usedRam) = GetMemoryInfo();

            string[] fetch =
            {
                $"User: {userCredentials[0]}",
                $"Hostname: {Environment.MachineName}",
                $"OS: {RuntimeInformation.OSDescription}",
                $"Architecture: {RuntimeInformation.OSArchitecture}",
                $"Uptime: {GetUptime()}",
                $"CPU Cores: {Environment.ProcessorCount}",
                $"RAM Used: {FormatBytes(usedRam)}",
                $"RAM Total: {FormatBytes(totalRam)}",
                $".NET Version: {Environment.Version}"
            };

            tool.WriteLineColored(tool.RenderRoundedBox(fetch), "Green");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("fastfetch: cannot be used in a pipe", "Red");
        }

        // ------------------------
        // System Helpers
        // ------------------------

        private static string GetUptime()
        {
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }

        private static (ulong total, ulong used) GetMemoryInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return GetLinuxMemory();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsMemory();

            return (0, 0);
        }

        // ------------------------
        // Linux Memory
        // ------------------------
        private static (ulong total, ulong used) GetLinuxMemory()
        {
            if (!File.Exists("/proc/meminfo"))
                return (0, 0);

            var lines = File.ReadAllLines("/proc/meminfo");

            ulong totalKb = ParseMeminfo(lines, "MemTotal");
            ulong availableKb = ParseMeminfo(lines, "MemAvailable");

            ulong usedKb = totalKb - availableKb;

            return (totalKb * 1024, usedKb * 1024);
        }

        // ------------------------
        // Windows Memory
        // ------------------------
        private static (ulong total, ulong used) GetWindowsMemory()
        {
            try
            {
                // Uses GlobalMemoryStatusEx via P/Invoke
                Memorystatusex mem = new Memorystatusex();
                if (GlobalMemoryStatusEx(mem))
                {
                    ulong total = mem.ullTotalPhys;
                    ulong used = total - mem.ullAvailPhys;
                    return (total, used);
                }
            }
            catch
            {
                // ignored
            }

            return (0, 0);
        }

        // Windows native struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class Memorystatusex
        {
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll")]
        private static extern bool GlobalMemoryStatusEx([In, Out] Memorystatusex lpBuffer);

        // ------------------------
        // Helpers
        // ------------------------

        private static ulong ParseMeminfo(string[] lines, string key)
        {
            var line = lines.FirstOrDefault(l => l.StartsWith(key));
            if (line == null) return 0;

            string number = new string(line.Where(char.IsDigit).ToArray());
            return ulong.TryParse(number, out ulong value) ? value : 0;
        }

        private static string FormatBytes(ulong bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB" };
            double value = bytes;
            int i = 0;

            while (value >= 1024 && i < suffix.Length - 1)
            {
                value /= 1024;
                i++;
            }

            return $"{value:0.##} {suffix[i]}";
        }
    }
}
