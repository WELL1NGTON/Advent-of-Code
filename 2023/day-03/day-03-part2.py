#!/usr/bin/env python3

import sys


class Range:
    """
    A range of characters in a string.

    Attributes:
    line_ref (str): The string that contains the range.
    start (int): The start index of the range.
    end (int): The end index of the range. (inclusive)
    """

    def __init__(self, line_ref: str, start: int, end: int):
        self.line_ref = line_ref
        self.start = start
        self.end = end  # end is inclusive

    def __str__(self):
        return self.line_ref[self.start : self.end + 1]


def get_number_range(line: str | None, pos: int) -> Range | None:
    """
    Get the range of digits in the given line at the given position.

    Parameters:
    line (str | None): The line to check.
    pos (int): The position to check.

    Returns:
    Range | None: The range of digits in the given line at the given position.
    """
    if not line or pos < 0 or pos >= len(line) or not line[pos].isdigit():
        return None
    range = Range(line, pos, pos)
    updated = True
    while updated:
        updated = False
        if range.start > 0 and line[range.start - 1].isdigit():
            updated = True
            range.start = range.start - 1
        if range.end + 1 < len(line) and line[range.end + 1].isdigit():
            updated = True
            range.end = range.end + 1
    return range


def get_ranges(line: str | None, pos: int) -> list[Range]:
    """
    Get the ranges of digits in the given line at the given position and the
    adjacent positions.

    Parameters:
    line (str | None): The line to check.
    pos (int): The position to check.

    Returns:
    list[Range]: The ranges of digits in the given line at the given position
    and the adjacent positions.
    """
    ranges: list[Range] = []
    for i in [pos - 1, pos, pos + 1]:
        if ranges and ranges[-1].end >= i:
            continue
        range = get_number_range(line, i)
        if range:
            ranges.append(range)
    return ranges


def get_gear_ratio(
    top_line: str | None, middle_line: str, bottom_line: str | None, pos: int
) -> int:
    """
    Get the gear ratio at the given position.

    Parameters:
    top_line (str | None): The top line.
    middle_line (str): The middle line.
    bottom_line (str | None): The bottom line.
    pos (int): The position to check.

    Returns:
    int: The gear ratio at the given position.
    """
    if middle_line[pos] != "*":
        return 0
    ranges: list[Range] = []
    for line in [top_line, middle_line, bottom_line]:
        ranges.extend(get_ranges(line, pos))
    if len(ranges) != 2:
        return 0
    return int(str(ranges[0])) * int(str(ranges[1]))


def sum_gears_in_line(
    top_line: str | None, middle_line: str, bottom_line: str | None
) -> int:
    """
    Sum all of the gear ratios in the given line.

    Parameters:
    top_line (str | None): The top line.
    middle_line (str): The middle line.
    bottom_line (str | None): The bottom line.

    Returns:
    int: The sum of all of the gear ratios in the given line.
    """
    if not middle_line:
        return 0
    sum = 0
    length = len(middle_line)
    for i in range(length):
        sum = sum + get_gear_ratio(top_line, middle_line, bottom_line, i)
    return sum


if __name__ == "__main__":
    # get file path from command line or default to input.txt
    if len(sys.argv) < 2:
        file_path = "input.txt"
    else:
        file_path = sys.argv[1]

    # read file and sum all part numbers
    with open(file_path, "r") as f:
        sum = 0
        top_line: str | None = None
        middle_line: str = f.readline().strip()
        bottom_line: str | None = f.readline().strip()
        while middle_line:
            sum = sum + sum_gears_in_line(top_line, middle_line, bottom_line)
            top_line = middle_line
            middle_line = bottom_line
            bottom_line = f.readline().strip()
        print(f"The sum of all of the gear ratios is {sum}")
