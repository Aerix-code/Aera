using System.Text;

namespace Aera.Commands
{
    internal class EchoCommand : ICommand
    {
        public string Name => "echo";
        public string Description => "Writes text to the console with optional formatting and transformations";
        public string Usage => "Usage: echo [-n] [-e|-E] [-c <color>] [--upper] [--lower] [--repeat N] [--box] <text>";

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

        private void ProcessEcho(string? pipedInput, string[] args, ShellContext tool)
        {
            bool newline = true;
            bool interpretEscapes = false;
            string? color = null;
            bool upper = false;
            bool lower = false;
            bool box = false;
            int repeat = 1;

            var textParts = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch (arg)
                {
                    case "-n": newline = false; break;
                    case "-e": interpretEscapes = true; break;
                    case "-E": interpretEscapes = false; break;
                    case "-c":
                        if (i + 1 >= args.Length)
                        {
                            tool.WriteLineColored("echo: missing color value", "Red");
                            return;
                        }
                        color = args[++i];
                        break;
                    case "--upper": upper = true; break;
                    case "--lower": lower = true; break;
                    case "--box": box = true; break;
                    case "--repeat":
                        if (i + 1 >= args.Length || !int.TryParse(args[i + 1], out repeat) || repeat < 1)
                        {
                            tool.WriteLineColored("echo: invalid repeat count", "Red");
                            return;
                        }
                        i++;
                        break;
                    default:
                        textParts.Add(arg);
                        break;
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

            if (upper) text = text.ToUpper();
            if (lower) text = text.ToLower();

            for (int i = 0; i < repeat; i++)
            {
                if (box)
                {
                    string[] lines = text.Split('\n');
                    tool.WriteLine(tool.RenderRoundedBox(lines));
                    if (newline) tool.WriteLine("");
                }
                else
                {
                    if (color != null) Output(text, newline, color, tool);
                }
            }
        }

        /* ================= OUTPUT ================= */

        private void Output(string text, bool newline, string color, ShellContext tool)
        {
            if (!string.IsNullOrWhiteSpace(color))
            {
                if (newline) tool.WriteLineColored(text, color);
                else tool.WriteColored(text, color);
            }
            else
            {
                if (newline) tool.WriteLine(text);
                else tool.Write(text);
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
