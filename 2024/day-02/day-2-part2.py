#!/usr/bin/env python3

import sys
from enum import Enum


class STATE(Enum):
    NONE = 0
    INCREASING = 1
    DECREASING = 2
    SAFE = 3
    UNSAFE = 4


def test_safe(
    numbers: list[int], levels_removed=0, max_levels_removed=1
) -> STATE:
    current_state = STATE.NONE
    for i in range(len(numbers) - 1):
        res = numbers[i + 1] - numbers[i]
        if not abs(res) in range(1, 4):
            if levels_removed >= max_levels_removed:
                return STATE.UNSAFE
            tests: list[list[int]] = []
            tests.append(numbers[0:i] + numbers[(i + 1) : len(numbers)])
            tests.append(
                numbers[0 : (i + 1)] + numbers[(i + 2) : len(numbers)]
            )
            if i == 1:
                tests.append(numbers[1 : len(numbers)])
            for t in tests:
                if (
                    test_safe(t, levels_removed + 1, max_levels_removed)
                    == STATE.SAFE
                ):
                    return STATE.SAFE
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
            if levels_removed >= max_levels_removed:
                return STATE.UNSAFE
            tests: list[list[int]] = []
            tests.append(numbers[0:i] + numbers[(i + 1) : len(numbers)])
            tests.append(
                numbers[0 : (i + 1)] + numbers[(i + 2) : len(numbers)]
            )
            if i == 1:
                tests.append(numbers[1 : len(numbers)])
            for t in tests:
                if (
                    test_safe(t, levels_removed + 1, max_levels_removed)
                    == STATE.SAFE
                ):
                    return STATE.SAFE
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
