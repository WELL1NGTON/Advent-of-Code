#!/usr/bin/env dotnet-script
#nullable enable

(int gameId, int minRed, int minGreen, int minBlue) IsGameValid(ReadOnlySpan<char> line)
{
    var colonIndex = line.IndexOf(":");
    var gameId = int.Parse(line[5..colonIndex]);
    var setsStr = line[(colonIndex + 1)..];
    Span<Range> setsRanges = stackalloc Range[10];
    var setsLength = setsStr.Split(setsRanges, ';');
    var minRed = 0;
    var minGreen = 0;
    var minBlue = 0;
    for (int i = 0; i < setsLength; i++)
    {
        var set = setsStr[setsRanges[i]];
        Span<Range> colorsRanges = stackalloc Range[3];
        var colorsLength = set.Split(
            destination: colorsRanges,
            separator: ',',
            options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < colorsLength; j++)
        {
            var colorStr = set[colorsRanges[j]];
            if (colorStr.Length == 0)
            {
                continue;
            }
            var separatorIndex = colorStr.IndexOf(' ');
            var number = int.Parse(colorStr[..separatorIndex]);
            separatorIndex++;
            var color = colorStr[separatorIndex..];
            _ = color[0] switch
            {
                'r' => minRed = Math.Max(minRed, number),
                'g' => minGreen = Math.Max(minGreen, number),
                'b' => minBlue = Math.Max(minBlue, number),
                _ => default
            };
        }
    }
    return (gameId, minRed, minGreen, minBlue);
}

string inputPath = Args.FirstOrDefault() ?? "input.txt";
using (StreamReader file = new(inputPath))
{
    string? line;
    int sum = 0;
    while ((line = file.ReadLine()) != null)
    {
        var (lineNumber, minRed, minGreen, minBlue) = IsGameValid(line.AsSpan());

        sum += minRed * minGreen * minBlue;
    }

    Print($"The sum of the power of these sets is: {sum}");
}
