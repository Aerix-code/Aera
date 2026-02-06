using System;
// Copy this file to create new commands.
// Do not modify structure unless ICommand changes.

namespace Aera
{
    internal class ExampleCommand : ICommand
    {
        public string Name => "example"; // what to enter to run the command
        public string Description => "Used as an example and base"; // description of what the command does
        public string Usage => "Usage: example"; // describes how to use the command

        public bool AcceptsPipeInput => false; // wether it accepts piping (multiple commands run on 1 line)
        public bool IsDestructive => false; // wether the command is destructive

        public string[] Aliases => new[] { "examp", "exmpl" }; // other strings to run the same command

        public void Execute(string[] args, ShellContext tool)
        {
            tool.WriteLineColored("THIS IS AN EXAMPLE COMMAND", "yellow"); // this is where you put the code of your command
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("example: cannot be used in a pipe", "Red"); // error message for when you try to use a non pipeable command with a pipe
        }
    }
}