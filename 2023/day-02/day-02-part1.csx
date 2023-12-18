#!/usr/bin/env dotnet-script
#nullable enable

const int MAX_RED = 12;
const int MAX_GREEN = 13;
const int MAX_BLUE = 14;

(int lineNumber, bool valid) IsGameValid(
    ReadOnlySpan<char> line,
    int maxRed,
    int maxBlue,
    int maxGreen)
{
    var colonIndex = line.IndexOf(":");
    var lineNumber = int.Parse(line[5..colonIndex]);
    var setsStr = line[(colonIndex + 1)..];
    Span<Range> setsRanges = stackalloc Range[10];
    var setsLength = setsStr.Split(setsRanges, ';');
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
            var isValid = color[0] switch
            {
                'r' => number <= maxRed,
                'g' => number <= maxGreen,
                'b' => number <= maxBlue,
                _ => default
            };
            if (!isValid)
            {
                return (lineNumber, false);
            }
        }
    }
    return (lineNumber, true);
}

string inputPath = Args.FirstOrDefault() ?? "input.txt";
using (StreamReader file = new(inputPath))
{
    string? line;
    int sum = 0;
    while ((line = file.ReadLine()) != null)
    {
        var (lineNumber, isValid) = IsGameValid(
            line: line.AsSpan(),
            maxRed: MAX_RED,
            maxBlue: MAX_BLUE,
            maxGreen: MAX_GREEN);

        if (isValid)
        {
            sum += lineNumber;
        }
    }

    Print($"Sum of valid games ids: {sum}");
}
