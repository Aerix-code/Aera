# Aera

**Aera** is a custom C# command-line shell designed to feel UNIX-like while enforcing **clear UX rules and safety guarantees**.

It is not a system shell replacement.  
It is a **controlled CLI environment** with explicit behavior, predictable pipelines, and guardrails around destructive actions.

---

## ✨ Goals

- UNIX-style commands and pipelines
- Clear, consistent command UX
- Centralized help and documentation
- Explicit safety for destructive operations
- Clean architecture for adding new commands

Aera prioritizes **clarity over cleverness**.

---

## 🧱 Core Features

### Command System
- Command registration with aliases
- Centralized command manager
- Automatic `--help` for every command
- Unified `help` / `man` system
- Ordered command listing

### Piping
- UNIX-style `|` pipelines
- Explicit pipe eligibility per command
- Pipe safety enforced by the manager
- No silent data loss
- Commands must consume, transform, or forward input

### Safety Model
- Destructive commands are explicitly marked
- Destructive commands require `sudo`
- Destructive commands **cannot be piped** without sudo
- Centralized confirmation prompts

### Sudo
- `sudo <command>` execution model
- Per-invocation authentication
- Clear privilege boundary
- No hidden elevation

---

## 🏗 Architecture Overview

### `ICommand`

Every command implements:

```csharp
string Name
string Description
string Usage
string[] Aliases
bool AcceptsPipeInput
bool IsDestructive
```
Rules:

Commands never print help manually

Commands never access Console directly

Commands never manage pipe capture themselves

ShellContext

The only I/O surface available to commands.

Responsibilities:

Console output (with color support)

Input reading

Pipe buffering (manager-controlled)

User identity & credentials

Sudo authentication

Confirmation prompts

Commands interact with the shell only through this context.

CommandManager

Responsible for:

Registering commands and aliases

Executing commands and pipelines

Enforcing pipe rules

Enforcing destructive + sudo rules

Handling sudo execution

Rendering help and man output

This is where policy lives, not in commands.

📚 Built-in Commands
Core / Meta

help, man — list commands or show manuals

exit, close, shutdown — terminate the CLI

clear — clear the console

Identity / Privilege

whoami — show current user

userinfo — show user info (password masked unless sudo)

sudo — run a command with elevated privileges

Filesystem: Navigation

pwd — print working directory

cd — change directory

Filesystem: Inspection

ls — list directory contents

tree — recursive directory view

cat — output file contents

grep — search for matching lines

wc — count lines, words, characters

Filesystem: Mutation

touch — create files / update timestamps

mkdir — create directories

rm — delete files or directories (destructive)

cp — copy files or directories

mv — move or rename files

Utilities

date — show current date

time — show current time

Output / Piping

echo — write text to output

write — formatted output helper

hello — prints a random greeting

🔗 Pipeline Examples
ls | grep .cs
cat file.txt | grep error | wc
echo hello world | wc -w
ls | grep .cs | wc -l


Pipeline rules:

Commands must explicitly accept pipe input

Commands may consume, transform, or forward input

Pipe capture is handled centrally

🔒 Safety Examples
rm file.txt
# permission denied (sudo required)

sudo rm file.txt
# confirmation prompt → file removed

echo test | rm file.txt
# rejected (destructive command in pipe)

🛠 Adding a New Command

Copy an existing command

Implement ICommand

Register it in the CommandManager

Done

No hidden wiring. No magic.

🚧 Non-Goals

Full-screen editors (nano, vim)

External process execution

Networking tools

Package management

These are intentionally out of scope.

## License

MIT (or update as appropriate)

🧠 Philosophy

A shell should never surprise you.

Aera is built to make command behavior explicit, predictable, and safe, while still feeling familiar to anyone used to UNIX-like environments.

void Execute(string[] args, ShellContext context)
void ExecutePipe(string input, string[] args, ShellContext context)
