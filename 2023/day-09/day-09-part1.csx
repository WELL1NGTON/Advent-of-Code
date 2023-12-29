#!/usr/bin/env dotnet-script
#nullable enable

const int MAX_NUMBERS = 25;

string inputPath = Args.FirstOrDefault() ?? "input.txt";

long[,] numbers = new long[MAX_NUMBERS + 1, MAX_NUMBERS + 1];
using (StreamReader file = new(inputPath)) {
    string? line;
    long sum = 0;
    while ((line = file.ReadLine()) != null) {
        var qtyNums = 0;
        var nums = line
            .Split(separator: " ")
            .Select((strNum, i) => (long.Parse(strNum), i));
        foreach ((var num, var j) in nums) {
            numbers[0, j] = num;
            qtyNums = j;
        }
        var allZeroesLine = 1;
        bool allZeroes = false;
        for (int i = 1; i < qtyNums && !allZeroes; i++) {
            allZeroes = true;
            for (int j = 0; j < qtyNums - i + 1; j++) {
                numbers[i, j] = numbers[i - 1, j + 1] - numbers[i - 1, j];
                if (numbers[i, j] != 0) {
                    allZeroes = false;
                }
            }

            if (allZeroes) {
                allZeroesLine = i;
                numbers[i, qtyNums - i + 1] = 0;
            }
        }

        for (int i = allZeroesLine - 1; i >= 0; i--) {
            numbers[i, qtyNums - i + 1] = numbers[i, qtyNums - i]
                                        + numbers[i + 1, qtyNums - i];
        }

        sum += numbers[0, qtyNums + 1];
    }

    Print($"The sum of these extrapolated values is {sum}");
}
