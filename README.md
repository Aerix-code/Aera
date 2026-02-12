# Aera

Aera is a modular C# command-line shell that replicates and extends common Unix-style commands. It is designed as a learning project focused on command architecture, piping support, and extensibility.

>  Experimental project. APIs and behaviour may change.

---

## Overview

Aera provides a lightweight CLI environment implemented in **.NET** with a plugin-style command system. Each command is implemented as a separate class that follows a shared interface, making it easy to expand functionality.

The project focuses on:

- Command abstraction
- Pipe input support
- Shell context handling
- Unix-like command experience
- Expandable architecture

---

## Features

### Built-in Commands

#### File & Directory
- `ls` – List directory contents  
- `cd` – Change directory  
- `pwd` – Print working directory  
- `mkdir` – Create directories  
- `rm` – Remove files or directories  
- `mv` – Move or rename files  
- `cp` – Copy files  
- `touch` – Create files  
- `tree` – Display directory structure  
- `stat` – File information  
- `du` – Disk usage
- `nano` - File editing

#### File Viewing & Processing
- `cat` – Output file contents  
- `head` – Show beginning of file  
- `tail` – Show end of file  
- `sort` – Sort text input  
- `uniq` – Remove duplicate lines  
- `wc` – Count lines, words, characters  
- `grep` – Search text patterns  
- `find` – Search for files  

#### Shell Utilities
- `echo` – Print text  
- `clear` – Clear console  
- `help` – Display command help  
- `exit` – Exit shell  
- `time` – Measure execution time  
- `which` – Locate commands  
- `env` – Show environment variables  
- `userinfo` – Display user information  
- `write` – Write text to files  

#### System / Misc
- `sudo` – Elevated command execution (simulated)  
- `fastfetch` – System information  
- `hello` – Example greeting command  
- `example` – Template/example command  

---

## Architecture

### Command System

All commands implement:

ICommand


This interface defines:

- Command name
- Description
- Usage
- Alias support
- Pipe input capability
- Execution behaviour

---

### Core Components

#### CommandManager
Responsible for:

- Command registration
- Command lookup
- Execution dispatching

#### ShellContext
Provides runtime context including:

- Current directory
- Environment data
- Shared shell utilities
- Pipe input/output handling

#### Program.cs
Acts as:

- Shell entry point
- Input loop handler
- Command parser
- Execution coordinator

---

## Requirements

- .NET SDK (Target Framework: .NET 10.0)
- Windows, Linux, or macOS

---

## Building

Clone the repository:

git clone https://github.com/Aerix-code/Aera.git
cd Aera

Build the project:

dotnet build

Run the shell:

dotnet run --project Aera

Usage

Once running, type commands similarly to a standard shell:

ls
cd folder
cat file.txt
grep "text" file.txt


Pipe Support

Many commands accept piped input:

cat file.txt | sort | uniq

Creating Custom Commands

Create a new class implementing ICommand.

Example:

internal class MyCommand : ICommand
{
    public string Name => "mycommand";
    public string Description => "Example command";
    public string Usage => "mycommand";

    public bool AcceptsPipeInput => false;
    public bool IsDestructive => false;

    public string[] Aliases => [];

    public void Execute(string[] args, ShellContext context)
    {
        Console.WriteLine("My custom command");
    }
}


Register it inside Program

Project Structure
Aera/
 ├── Program.cs
 ├── CommandManager.cs
 ├── ShellContext.cs
 ├── ICommand.cs
 ├── *Command.cs
 └── Aera.csproj

Goals

Provide a learning environment for shell design

Explore command parsing and pipelines

Practice scalable CLI architecture

Experiment with system utilities

Limitations

Not intended as a full production shell

Some commands may differ from Unix behaviour

Permission handling is simplified

Contributing

Contributions, ideas, and improvements are welcome.

Steps:

Fork repository

Create feature branch

Submit pull request

License

No license currently specified.

Auther:
https://github.com/Aerix-code
