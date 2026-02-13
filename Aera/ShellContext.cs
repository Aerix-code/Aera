using System.Text;
using System.Text.RegularExpressions;

namespace Aera
{
    internal class ShellContext
    {
        /* =========================================================
           STATE
        ========================================================= */

        public bool IsSudo { get; set; }
        public bool CaptureMode { get; set; }

        private readonly StringBuilder _buffer = new();
        private string[] _userCredentials = new string[2];

        /* =========================================================
           ANSI / COLOR SYSTEM
        ========================================================= */

        private const string Reset = "\x1b[0m";

        private static readonly Dictionary<string, (int r, int g, int b)> NamedColors =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Black"] = (0,0,0),
                ["White"] = (255,255,255),
                ["Red"] = (255,0,0),
                ["Green"] = (25,172,12),
                ["Blue"] = (0,120,255),
                ["Yellow"] = (255,220,0),
                ["Cyan"] = (0,255,255),
                ["Magenta"] = (255,0,255),
                ["Metablue"] = (45,175,200),

                ["DarkCyan"] = (12,120,120),
                ["DarkRed"] = (139,0,0),
                ["DarkGreen"] = (0,160,0),
                ["DarkYellow"] = (200,150,0),
                ["Gray"] = (180,180,180),
                ["DarkGray"] = (120,120,120)
            };

        public static class Theme
        {
            public const string Prompt = "Yellow";
            public const string Error = "Red";
            public const string Success = "Green";
            public const string Info = "DarkCyan";
            public const string Accent = "Metablue";
        }

        private bool TryResolveColor(string input, out string ansi)
        {
            ansi = "";

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (NamedColors.TryGetValue(input.Trim(), out var rgb))
            {
                ansi = $"\x1b[38;2;{rgb.r};{rgb.g};{rgb.b}m";
                return true;
            }

            var parts = input.Split(',');

            if (parts.Length == 3 &&
                int.TryParse(parts[0].Trim(), out int r) &&
                int.TryParse(parts[1].Trim(), out int g) &&
                int.TryParse(parts[2].Trim(), out int b))
            {
                ansi = $"\x1b[38;2;{r};{g};{b}m";
                return true;
            }

            return false;
        }

        /* =========================================================
           ANSI UTILITIES
        ========================================================= */

        private static readonly Regex AnsiRegex =
            new(@"\x1B\[[0-9;]*m", RegexOptions.Compiled);

        private static string StripAnsi(string input)
            => AnsiRegex.Replace(input, "");

        private static int VisibleLength(string input)
            => StripAnsi(input).Length;

        private static string PadRightAnsi(string input, int width)
        {
            int padding = Math.Max(0, width - VisibleLength(input));
            return input + new string(' ', padding);
        }

        /* =========================================================
           GRADIENT UTILITIES
        ========================================================= */

        public string[] ApplyVerticalGradient(
            string[] lines,
            (int r, int g, int b) start,
            (int r, int g, int b) end)
        {
            if (lines == null || lines.Length == 0)
                return Array.Empty<string>();

            string[] result = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                double t = lines.Length == 1
                    ? 1.0
                    : (double)i / (lines.Length - 1);

                int r = (int)(start.r + (end.r - start.r) * t);
                int g = (int)(start.g + (end.g - start.g) * t);
                int b = (int)(start.b + (end.b - start.b) * t);

                result[i] = $"\x1b[38;2;{r};{g};{b}m{lines[i]}{Reset}";
            }

