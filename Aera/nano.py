import curses
import curses.ascii
import sys
import os

if len(sys.argv) < 2:
    print("Usage: nano.py <file>")
    sys.exit(1)

file_path = sys.argv[1]


# -------------------------
# File Handling
# -------------------------
def load_file():
    if os.path.exists(file_path):
        with open(file_path, "r", encoding="utf-8") as f:
            lines = f.read().splitlines()
            return lines if lines else [""]
    return [""]


def save_file(lines):
    with open(file_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


# -------------------------
# Cursor Safety
# -------------------------
def clamp_cursor(lines, y, x):
    if not lines:
        lines.append("")

    y = max(0, min(y, len(lines) - 1))
    x = max(0, min(x, len(lines[y])))
    return y, x


# -------------------------
# Editor
# -------------------------
def editor(stdscr):
    curses.curs_set(1)
    stdscr.clear()
    stdscr.keypad(True)

    lines = load_file()

    y = 0
    x = 0

    while True:
        y, x = clamp_cursor(lines, y, x)

        stdscr.clear()
        h, w = stdscr.getmaxyx()

        # Draw text buffer
        for idx, line in enumerate(lines[:h - 1]):
            try:
                stdscr.addstr(idx, 0, line[: w - 1])
            except curses.error:
                pass

        # Status bar
        status = f"{os.path.basename(file_path)} | Ctrl+O Save | Ctrl+X Exit"
        stdscr.addstr(h - 1, 0, status[: w - 1], curses.A_REVERSE)

        stdscr.move(y, x)
        stdscr.refresh()

        key = stdscr.getch()

        # -----------------
        # Ctrl+X Exit
        # -----------------
        if key == curses.ascii.CAN:  # Ctrl+X
            save_file(lines)
            break

        # -----------------
        # Ctrl+O Save
        # -----------------
        elif key == curses.ascii.SI:  # Ctrl+O
            save_file(lines)

        # -----------------
        # Enter
        # -----------------
        elif key in (10, 13):
            current = lines[y]
            lines[y] = current[:x]
            lines.insert(y + 1, current[x:])
            y += 1
            x = 0

        # -----------------
        # Backspace
        # -----------------
        elif key in (8, 127, curses.KEY_BACKSPACE):
            if x > 0:
                line = lines[y]
                lines[y] = line[: x - 1] + line[x:]
                x -= 1
            elif y > 0:
                prev_len = len(lines[y - 1])
                lines[y - 1] += lines[y]
                lines.pop(y)
                y -= 1
                x = prev_len

        # -----------------
        # Arrow Keys
        # -----------------
        elif key == curses.KEY_UP:
            y -= 1

        elif key == curses.KEY_DOWN:
            y += 1

        elif key == curses.KEY_LEFT:
            x -= 1

        elif key == curses.KEY_RIGHT:
            x += 1

        # -----------------
        # Printable Characters
        # -----------------
        elif 32 <= key <= 126:
            line = lines[y]
            lines[y] = line[:x] + chr(key) + line[x:]
            x += 1


# -------------------------
# Program Entry
# -------------------------
if __name__ == "__main__":
    curses.wrapper(editor)