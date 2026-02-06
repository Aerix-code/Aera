using System;
using System.Collections.Generic;
using System.Linq;

namespace Aera
{
    /// <summary>
    /// Central dispatcher and registry for all CLI commands.
    /// Handles command lookup, aliases, piping, sudo execution,
    /// and help/man output.
    /// </summary>
    internal class CommandManager
    {
        // Fast lookup by command name or alias
        private readonly Dictionary<string, ICommand> commands = new();

        // Preserves registration order for help output
        private readonly List<ICommand> orderedCommands = new();

        // ───────────────────── Registration ─────────────────────

        /// <summary>
        /// Registers a command and all of its aliases.
        /// </summary>
        public void Register(ICommand command)
        {
            // Register primary name
            commands[command.Name.ToLower()] = command;

            // Register aliases
            foreach (var alias in command.Aliases)
                commands[alias.ToLower()] = command;

            // Preserve order for help/man
            orderedCommands.Add(command);
        }

        // ───────────────────── Execution Entry ─────────────────────

        /// <summary>
        /// Entry point for executing user input.
        /// Handles pipe splitting and dispatch.
        /// </summary>
        public void Execute(string input, ShellContext tool)
        {
            // Split on first pipe only
            var parts = input.Split('|', 2, StringSplitOptions.TrimEntries);

            // No pipe → normal execution
            if (parts.Length == 1)
            {
                ExecuteSingle(parts[0], tool);
                return;
            }

            // Pipe detected
            string left = parts[0];
            string right = parts[1];

            string pipeOutput = CaptureOutput(left, tool);
            ExecutePiped(right, pipeOutput, tool);
        }

        // ───────────────────── Single Command Execution ─────────────────────

        /// <summary>
        /// Executes a single command without piping.
        /// </summary>
        private void ExecuteSingle(string input, ShellContext tool)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string name = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (!commands.TryGetValue(name, out var cmd))
            {
                tool.WriteLine($"Command '{name}' not recognized.");
                return;
            }

            // Automatic per-command --help
            if (args.Contains("--help"))
            {
                ShowCommandHelp(cmd, tool);
                return;
            }

            cmd.Execute(args, tool);
        }

        // ───────────────────── Piping Support ─────────────────────

        /// <summary>
        /// Executes a command in capture mode and returns its output.
        /// </summary>
        private string CaptureOutput(string command, ShellContext tool)
        {
            tool.CaptureMode = true;
            ExecuteSingle(command, tool);
            tool.CaptureMode = false;

            return tool.FlushPipeBuffer();
        }

        /// <summary>
        /// Executes a command with piped input.
        /// Enforces pipe acceptance and destructive safety.
        /// </summary>
        private void ExecutePiped(string command, string input, ShellContext tool)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string name = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (!commands.TryGetValue(name, out var cmd))
            {
                tool.WriteLine($"Command '{name}' not recognized.");
                return;
            }

            if (!cmd.AcceptsPipeInput)
            {
                tool.WriteLineColored($"Command '{name}' does not accept piped input.", "Red");
                return;
            }

            if (cmd.IsDestructive && !tool.IsSudo)
            {
                tool.WriteLineColored($"'{name}' is destructive and requires sudo when piped.", "Red");
                return;
            }

            cmd.ExecutePipe(input, args, tool);
        }

        // ───────────────────── Sudo Execution ─────────────────────

        /// <summary>
        /// Executes a command after sudo authentication.
        /// </summary>
        public void ExecuteSudo(string name, string[] args, ShellContext tool)
        {
            name = name.ToLower();

            if (!commands.TryGetValue(name, out var command))
            {
                tool.WriteLine($"Command '{name}' not recognized.");
                return;
            }

            command.Execute(args, tool);
        }

        // ───────────────────── Help / Man Output ─────────────────────

        /// <summary>
        /// Displays detailed help for a single command.
        /// </summary>
        public void ShowCommandHelp(ICommand cmd, ShellContext tool)
        {
            tool.WriteLine($"Command: {cmd.Name}");
            tool.WriteLine($"Description: {cmd.Description}");

            if (cmd.Aliases.Length > 0)
                tool.WriteLine($"Aliases: {string.Join(", ", cmd.Aliases)}");

            tool.WriteLine("");
            tool.WriteLine($"Accepts pipe input: {(cmd.AcceptsPipeInput ? "yes" : "no")}");
            tool.WriteLine($"Destructive: {(cmd.IsDestructive ? "yes" : "no")}");

            tool.WriteLine("");
            tool.WriteLine("  " + cmd.Usage + "\n");
        }

        /// <summary>
        /// Attempts to resolve a command by name or alias.
        /// </summary>
        public bool TryGet(string name, out ICommand cmd)
        {
            return commands.TryGetValue(name, out cmd);
        }

        /// <summary>
        /// Compact help listing (used by `help`).
        /// </summary>
        public void ShowHelp(ShellContext tool)
        {
            tool.WriteLine("Aera Command Manual:");

            var uniqueCommands = orderedCommands
                .GroupBy(c => c.Name)
                .Select(g => g.First());

            const int nameWidth = 8;
            const int aliasWidth = 26;

            foreach (var cmd in uniqueCommands)
            {
                string aliases = cmd.Aliases.Length > 0
                    ? $"({string.Join(", ", cmd.Aliases)})"
                    : "";

                tool.WriteLine(
                    $"  {cmd.Name,-nameWidth} {aliases,-aliasWidth} {cmd.Description} {cmd.Usage}"
                );
            }

            tool.WriteLine("");
            tool.WriteLine("Use `man <command>` for detailed help.");
        }

        /// <summary>
        /// Verbose help listing (legacy / debug view).
        /// </summary>
        public void ShowAll(ShellContext tool)
        {
            tool.WriteLine("Aera Command Manual:");

            foreach (var cmd in orderedCommands)
            {
                string aliasText = cmd.Aliases.Length > 0
                    ? $" ({string.Join(", ", cmd.Aliases)})"
                    : "";

                string danger = cmd.IsDestructive ? " !" : "";

                tool.WriteLine($"  {cmd.Name,-12} {cmd.Description}{aliasText}{danger}");
                tool.WriteLine("  " + cmd.Usage + "\n");
            }
        }
    }
}
