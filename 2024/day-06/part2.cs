#!/usr/bin/env dotnet

#:property LangVersion=preview

using System.Text;
using Vector = Point;

var DEBUG = Environment.GetEnvironmentVariable("DEBUG")?.ToUpper() == "TRUE";
var DEBUG_DELAY = int.TryParse(Environment.GetEnvironmentVariable("DEBUG_DELAY"), out var delay) ? delay : 100;

Dictionary<char, TileType> TileMapping = new() {
    { '.', TileType.EMPTY             },
    { '#', TileType.OBSTRUCTION       },
    { '^', TileType.GUARD_FACING_UP   },
    { '>', TileType.GUARD_FACING_RIGH },
    { 'v', TileType.GUARD_FACING_DOWN },
    { '<', TileType.GUARD_FACING_LEFT },
    { 'X', TileType.PATROL_PATH       },
    { 'O', TileType.NEW_LOOP_OBSTACLE },
};

var ReverseTileMapping = TileMapping.ToDictionary(kv => kv.Value, kv => kv.Key);

var filePath = args.FirstOrDefault() ?? "input.txt";
var fileFallbackPath = "input.example.txt";

if (!File.Exists(filePath)) {
    Console.Error.WriteLine($"File {filePath} does not exist, falling back to {fileFallbackPath}");
    filePath = fileFallbackPath;
    if (!File.Exists(filePath)) {
        Console.Error.WriteLine($"File {filePath} does not exist");
        Environment.Exit(1);
    }
}

var text = File.ReadAllText(filePath, Encoding.UTF8);
var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
List<List<TileType>> labList = [];
Guard firstGuard = new();
for (int i = 0; i < lines.Length; i++) {
    var line = lines[i];
    List<TileType> labLine = [];
    for (int j = 0; j < line.Length; j++) {
        labLine.Add(TileMapping[line[j]]);
        if (labLine[j] is TileType.GUARD_FACING_UP
                      or TileType.GUARD_FACING_RIGH
                      or TileType.GUARD_FACING_DOWN
                      or TileType.GUARD_FACING_LEFT) {
            firstGuard = new(new(i, j), labLine[j]);
        }
    }
    labList.Add(labLine);
}
TileType[,] lab = new TileType[labList.Count, labList[0].Count];
for (int i = 0; i < labList.Count; i++) {
    for (int j = 0; j < labList[i].Count; j++) {
        lab[i, j] = labList[i][j];
    }
}

List<Point> obstructionTiles = [];
for (int i = 0; i < lab.GetLength(0); i++) {
    for (int j = 0; j < lab.GetLength(1); j++) {
        if (lab[i, j] is TileType.OBSTRUCTION) {
            obstructionTiles.Add(new(i, j));
        }
    }
}

var path = SimulatePath([firstGuard], obstructionTiles, lab);

// var uniqueWalkedTiles = GetUniqueWalkedTiles(lab, path);
// Console.WriteLine($"The amount of unique tiles the guard walks over is: {uniqueWalkedTiles.Count}");

/* ----------------------------- DEBUG BUT SLOW ----------------------------- */
HashSet<Point> loopTiles = [];
var stopPoint = StopPoint(path.Last(), obstructionTiles, lab);
var dir = GetMovementVector(path.Last());
stopPoint = new(stopPoint.x + dir.x, stopPoint.y + dir.y); // make stop out of bounds
path.Add(path.Last() with { pos = stopPoint });
for (int i = path.Count - 1; i > 0; i--) {
    Point[] points = [.. PointsInBetween(path.ElementAt(i).pos, path.ElementAt(i - 1).pos), path.ElementAt(i - 1).pos];
    foreach (var point in points.Except([path.First().pos])) { // can't put obstruction on first position
        var loopObstacle = point;
        var newPath = SimulatePath(path: GetPathBeforePoint(path, loopObstacle),
                                   obstructionTiles: obstructionTiles,
                                   lab: lab,
                                   loopObstacle: loopObstacle);
        if (newPath.SkipLast(1).Contains(newPath.Last())) {
            loopTiles.Add(loopObstacle);
        }
    }
}

