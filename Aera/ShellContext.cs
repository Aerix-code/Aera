using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Aera
{
    internal class ShellContext
    {
        public bool IsSudo { get; set; }
        public bool CaptureMode { get; set; }

        private readonly StringBuilder buffer = new();
        public string[] UserCredentials = new string[2];

        /* ================= OUTPUT ================= */

        public void WriteLine(string t)
        {
            if (CaptureMode)
                buffer.AppendLine(t);
            else
                Console.WriteLine(t);
        }

        public void Write(string t)
        {
            if (CaptureMode)
                buffer.Append(t);
            else
                Console.Write(t);
        }

        public void WriteColor(string t, string c)
        {
            if (CaptureMode)
            {
                buffer.Append(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.Write(t);
            Console.ResetColor();
        }

        public void WriteLineColor(string t, string c)
        {
            if (CaptureMode)
            {
                buffer.AppendLine(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.WriteLine(t);
            Console.ResetColor();
        }

        public string FlushPipeBuffer()
        {
            string result = buffer.ToString();
            buffer.Clear();
            return result;
        }

        public string GetInput() => Console.ReadLine();

        /* ================= USER BOOTSTRAP ================= */

        public string[] CreateUser()
        {
            WriteLine("Create User");

            string username;
            do
            {
                WriteColor("Enter username: ", "Yellow");
                username = GetInput();
            } while (string.IsNullOrWhiteSpace(username));

            string password;
            do
            {
                WriteColor("Enter password: ", "Yellow");
                password = GetInput();
            } while (string.IsNullOrWhiteSpace(password));

            UserCredentials[0] = username;
            UserCredentials[1] = password;

            File.WriteAllLines("user.ss", UserCredentials);

            WriteLineColor($"User {username} created.", "Green");
            Thread.Sleep(1200);
            Console.Clear();

            return UserCredentials;
        }

        public void LoadUserCredentials(string[] inf)
        {
            UserCredentials = inf;
        }

        public void ValidatePassword()
        {
            while (true)
            {
                WriteColor("Password: ", "Cyan");
                string pass = GetInput();

                if (pass == UserCredentials[1])
                {
                    WriteLineColor("Login success", "Green");
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                WriteLineColor("Invalid password", "Red");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        /* ================= USER INFO ================= */

        public void ShowUser(bool sudo)
        {
            WriteLineColor("User Information:", "DarkCyan");
            WriteLineColor($" - Username: {UserCredentials[0]}", "DarkCyan");

            if (!sudo)
                WriteLineColor($" - Password: {"".PadLeft(UserCredentials[1].Length, '*')}", "DarkCyan");
            else
                WriteLineColor($" - Password: {UserCredentials[1]}", "DarkCyan");
        }

        public string un() => UserCredentials[0];

        /* ================= SUDO ================= */

        public bool AuthenticateSudo()
        {
            WriteColor("[sudo] Enter password: ", "Yellow");
            string attempt = GetInput();

            if (attempt != UserCredentials[1])
            {
                WriteLineColor("No sudo: authentication failed.", "Red");
                return false;
            }

            WriteLineColor("Access granted.", "Green");
            IsSudo = true;
            return true;
        }
        public bool Confirm(string message, bool defaultYes = false)
        {
            var suffix = defaultYes ? "(Y/n)" : "(y/N)";
            WriteColor("! ", "yellow");
            Write($"{message} {suffix} ");

            var input = GetInput()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
                return defaultYes;

            return input == "y" || input == "yes";
        }

    }
}