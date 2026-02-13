using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace Aera.Commands
{
    internal class PingCommand : ICommand
    {
        public string Name => "ping";
        public string Description => "Ping a host to test network connectivity";
        public string Usage => "Usage: ping <host> [-c <count>] [-t] [-w <timeout_ms>]";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored("Usage: ping <host> [-c <count>] [-t] [-w <timeout_ms>]", "Yellow");
                return;
            }

            string host = args[0];
            int count = 4;
            int timeout = 1000;
            bool continuous = false;

            // Parse options
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-c":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out var n))
                        {
                            count = n;
                            i++;
                        }
                        break;
                    case "-t":
                        continuous = true;
                        break;
                    case "-w":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out var t))
                        {
                            timeout = t;
                            i++;
                        }
                        break;
                }
            }

            using var ping = new Ping();
            int sent = 0;
            int received = 0;
            long totalTime = 0;

            void SendPing()
            {
                try
                {
                    var reply = ping.Send(host, timeout);
                    sent++;
                    if (reply.Status == IPStatus.Success)
                    {
                        received++;
                        totalTime += reply.RoundtripTime;
                        tool.WriteLineColored($"Reply from {reply.Address}: time={reply.RoundtripTime}ms", "Green");
                    }
                    else
                    {
                        tool.WriteLineColored($"Request failed: {reply.Status}", "Red");
                    }
                }
                catch (Exception ex)
                {
                    sent++;
                    tool.WriteLineColored($"Error: {ex.Message}", "Red");
                }
            }

            if (continuous)
            {
                tool.WriteLineColored($"Pinging {host} continuously. Ctrl+C to stop.", "Cyan");
                while (true)
                {
                    SendPing();
                    Thread.Sleep(1000);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    SendPing();
                    Thread.Sleep(1000);
                }

                // Summary
                tool.WriteLineColored("--- Ping statistics ---", "Cyan");
                tool.WriteLineColored($"{sent} packets transmitted, {received} received, {(sent - received) * 100 / sent}% packet loss", "Cyan");
                if (received > 0)
                {
                    tool.WriteLineColored($"Average round-trip time: {totalTime / received}ms", "Cyan");
                }
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("ping: cannot be used in a pipe", "Red");
        }
    }
}
