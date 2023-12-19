#!/usr/bin/env python3

import sys


def char_is_symbol(char: str) -> bool:
    """
    Check if the given character is a symbol.

    Parameters:
    char (str): The character to check.

    Returns:
    bool: True if the given character is a symbol (any character that is not a
    digit or a period).
    """
    if char and char not in ".0123456789":
        return True
    return False


def has_symbol_adjacent(
    top: str | None, middle: str, bottom: str | None, i: int
) -> bool:
    """
    Check if there is a symbol adjacent to the given index in the top, middle,
    and bottom lines.

    Parameters:
    top (str | None): The top line.
    middle (str): The middle line.
    bottom (str | None): The bottom line.
    i (int): The index to check.

    Returns:
    bool: True if there is a symbol adjacent to the given index in the top,
    """
    length = len(middle)
    # top left
    return (
        char_is_symbol(top[i - 1] if top and i > 0 else None)
        # top middle
        or char_is_symbol(top[i] if top else None)
        # top right
        or char_is_symbol(top[i + 1] if top and i + 1 < length else None)
        # middle left
        or char_is_symbol(middle[i - 1] if i > 0 else None)
        # middle right
        or char_is_symbol(middle[i + 1] if i + 1 < length else None)
        # bottom left
        or char_is_symbol(bottom[i - 1] if bottom and i > 0 else None)
        # bottom middle
        or char_is_symbol(bottom[i] if bottom else None)
        # bottom right
        or char_is_symbol(bottom[i + 1] if bottom and i + 1 < length else None)
    )


def sum_line(
    top_line: str | None, middle_line: str, bottom_line: str | None
) -> int:
    """
    Sum all of the part numbers in the given line.

    Parameters:
    top_line (str | None): The top line.
    middle_line (str): The middle line.
    bottom_line (str | None): The bottom line.

    Returns:
    int: The sum of all of the part numbers in the given line.
    """
    if not middle_line:
        return 0
    sum = 0
    length = len(middle_line)
    parsing_number = False
    symbol_found = False
    buffer = ""
    for i in range(length):
        if middle_line[i].isdigit():
            if not parsing_number:
                parsing_number = True
                symbol_found = False
                buffer = ""
            buffer = buffer + middle_line[i]
            if not symbol_found:
                symbol_found = has_symbol_adjacent(
                    top_line, middle_line, bottom_line, i
                )
        elif parsing_number:
            parsing_number = False
            if symbol_found:
                sum = sum + int(buffer)
    if parsing_number and symbol_found:
        sum = sum + int(buffer)
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
            sum = sum + sum_line(top_line, middle_line, bottom_line)
            top_line = middle_line
            middle_line = bottom_line
            bottom_line = f.readline().strip()
        print(f"The sum of all of the part numbers is {sum}")
