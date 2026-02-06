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

        public void WriteColored(string t, string c)
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

        public void WriteLineColored(string t, string c)
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

        public string ReadLine() => Console.ReadLine();

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

            UserCredentials[0] = username;
            UserCredentials[1] = password;

            File.WriteAllLines("user.ss", UserCredentials);

            WriteLineColored($"User {username} created.", "Green");
            Thread.Sleep(1200);
            Console.Clear();

            return UserCredentials;
        }

        public void LoadUserCredentials(string[] inf)
        {
            UserCredentials = inf;
        }

        public void Login()
        {
            while (true)
            {
                WriteColored("Password: ", "Cyan");
                string pass = ReadLine();

                if (pass == UserCredentials[1])
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
            WriteLineColored($" - Username: {UserCredentials[0]}", "DarkCyan");

            if (!sudo)
                WriteLineColored($" - Password: {"".PadLeft(UserCredentials[1].Length, '*')}", "DarkCyan");
            else
                WriteLineColored($" - Password: {UserCredentials[1]}", "DarkCyan");
        }

        public string GetUsername() => UserCredentials[0];

        /* ================= SUDO ================= */

        public bool AuthenticateSudo()
        {
            WriteColored("[sudo] Enter password: ", "Yellow");
            string attempt = ReadLine();

            if (attempt != UserCredentials[1])
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

    }
}