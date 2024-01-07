#!/usr/bin/env python3

import sys

from math import ceil


class Node:
    def __init__(self, symbol: str, x: int, y: int):
        self.symbol = symbol
        self.x = x
        self.y = y

    def is_adjacent(self, other: "Node") -> bool:
        if not other or other.symbol == ".":
            return False
        # left
        if other.y == self.y and other.x - self.x == -1:
            if self.symbol in [
                "-",
                "7",
                "J",
                "S",
            ] and other.symbol in [
                "-",
                "F",
                "L",
                "S",
            ]:
                return True
        # right
        elif other.y == self.y and other.x - self.x == 1:
            if self.symbol in [
                "-",
                "F",
                "L",
                "S",
            ] and other.symbol in [
                "-",
                "7",
                "J",
                "S",
            ]:
                return True
        # top
        elif other.x == self.x and other.y - self.y == -1:
            if self.symbol in [
                "|",
                "J",
                "L",
                "S",
            ] and other.symbol in [
                "|",
                "7",
                "F",
                "S",
            ]:
                return True
        # bottom
        elif other.x == self.x and other.y - self.y == 1:
            if self.symbol in [
                "|",
                "7",
                "F",
                "S",
            ] and other.symbol in [
                "|",
                "J",
                "L",
                "S",
            ]:
                return True
        return False

    def __eq__(self, other: "Node"):
        if not other:
            return False
        return (
            other.symbol == self.symbol
            and other.x == self.x
            and other.y == self.y
        )

    def __hash__(self):
        return hash((self.symbol, self.x, self.y))


def max_dist(graph: dict[Node, list[Node]], start: Node):
    if len(graph[start]) < 2:
        return 0
    nodes_check = [n for n in graph[start]]
    max = 0
    for node in nodes_check:
        checked: set[Node] = set()
        count = 0
        current = node
        previous = start
        while True:
            count += 1
            if len(graph[current]) != 2 or current in checked:
                count = 0
                break
            if current == start:
                break
            next = [n for n in graph[current] if n != previous][0]
            checked.add(current)
            previous = current
            current = next
        if count > max:
            max = count
    return ceil(max / 2)


if __name__ == "__main__":
    file_path = sys.argv[1] if len(sys.argv) > 1 else "input.txt"

    start: Node
    graph: dict[Node, list[Node]] = {}
    with open(file_path, "r") as f:
        i = 0
        prev_line: str | None = None
        while True:
            line = f.readline().replace("\n", "")
            if not line:
                break
            for j in range(len(line)):
                if line[j] == ".":
                    continue
                node = Node(symbol=line[j], x=j, y=i)
                graph[node] = []
                if node.symbol == "S":
                    start = node
                left = (
                    Node(symbol=line[j - 1], x=j - 1, y=i) if j > 0 else None
                )
                if j > 0 and node.is_adjacent(left):
                    graph[left].append(node)
                    graph[node].append(left)
                top = (
                    Node(symbol=prev_line[j], x=j, y=i - 1)
                    if prev_line
                    else None
                )
                if prev_line and node.is_adjacent(top):
                    graph[node].append(top)
                    graph[top].append(node)
            i += 1
            prev_line = line
    print(max_dist(graph, start))
