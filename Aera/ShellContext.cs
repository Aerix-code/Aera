using System.Text;

namespace Aera
{
    internal class ShellContext
    {
        public bool IsSudo { get; set; }
        public bool CaptureMode { get; set; }

        private readonly StringBuilder _buffer = new();
        private string[] _userCredentials = new string[2];

        /* ================= OUTPUT ================= */

        public void WriteLine(string t)
        {
            if (CaptureMode)
                _buffer.AppendLine(t);
            else
                Console.WriteLine(t);
        }

        public void Write(string t)
        {
            if (CaptureMode)
                _buffer.Append(t);
            else
                Console.Write(t);
        }

        public void WriteColored(string t, string c)
        {
            if (CaptureMode)
            {
                _buffer.Append(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.Write(t);
            Console.ResetColor();
        }

        public void WriteLineColored(string t, string c)
        {
            if (CaptureMode)
            {
                _buffer.AppendLine(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.WriteLine(t);
            Console.ResetColor();
        }

        public string FlushPipeBuffer()
        {
            var result = _buffer.ToString();
            _buffer.Clear();
            return result;
        }

        public string ReadLine() => Console.ReadLine() ?? string.Empty;

        /* ================= USER BOOTSTRAP ================= */

        public string[] CreateUser()
        {
            WriteLine("Create User");

            string username;
            do
            {
                WriteColored("Enter username: ", "Yellow");
                username = ReadLine();
            } while (string.IsNullOrWhiteSpace(username));

            string password;
            do
            {
                WriteColored("Enter password: ", "Yellow");
                password = ReadLine();
            } while (string.IsNullOrWhiteSpace(password));

            _userCredentials[0] = username;
            _userCredentials[1] = password;

            File.WriteAllLines("user.ss", _userCredentials);

            WriteLineColored($"User {username} created.", "Green");
            Thread.Sleep(1200);
            Console.Clear();

            return _userCredentials;
        }

        public void LoadUserCredentials(string[] inf)
        {
            _userCredentials = inf;
        }

        public void Login()
        {
            while (true)
            {
                WriteColored("Password: ", "Cyan");
                var pass = ReadLine();

                if (pass == _userCredentials[1])
                {
                    WriteLineColored("Login success", "Green");
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                WriteLineColored("Invalid password", "Red");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        /* ================= USER INFO ================= */

        public void ShowUser(bool sudo)
        {
            WriteLineColored("User Information:", "DarkCyan");
            WriteLineColored($" - Username: {_userCredentials[0]}", "DarkCyan");

            WriteLineColored(
                !sudo
                    ? $" - Password: {"".PadLeft(_userCredentials[1].Length, '*')}"
                    : $" - Password: {_userCredentials[1]}", "DarkCyan");
        }

        public string GetUsername() => _userCredentials[0];

        /* ================= SUDO ================= */

        public bool AuthenticateSudo()
        {
            WriteColored("[sudo] Enter password: ", "Yellow");
            var attempt = ReadLine();

            if (attempt != _userCredentials[1])
            {
                WriteLineColored("No sudo: authentication failed.", "Red");
                return false;
            }

            WriteLineColored("Access granted.", "Green");
            IsSudo = true;
            return true;
        }
        public bool Confirm(string message, bool defaultYes = false)
        {
            var suffix = defaultYes ? "(Y/n)" : "(y/N)";
            WriteColored("! ", "yellow");
            Write($"{message} {suffix} ");

            var input = ReadLine()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
                return defaultYes;

            return input == "y" || input == "yes";
        }

        public string RenderRoundedBox(string[] lines)
        {
            if (lines.Length == 0)
                return string.Empty;

            int longest = lines.Max(l => l?.Length ?? 0);
            int contentWidth = longest + 5;

            const string topLeft = "╭";
            const string topRight = "╮";
            const string bottomLeft = "╰";
            const string bottomRight = "╯";
            const string horizontal = "─";
            const string vertical = "│";

            // ANSI Colors
            const string green = "\u001b[32m";
            const string white = "\u001b[37m";
            const string reset = "\u001b[0m";

            var sb = new StringBuilder();

            // Top border
            sb.AppendLine($"{green}{topLeft}{new string(horizontal[0], contentWidth + 2)}{topRight}{reset}");

            // Content lines
            foreach (var line in lines)
            {
                var safeLine = line ?? string.Empty;
                var padded = safeLine.PadRight(contentWidth, ' ');

                sb.AppendLine($"{green}{vertical}{reset} {white}{padded}{reset} {green}{vertical}{reset}");
            }

            // Bottom border
            sb.Append($"{green}{bottomLeft}{new string(horizontal[0], contentWidth + 2)}{bottomRight}{reset}");

            return sb.ToString();
        }
    }
}