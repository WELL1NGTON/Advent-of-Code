#!/usr/bin/env dotnet-script
#nullable enable

string[] validDigits = [
    "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

int GetDigitValue(ReadOnlySpan<char> span) {
    if (char.IsDigit(span[0])) {
        return span[0] - '0';
    }
    for (int i = 0; i < validDigits.Length; i++) {
        if (span.StartsWith(validDigits[i].AsSpan())) {
            return i + 1;
        }
    }
    return -1;
}

string inputPath = Args.FirstOrDefault() ?? "input.txt";
int sum = 0;

using (StreamReader file = new(inputPath)) {
    string? line;
    while ((line = file.ReadLine()) != null) {
        bool digit1Found = false;
        bool digit2Found = false;
        int digit1 = 0;
        int digit2 = 0;
        for (int i = 0; i < line.Length && (!digit1Found || !digit2Found); i++) {
            int digit;
            if (!digit1Found && (digit = GetDigitValue(line.AsSpan(i))) >= 0) {
                digit1Found = true;
                digit1 = digit;
            }
            if (!digit2Found && (digit = GetDigitValue(line.AsSpan(line.Length - i - 1))) >= 0) {
                digit2Found = true;
                digit2 = digit;
            }
        }

        sum += (digit1 * 10) + digit2;
    }
}

WriteLine($"The sum of all of the calibration values is {sum}");
return 0;
