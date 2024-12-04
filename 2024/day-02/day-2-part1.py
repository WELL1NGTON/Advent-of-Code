#!/usr/bin/env python3

import sys
from enum import Enum


class STATE(Enum):
    NONE = 0
    INCREASING = 1
    DECREASING = 2
    SAFE = 3
    UNSAFE = 4


def test_safe(numbers: list[int]) -> STATE:
    current_state = STATE.NONE
    for i in range(len(numbers) - 1):
        res = numbers[i + 1] - numbers[i]
        if not abs(res) in range(1, 4):
            return STATE.UNSAFE
        test_state = STATE.NONE
        if res > 0:
            test_state = STATE.INCREASING
        else:
            test_state = STATE.DECREASING
        if current_state == STATE.NONE:
            current_state = test_state
            continue
        if current_state != test_state:
            return STATE.UNSAFE
    return STATE.SAFE


if __name__ == "__main__":
    file_path = sys.argv[1] if len(sys.argv) > 1 else "input.txt"

    with open(file_path, "r") as f:
        count = 0
        while True:
            line = f.readline().replace("\n", "")
            if not line:
                break
            numbers = [
                int(x.strip())
                for x in [x.strip() for x in line.split(" ")]
                if x
            ]
            if test_safe(numbers) == STATE.SAFE:
                count += 1
        print(f"{count} reports are safe")