/* ---------------------------- FAST BUT NO DEBUG --------------------------- */
// HashSet<Point> loopTiles = [];
// var stopPoint = StopPoint(path.Last(), obstructionTiles, lab);
// var dir = GetMovementVector(path.Last());
// stopPoint = new(stopPoint.x + dir.x, stopPoint.y + dir.y);
// path.Add(path.Last() with { pos = stopPoint });
// // PrintLab(lab, path, clear: true);
// // Console.WriteLine(dir);
// // Thread.Sleep(10000);
// for (int i = path.Count - 1; i > 0; i--) {
//     Point[] points = [path.ElementAt(i - 1).pos, .. PointsInBetween(path.ElementAt(i).pos, path.ElementAt(i - 1).pos)];
//     var tasks = points.Select(point => Task.Run(() => {
//         var loopObstacle = point;
//         var newPath = SimulatePath(path: GetPathBeforePoint(path, loopObstacle),
//                                    obstructionTiles: obstructionTiles,
//                                    lab: lab,
//                                    loopObstacle: loopObstacle);
//         if (newPath.SkipLast(1).Contains(newPath.Last())) {
//             return loopObstacle;
//         }
//         return (Point?)null;
//     })).ToArray();
//     Task.WaitAll(tasks);
//     foreach (var task in tasks) {
//         if (task.Result is not null) {
//             loopTiles.Add(task.Result.Value);
//         }
//     }
// }

PrintLabLoopTiles(lab, loopTiles);

Console.WriteLine($"The amount of unique tiles that generate loops is: {loopTiles.Count}");

/* -------------------------------------------------------------------------- */

#region methods

ICollection<Guard> SimulatePath(ICollection<Guard> path,
                                List<Point> obstructionTiles,
                                TileType[,] lab,
                                Point? loopObstacle = null) {
    if (loopObstacle is not null) {
        obstructionTiles = [.. obstructionTiles, loopObstacle.Value];
    }

    var firstGuard = path.First();
    var guard = path.Last();
    path = [.. path];
    Point? colision;
    while ((colision = CalculateColision(guard, obstructionTiles)) != null) {
        Point stop = StopPoint(guard, obstructionTiles, lab);
        var unitVector = GetMovementVector(guard);
        var newGuard = RotateGuard(guard) with { pos = stop };
        if (path.Contains(newGuard)) { // Loop detected
            path.Add(newGuard);
            if (DEBUG) {
                PrintLab(lab, path, loopObstacle, clear: true);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Loop detected at position {newGuard.pos} by moving from {guard.pos} to {colision.Value}");
                Console.ForegroundColor = color;
                Thread.Sleep(DEBUG_DELAY * 2);
            }
            return path;
        }
        path.Add(newGuard);
        guard = newGuard;
        if (DEBUG) {
            PrintLab(lab, path, loopObstacle, clear: true);
            Thread.Sleep(DEBUG_DELAY);
        }
    }

    return path;
}

ICollection<Guard> GetPathBeforePoint(ICollection<Guard> path, Point point) {
    List<Guard> newPath = [path.First()];
    for (int i = 1; i < path.Count; i++) {
        if (path.ElementAt(i).pos == point) {
            break;
        }
        if (PointsInBetween(path.ElementAt(i - 1).pos, path.ElementAt(i).pos).Contains(point)) {
            break;
        }
        newPath.Add(path.ElementAt(i));
    }
    return newPath;
}

Guard RotateGuard(Guard guard) {
    return guard with {
        facingDirection = guard.facingDirection switch {
            TileType.GUARD_FACING_UP => TileType.GUARD_FACING_RIGH,
            TileType.GUARD_FACING_RIGH => TileType.GUARD_FACING_DOWN,
            TileType.GUARD_FACING_DOWN => TileType.GUARD_FACING_LEFT,
            TileType.GUARD_FACING_LEFT => TileType.GUARD_FACING_UP,
            _ => throw new InvalidOperationException("Tile should be the guard")
        }
    };
}