            return result;
        }

        /* =========================================================
           OUTPUT
        ========================================================= */

        public void Write(string text)
        {
            if (CaptureMode) _buffer.Append(text);
            else Console.Write(text);
        }

        public void WriteLine(string text)
        {
            if (CaptureMode) _buffer.AppendLine(text);
            else Console.WriteLine(text);
        }

        public void WriteColored(string text, string color)
        {
            if (CaptureMode)
            {
                _buffer.Append(text);
                return;
            }

            if (TryResolveColor(color, out var ansi))
                Console.Write($"{ansi}{text}{Reset}");
            else
                Console.Write(text);
        }

        public void WriteLineColored(string text, string color)
        {
            if (CaptureMode)
            {
                _buffer.AppendLine(text);
                return;
            }

            if (TryResolveColor(color, out var ansi))
                Console.WriteLine($"{ansi}{text}{Reset}");
            else
                Console.WriteLine(text);
        }

        public string FlushPipeBuffer()
        {
            var result = _buffer.ToString();
            _buffer.Clear();
            return result;
        }

        public string ReadLine()
            => Console.ReadLine() ?? string.Empty;

        /* =========================================================
           USER BOOTSTRAP / AUTH
        ========================================================= */

        public string GetPassword()
        {
            WriteColored("Enter password: ", Theme.Prompt);

            List<char> buffer = new();

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace && buffer.Count > 0)
                {
                    buffer.RemoveAt(buffer.Count - 1);
                    Console.Write("\b \b");
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    buffer.Add(key.KeyChar);
                    Console.Write("*");
                }
            }

            return new string(buffer.ToArray());
        }

        public string[] CreateUser()
        {
            WriteLine("Create User");

            string username;
            do
            {
                WriteColored("Enter username: ", Theme.Prompt);
                username = ReadLine();
            } while (string.IsNullOrWhiteSpace(username));

            string password;
            do password = GetPassword();
            while (string.IsNullOrWhiteSpace(password));

            _userCredentials[0] = username;
            _userCredentials[1] = password;

            File.WriteAllLines("user.ss", _userCredentials);

            WriteLineColored($"User {username} created.", Theme.Success);
            Thread.Sleep(1200);
            Console.Clear();

            return _userCredentials;
        }

        public void LoadUserCredentials(string[] credentials)
            => _userCredentials = credentials;

        public void Login()
        {
            if (!File.Exists(Program.User) ||
                File.ReadAllLines(Program.User).All(string.IsNullOrWhiteSpace))
            {
                CreateUser();
                return;
            }

            while (true)
            {
                if (_userCredentials.Length < 2)
                {
                    WriteLineColored("User credentials not loaded properly.", Theme.Error);
                    return;
                }

                var pass = GetPassword();

                if (pass == _userCredentials[1])
                {
                    WriteLineColored("Login success", Theme.Success);
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                WriteLineColored("Invalid password", Theme.Error);
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        /* =========================================================
           USER INFO
        ========================================================= */

        public void ShowUser(bool sudo)
        {
            WriteLineColored("User Information:", Theme.Info);
            WriteLineColored($" - Username: {_userCredentials[0]}", Theme.Info);

            string pass = sudo
                ? _userCredentials[1]
                : new string('*', _userCredentials[1].Length);

            WriteLineColored($" - Password: {pass}", Theme.Info);
        }

        public string GetUsername()
            => _userCredentials[0];

        /* =========================================================
           SUDO
        ========================================================= */

        public bool AuthenticateSudo()
        {
            var attempt = GetPassword();

            if (attempt != _userCredentials[1])
            {
                WriteLineColored("No sudo: authentication failed.", Theme.Error);
                return false;
            }

            WriteLineColored("Access granted.", Theme.Success);
            return true;
        }

        public bool Confirm(string message, bool defaultYes = false)
        {
            WriteColored("! ", Theme.Prompt);

            string suffix = defaultYes ? "(Y/n)" : "(y/N)";
            Write($"{message} {suffix} ");

            var input = ReadLine().Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
                return defaultYes;

            return input is "y" or "yes";
        }

        /* =========================================================
           UI RENDERING
        ========================================================= */

        public string RenderRoundedBox(string[] lines)
        {
            if (lines.Length == 0)
                return string.Empty;

            int longest = lines.Max(l => VisibleLength(l ?? ""));
            int width = longest + 5;

            const string TL = "╭";
            const string TR = "╮";
            const string BL = "╰";
            const string BR = "╯";
            const string H = "─";
            const string V = "│";

            const string border = "\x1b[38;2;0;210;190m";

            var sb = new StringBuilder();

            sb.AppendLine($"{border}{TL}{new string(H[0], width + 2)}{TR}{Reset}");

            foreach (var line in lines)
            {
                var padded = PadRightAnsi(line ?? "", width);
                sb.AppendLine($"{border}{V}{Reset} {padded} {border}{V}{Reset}");
            }

            sb.Append($"{border}{BL}{new string(H[0], width + 2)}{BR}{Reset}");

            return sb.ToString();
        }
    }
}