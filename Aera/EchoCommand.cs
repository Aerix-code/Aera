using System;
using System.Linq;
using System.Text;

namespace Aera
{
    internal class EchoCommand : ICommand
    {
        public string Name => "echo";
        public string Description => "Writes text to the console";
        public string Usage => "Usage: echo [-n] [-e|-E] [-c <color>] <text>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            ProcessEcho(null, args, tool);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            ProcessEcho(input, args, tool);
        }

        /* ================= CORE ================= */

        private void ProcessEcho(string pipedInput, string[] args, ShellContext tool)
        {
            bool newline = true;
            bool interpretEscapes = false;
            string color = null;

            var textParts = new System.Collections.Generic.List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == "-n")
                    newline = false;
                else if (arg == "-e")
                    interpretEscapes = true;
                else if (arg == "-E")
                    interpretEscapes = false;
                else if (arg == "-c")
                {
                    if (i + 1 >= args.Length)
                    {
                        tool.WriteLineColored("echo: missing color value", "Red");
                        return;
                    }

                    color = args[++i];
                }
                else
                {
                    textParts.Add(arg);
                }
            }

            string text = string.Join(" ", textParts);

            if (!string.IsNullOrEmpty(pipedInput))
            {
                if (!string.IsNullOrEmpty(text))
                    text = pipedInput + text;
                else
                    text = pipedInput;
            }

            if (interpretEscapes)
                text = ParseEscapes(text);

            Output(text, newline, color, tool);
        }

        /* ================= OUTPUT ================= */

        private void Output(string text, bool newline, string color, ShellContext tool)
        {
            if (!string.IsNullOrWhiteSpace(color))
            {
                if (newline)
                    tool.WriteLineColored(text, color);
                else
                    tool.WriteColored(text, color);
            }
            else
            {
                if (newline)
                    tool.WriteLine(text);
                else
                    tool.Write(text);
            }
        }

        /* ================= ESCAPES ================= */

        private string ParseEscapes(string input)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\' && i + 1 < input.Length)
                {
                    i++;
                    sb.Append(input[i] switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        'r' => '\r',
                        '\\' => '\\',
                        '"' => '"',
                        _ => input[i]
                    });
                }
                else
                {
                    sb.Append(input[i]);
                }
            }

            return sb.ToString();
        }
    }
}
