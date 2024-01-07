#!/usr/bin/env python3

import sys
from enum import Enum


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


def get_start_symbol(start: Node, first: Node, last: Node):
    if first.x == last.x:
        return "|"
    elif first.y == last.y:
        return "-"
    # first on top
    elif start.x == first.x and first.y - start.y == -1:
        # last on right
        if last.x - start.x == 1:
            return "L"
        # last on left
        elif last.x - start.x == -1:
            return "J"
    # first on bottom
    elif start.x == first.x and first.y - start.y == 1:
        # last on right
        if last.x - start.x == 1:
            return "F"
        # last on left
        elif last.x - start.x == -1:
            return "7"
    # first on left
    elif start.y == first.y and first.x - start.x == -1:
        # last on top
        if last.y - start.y == -1:
            return "J"
        # last on bottom
        elif last.y - start.y == 1:
            return "7"
    # first on right
    elif start.y == first.y and first.x - start.x == 1:
        # last on top
        if last.y - start.y == -1:
            return "L"
        # last on bottom
        elif last.y - start.y == 1:
            return "F"
    raise Exception("No start symbol found")


def get_loop(graph: dict[Node, list[Node]], start: Node):
    if len(graph[start]) < 2:
        return 0
    nodes_check = [n for n in graph[start]]
    for node in nodes_check:
        checked: set[Node] = set()
        count = 0
        current = node
        previous = start
        first_node = node
        last_node = None
        while True:
            count += 1
            if len(graph[current]) != 2 or current in checked:
                count = 0
                break
            if current == start:
                break
            next = [n for n in graph[current] if n != previous][0]
            checked.add(current)
            last_node = current
            previous = current
            current = next
        if count > 0:
            return (first_node, last_node)
    raise Exception("No loop found")


def get_vertices(graph: dict[Node, list[Node]], start: Node, first: Node):
    vertices: list[Node] = []
    current = first
    previous = start
    while True:
        if current.symbol not in ["-", "|"]:
            vertices.append(current)
        if graph[current] == graph[start]:
            break
        next = [n for n in graph[current] if n != previous][0]
        previous = current
        current = next
    return vertices


def update_start_symbol(
    graph: dict[Node, list[Node]],
    start: Node,
    first: Node,
    last: Node,
):
    old_start = start
    del graph[old_start]
    graph[first].remove(old_start)
    graph[last].remove(old_start)
    start.symbol = get_start_symbol(start, first, last)
    graph[start] = [first, last]
    graph[first].append(start)
    graph[last].append(start)


class Direction(Enum):
    CLOCKWISE = -1
    COUNTERCLOCKWISE = 1


def get_direction_nodes_turn(
    start: tuple[int, int], middle: tuple[int, int], end: tuple[int, int]
) -> Direction:
    vec1 = (middle[0] - start[0], middle[1] - start[1])
    vec2 = (end[0] - middle[0], end[1] - middle[1])
    dot = vec1[0] * vec2[0] + vec1[1] * vec2[1]
    det = vec1[0] * vec2[1] - vec1[1] * vec2[0]
    if det > 0:
        return Direction.CLOCKWISE
    elif det < 0:
        return Direction.COUNTERCLOCKWISE
    elif dot < 0:
        return Direction.CLOCKWISE
    else:
        return Direction.COUNTERCLOCKWISE


def get_direction_polygon_internal(vertices: list[Node]) -> Direction:
    count_left = 0
    count_right = 0
    for i in range(len(vertices)):
        start = i - 1 if i > 0 else len(vertices) - 1
        middle = i
        end = i + 1 if i < len(vertices) - 1 else 0
        direction = get_direction_nodes_turn(
            (vertices[start].x, vertices[start].y),
            (vertices[middle].x, vertices[middle].y),
            (vertices[end].x, vertices[end].y),
        )
        if direction == Direction.CLOCKWISE:
            count_left += 1
        elif direction == Direction.COUNTERCLOCKWISE:
            count_right += 1
    if count_left > count_right:
        return Direction.CLOCKWISE
    else:
        return Direction.COUNTERCLOCKWISE


class Rectangle:
    def __init__(self, vertices: list[tuple[int, int]]):
        self.vertices = sorted(vertices, key=lambda v: (v[0], v[1]))

    def __eq__(self, other: "Rectangle"):
        if not other:
            return False
        return self.vertices == other.vertices

    def __hash__(self):
        return hash(
            (
                self.vertices[0],
                self.vertices[1],
                self.vertices[2],
                self.vertices[3],
            )
        )

    def is_point_inside(self, point: tuple[int, int]) -> bool:
        if abs(point[0] - self.vertices[0][0]) < abs(
            self.vertices[2][0] - self.vertices[0][0]
        ) and abs(point[1] - self.vertices[0][1]) < abs(
            self.vertices[2][1] - self.vertices[0][1]
        ):
            return True
        return False


def get_smallest_rectangle(
    vertices: list[tuple[int, int]],
    start: tuple[int, int],
    middle: tuple[int, int],
    end: tuple[int, int],
) -> Rectangle:
    if start[0] == middle[0]:
        rectangle = Rectangle(
            [
                (start[0], start[1]),
                (middle[0], middle[1]),
                (end[0], end[1]),
                (end[0], start[1]),
            ]
        )
    else:
        rectangle = Rectangle(
            [
                (start[0], start[1]),
                (middle[0], middle[1]),
                (end[0], end[1]),
                (start[0], end[1]),
            ]
        )
    for node in [v for v in vertices if v not in [start, middle, end]]:
        if rectangle.is_point_inside((node[0], node[1])):
            rectangle = Rectangle(
                [
                    (start[0], start[1]),
                    (middle[0], middle[1]),
                    (end[0], end[1]),
                    (node[0], node[1]),
                ]
            )
    rectangle.vertices = sorted(rectangle.vertices, key=lambda v: (v[0], v[1]))
    return rectangle


def count_internal_points(vertices: list[Node], direction: Direction) -> int:
    area = 0
    # area shoelace algorithm
    if direction == Direction.COUNTERCLOCKWISE:
        j = len(vertices) - 1
        for i in range(len(vertices)):
            area += (vertices[j].x + vertices[i].x) * (
                vertices[j].y - vertices[i].y
            )
            j = i
    else:
        j = 0
        for i in range(len(vertices) - 1, -1, -1):
            area += (vertices[j].x + vertices[i].x) * (
                vertices[j].y - vertices[i].y
            )
            j = i
    perimeter = 0
    for i, v in enumerate(vertices):
        if i == 0:
            prev = vertices[len(vertices) - 1]
        else:
            prev = vertices[i - 1]
        perimeter += abs(v.x - prev.x) + abs(v.y - prev.y)
    return (area - perimeter) // 2 + 1  # not sure where the +1 comes from...


if __name__ == "__main__":
    """
    Notes: there is probably a better way to do this, but this is the direction
    I started with and, for the better or worse, I'm sticking with it.

    ¯\_(ツ)_/¯

    ps: this took way longer than I would like to admit...
    """

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
    (first, last) = get_loop(graph, start)
    update_start_symbol(graph, start, first, last)
    vertices = get_vertices(graph, start, first)
    poligon_direction = get_direction_polygon_internal(vertices)
    print(count_internal_points(vertices, poligon_direction))