Point[] PointsInBetween(Point start, Point end) {
    if (start == end) {
        return [];
    }
    else if (start.x == end.x) {
        var distance = Math.Abs(end.y - start.y);
        Point[] arr = new Point[distance - 1];
        var direction = (end.y - start.y) / distance;
        for (int i = 1; i < distance; i++) {
            arr[i - 1] = new(start.x, start.y + i * direction);
        }
        return arr;
    }
    else if (start.y == end.y) {
        var distance = Math.Abs(end.x - start.x);
        Point[] arr = new Point[distance - 1];
        var direction = (end.x - start.x) / distance;
        for (int i = 1; i < distance; i++) {
            arr[i - 1] = new(start.x + i * direction, start.y);
        }
        return arr;
    }
    else {
        Console.Error.WriteLine("Points are not aligned");
        Environment.Exit(1);
        return [];
    }
}


Point? CalculateColision(Guard guard, List<Point> obstructionTiles) {
    var moveDir = GetMovementVector(guard);
    List<Point> possibleColisions = [];
    foreach (var obstruction in obstructionTiles) {
        var x = obstruction.x - guard.pos.x;
        var y = obstruction.y - guard.pos.y;
        if (x != 0 && y != 0) {
            continue;
        }
        Vector unitVector = new(x != 0 ? x / Math.Abs(x) : x,
                                y != 0 ? y / Math.Abs(y) : y);
        if (unitVector != moveDir) {
            continue;
        }
        possibleColisions.Add(obstruction);
    }

    var distance = int.MaxValue;
    Point? obstructionColide = null;
    foreach (var p in possibleColisions) {
        var d = Math.Abs(p.x - guard.pos.x) + Math.Abs(p.y - guard.pos.y);
        if (d < distance) {
            distance = d;
            obstructionColide = p;
        }
    }

    return obstructionColide;
}

Point StopPoint(Guard guard, List<Point> obstructionTiles, TileType[,] lab) {
    var colision = CalculateColision(guard, obstructionTiles);
    var moveDir = GetMovementVector(guard);
    if (colision == null) {
        if (moveDir.x > 0) {
            return new(lab.GetLength(0) - 1, guard.pos.y);
        }
        else if (moveDir.x < 0) {
            return new(0, guard.pos.y);
        }
        else if (moveDir.y > 0) {
            return new(guard.pos.x, lab.GetLength(1) - 1);
        }
        else if (moveDir.y < 0) {
            return new(guard.pos.x, 0);
        }
        else {
            throw new InvalidOperationException("Guard is not moving");
        }
    }
    else {
        return new(colision.Value.x - moveDir.x,
                   colision.Value.y - moveDir.y);
    }
}

Vector GetMovementVector(Guard guard) {
    return new(
        guard.facingDirection switch {
            TileType.GUARD_FACING_RIGH or TileType.GUARD_FACING_LEFT => 0,
            TileType.GUARD_FACING_UP => -1,
            TileType.GUARD_FACING_DOWN => +1,
            _ => throw new InvalidOperationException("Tile should be the guard")
        },
        guard.facingDirection switch {
            TileType.GUARD_FACING_UP or TileType.GUARD_FACING_DOWN => 0,
            TileType.GUARD_FACING_RIGH => +1,
            TileType.GUARD_FACING_LEFT => -1,
            _ => throw new InvalidOperationException("Tile should be the guard")
        }
    );
}

