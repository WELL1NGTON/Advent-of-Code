#!/usr/bin/env dotnet

#:property LangVersion=preview

#:package Microsoft.Extensions.Logging.Console@9.0.9

using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console; // not necessary, just testing imports on new c# file based apps

var DEBUG = Environment.GetEnvironmentVariable("DEBUG")?.ToUpper() == "TRUE";

var logger = LoggerFactory.Create(builder => {
    builder.AddSimpleConsole(options => {
        options.IncludeScopes = false;
        options.SingleLine = true;
        options.TimestampFormat = "hh:mm:ss ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    });

    // Ensure immediate flush
    builder.Configure(options => {
        options.ActivityTrackingOptions = ActivityTrackingOptions.None;
    });

    // Force synchronous console logging
    builder.AddConsole();
    builder.AddFilter("Microsoft", LogLevel.Information);
}).CreateLogger("[Day06 - Part1]");

var fileName = args.FirstOrDefault() ?? "input.txt";

if (!File.Exists(fileName)) {
    // Console.Error.WriteLine($"File {fileName} does not exist");
    // Just testing nugets download, there is no need for logger...
    logger.LogError("File {FileName} does not exist", fileName);
    Console.Error.Flush();
    Thread.Sleep(100); // Give time for the logger to flush
    Environment.Exit(1);
}

Dictionary<char, Tile> TileMapping = new() {
    { '.', Tile.EMPTY             },
    { '#', Tile.OBSTRUCTION       },
    { '^', Tile.GUARD_FACING_UP   },
    { '>', Tile.GUARD_FACING_RIGH },
    { 'v', Tile.GUARD_FACING_DOWN },
    { '<', Tile.GUARD_FACING_LEFT },
    { 'X', Tile.PATROL_PATH       }
};

Dictionary<Tile, char> ReverseTileMapping = TileMapping.ToDictionary(kv => kv.Value, kv => kv.Key);

Tile[][] lab = [.. File.ReadAllLines(fileName)
    .Where(line => line.Length > 0)
    .Select(line => line.Select(c => TileMapping[c]).ToArray())];

int x = 0;
int y = 0;
for (int i = 0; i < lab.Length; i++) {
    for (int j = 0; j < lab.Length; j++) {
        if (lab[i][j] is Tile.GUARD_FACING_UP
                      or Tile.GUARD_FACING_RIGH
                      or Tile.GUARD_FACING_DOWN
                      or Tile.GUARD_FACING_LEFT) {
            x = i;
            y = j;
        }
    }
}

Console.WriteLine($"The ammount of unique tiles the guard will patrol is: {SimulatePath((x, y), lab)}");

/* -------------------------------------------------------------------------- */

#region Methods

Tile Turn90DegreesClockwise(Tile t) => t switch {
    Tile.GUARD_FACING_UP => Tile.GUARD_FACING_RIGH,
    Tile.GUARD_FACING_RIGH => Tile.GUARD_FACING_DOWN,
    Tile.GUARD_FACING_DOWN => Tile.GUARD_FACING_LEFT,
    Tile.GUARD_FACING_LEFT => Tile.GUARD_FACING_UP,
    _ => t
};

int SimulatePath((int x, int y) pos, Tile[][] lab) {
    var count = 1;

    Tile guardState;
    (int x, int y) direction;
    while (true) {
        if (DEBUG) {
            Console.Clear();
            Console.WriteLine($"Current position: ({pos.x}, {pos.y})");
            Console.WriteLine($"count: {count}");
            PrintLab(lab);
            Thread.Sleep(25);
        }
        guardState = lab[pos.x][pos.y];
        direction = Direction(guardState);

        lab[pos.x][pos.y] = Tile.PATROL_PATH;

        if (!IsInside((pos.x + direction.x, pos.y + direction.y), lab)) {
            break;
        }

        while (lab[pos.x + direction.x][pos.y + direction.y] is Tile.OBSTRUCTION) {
            guardState = Turn90DegreesClockwise(guardState);
            direction = Direction(guardState);
        }

        pos.x += direction.x;
        pos.y += direction.y;

        if (lab[pos.x][pos.y] is not Tile.PATROL_PATH) {
            count++;
        }

        lab[pos.x][pos.y] = guardState;
    }

    return count;
}

bool IsInside((int x, int y) pos, Tile[][] lab) {
    return pos.x >= 0 && pos.x < lab.Length
        && pos.y >= 0 && pos.y < lab[0].Length;
}

(int x, int y) Direction(Tile state) {
    return (
        state switch {
            Tile.GUARD_FACING_RIGH or Tile.GUARD_FACING_LEFT => 0,
            Tile.GUARD_FACING_UP => -1,
            Tile.GUARD_FACING_DOWN => +1,
            _ => throw new InvalidOperationException("Tile should be the guard")
        },
        state switch {
            Tile.GUARD_FACING_UP or Tile.GUARD_FACING_DOWN => 0,
            Tile.GUARD_FACING_RIGH => +1,
            Tile.GUARD_FACING_LEFT => -1,
            _ => throw new InvalidOperationException("Tile should be the guard")
        }
    );
}

void PrintLab(Tile[][] lab) {
    var sb = new StringBuilder();
    foreach (var line in lab) {
        foreach (var tile in line) {
            sb.Append(ReverseTileMapping[tile]);
        }
        sb.AppendLine();
    }
    Console.WriteLine(sb.ToString());
    Console.WriteLine();
}

#endregion Methods

/* -------------------------------------------------------------------------- */

#region Types

enum Tile {
    EMPTY,
    OBSTRUCTION,
    GUARD_FACING_UP,
    GUARD_FACING_RIGH,
    GUARD_FACING_DOWN,
    GUARD_FACING_LEFT,
    PATROL_PATH,
}

#endregion Types
