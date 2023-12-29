# Advent of Code

Doing some Advent of Code puzzles in different languages to remember or learn
them.

## Instructions

### Download input

Download puzzle input and save it to `input.txt` in the challenge day directory.

Or use the command line with `curl`:

```bash
curl https://adventofcode.com/2023/day/1/input \
  --cookie "session=$SESSION_ID" \
  --output ./2023/day-01/input.txt
```

### Execute script file in Visual Studio Code

Open the script file in Visual Studio Code, then open the Command Palette
(default: `Ctrl+Shift+P`) and select `Run build task` (default: `Ctrl+Shift+B`),
then select the task to run.

### Run solution in C

```bash
gcc day-01.c -o day-01
./day-01
```

### Run solution in Lua

```bash
lua day-01.lua
```

### Run solution in C# (dotnet-script)

```bash
dotnet script day-01.csx
```

### Run solution in Python

```bash
python day-01.py
```

### Run solution in Rust

```bash
rustc day-01.rs -o day-01
./day-01
```

### Run solution in Go

```bash
go run day-01.go
```

### Run solution in JavaScript

```bash
node day-01.js
```