void PrintLab(TileType[,] lab,
              ICollection<Guard>? guards = null,
              Point? loopObstacle = null,
              bool clear = false) {
    HashSet<Vector> uniqueWalkedTiles = GetUniqueWalkedTiles(lab,
                                                             guards,
                                                             loopObstacle);
    var count = 0;
    StringBuilder sb = new();
    for (int i = 0; i < lab.GetLength(0); i++) {
        for (int j = 0; j < lab.GetLength(1); j++) {
            Point printPos = new(i, j);
            if (guards is not null && guards.Last().pos == printPos) {
                sb.Append(ReverseTileMapping[guards.Last().facingDirection]);
            }
            else if (uniqueWalkedTiles.Contains(printPos)) {
                sb.Append('X');
                count++;
            }
            else if (loopObstacle is not null
                     && loopObstacle.Value == printPos) {
                sb.Append('O');
            }
            else {
                sb.Append(ReverseTileMapping[lab[i, j]]);
            }
        }
        sb.AppendLine();
    }
    if (clear) {
        Console.Clear();
    }
    Console.WriteLine(sb.ToString());
    Console.WriteLine();
}

void PrintLabLoopTiles(TileType[,] lab,
                       HashSet<Point> loopTiles) {
    StringBuilder sb = new();
    for (int i = 0; i < lab.GetLength(0); i++) {
        for (int j = 0; j < lab.GetLength(1); j++) {
            Point printPos = new(i, j);
            if (loopTiles.Contains(printPos)) {
                sb.Append('O');
            }
            else {
                sb.Append(ReverseTileMapping[lab[i, j]]);
            }
        }
        sb.AppendLine();
    }
    Console.WriteLine(sb.ToString());
    Console.WriteLine();
}

HashSet<Vector> GetUniqueWalkedTiles(TileType[,] lab,
                                     ICollection<Guard>? guards,
                                     Vector? loopObstacle = null) {
    HashSet<Point> uniqueWalkedTiles = [];
    if (guards is not null) {
        for (int i = 0; i < guards.Count - 1; i++) {
            var pointsInBetween = PointsInBetween(guards.ElementAt(i).pos,
                                                  guards.ElementAt(i + 1).pos);
            foreach (var p in pointsInBetween) {
                uniqueWalkedTiles.Add(p);
            }
            uniqueWalkedTiles.Add(guards.ElementAt(i).pos);
        }
        uniqueWalkedTiles.Add(guards.Last().pos);
        List<Point> obstructionTiles = [];
        for (int i = 0; i < lab.GetLength(0); i++) {
            for (int j = 0; j < lab.GetLength(1); j++) {
                if (lab[i, j] is TileType.OBSTRUCTION) {
                    obstructionTiles.Add(new(i, j));
                }
            }
        }
        if (loopObstacle is not null) {
            obstructionTiles.Add(loopObstacle.Value);
        }
        var colision = CalculateColision(guards.Last(), obstructionTiles);
        if (colision is null) {
            var wallPoint = StopPoint(guards.Last(), obstructionTiles, lab);
            var unitVector = GetMovementVector(guards.Last());
            wallPoint = new(
                wallPoint.x + unitVector.x,
                wallPoint.y + unitVector.y
            );
            var pointsInBetweenLast = PointsInBetween(guards.Last().pos,
                                                      wallPoint);
            foreach (var p in pointsInBetweenLast) {
                uniqueWalkedTiles.Add(p);
            }
        }
        else {
            var pointsInBetweenLast = PointsInBetween(guards.Last().pos,
                                                      colision.Value);
            foreach (var p in pointsInBetweenLast) {
                uniqueWalkedTiles.Add(p);
            }
        }
    }

    return uniqueWalkedTiles;
}

#endregion

/* -------------------------------------------------------------------------- */

#region types

enum TileType {
    EMPTY,
    OBSTRUCTION,
    GUARD_FACING_UP,
    GUARD_FACING_RIGH,
    GUARD_FACING_DOWN,
    GUARD_FACING_LEFT,
    PATROL_PATH,
    NEW_LOOP_OBSTACLE,
}

record struct Point(int x, int y);

record struct Guard(Point pos, TileType facingDirection);

#endregion types
