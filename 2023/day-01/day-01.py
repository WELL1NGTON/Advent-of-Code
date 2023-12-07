#!/usr/bin/env python3

import sys


valid_digits = [
    "one",
    "two",
    "three",
    "four",
    "five",
    "six",
    "seven",
    "eight",
    "nine",
]


def get_digit_value(str: str) -> int:
    if str[0] >= '0' and str[0] <= '9':
        return int(str[0])
    for i in range(9):
        if str.startswith(valid_digits[i]):
            return i + 1
    return -1


if len(sys.argv) < 2:
    file_path = "input.txt"
else:
    file_path = sys.argv[1]

sum = 0
with open(file_path, "r") as f:
    line = f.readline()
    while line:
        digit1_found = False
        digit2_found = False
        digit1 = 0
        digit2 = 0
        for i in range(len(line)):
            if not digit1_found:
                digit1 = get_digit_value(line[i:])
                digit1_found = digit1 != -1
            if not digit2_found:
                digit2 = get_digit_value(line[len(line) - i - 1 :])
                digit2_found = digit2 != -1
            if digit1_found and digit2_found:
                break
        sum += digit1 * 10 + digit2
        line = f.readline()

print(f'The sum of all of the calibration values is {sum}')
