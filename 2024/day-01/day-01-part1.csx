#!/usr/bin/env dotnet-script
#nullable enable

using System.Linq;

const int MAX_IDS = 1024;

string inputPath = Args.FirstOrDefault() ?? "input.txt";

var l1 = new int[MAX_IDS];
var l2 = new int[MAX_IDS];

void InsertInOrder(int[] arr, int value, int length) {
    if (length == 0) {
        arr[length] = value;
        return;
    }

    int start = 0;
    int end = length - 1;

    int mid = 0;
    while (end > start) {
        mid = ((end - start) / 2) + start;
        if (value > arr[mid]) {
            start = Math.Min(mid + 1, length - 1);
        }
        else if (value < arr[mid]) {
            end = Math.Max(0, mid - 1);
        }
        else {
            end = start = mid;
        }
    }
    mid = ((end - start) / 2) + start;

    if (value > arr[mid]) {
        mid++;
    }

    arr.AsSpan()[mid..length].CopyTo(arr.AsSpan()[(mid + 1)..(length + 1)]);

    arr[mid] = value;
}
using (StreamReader file = new(inputPath)) {
    string? line;
    int lineNumber = 0;
    while ((line = file.ReadLine()) != null) {
        int[] numbers = line.Split(separator: ' ',
                                   count: 2,
                                   options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(int.Parse)
                            .ToArray();
        InsertInOrder(l1, numbers[0], lineNumber);
        InsertInOrder(l2, numbers[1], lineNumber);
        lineNumber++;
    }
    int length = lineNumber;
    long sum = 0;
    for (int i = 0; i < length; i++) {
        sum += Math.Abs(l1[i] - l2[i]);
    }
    WriteLine($"total distance between lists: {sum}");
}
