package main

import (
	"bufio"
	"fmt"
	"os"
)

func any_positive_or_zero(min_nodes []int) bool {
	for i := 0; i < len(min_nodes); i++ {
		if min_nodes[i] >= 0 {
			return true
		}
	}
	return false
}

// Greatest common divisor (GCD)
func gcd(a, b int) int {
	for b != 0 {
		a, b = b, a%b
	}
	return a
}

// Least common multiple (LCM)
func lcm(a, b int) int {
	return (a * b) / gcd(a, b)
}

func calculateLCM(numbers []int) int {
	if len(numbers) == 0 {
		return 0
	}
	result := numbers[0]
	for i := 1; i < len(numbers); i++ {
		result = lcm(result, numbers[i])
	}
	return result
}

func main() {
	var args []string
	var file_path string
	for i := 0; i < len(os.Args); i++ {
		args = append(args, os.Args[i])
	}
	if len(args) < 2 {
		file_path = "input.txt"
	} else {
		file_path = args[1]
	}

	file, err := os.Open(file_path)
	if err != nil {
		panic(err)
	}
	scanner := bufio.NewScanner(file)
	scanner.Scan()
	commands := scanner.Text()
	map_commands := make(map[string][]string)
	nodes := []string{}
	for scanner.Scan() {
		line := scanner.Text()
		if line == "" {
			continue
		}
		map_commands[line[0:3]] = []string{line[7:10], line[12:15]}
		if line[2] == 'A' {
			nodes = append(nodes, line[0:3])
		}
	}
	lcm := 0
	min_steps_nodes := []int{}
	for i := 0; i < len(nodes); i++ {
		min_steps_nodes = append(min_steps_nodes, 0)
	}
	for any_positive_or_zero(min_steps_nodes) {
		for i := 0; i < len(commands); i++ {
			var command_index int
			if commands[i] == 'L' {
				command_index = 0
			} else if commands[i] == 'R' {
				command_index = 1
			}
			for j := 0; j < len(nodes); j++ {
				if min_steps_nodes[j] < 0 {
					continue
				}
				nodes[j] = map_commands[nodes[j]][command_index]
				min_steps_nodes[j]++
				if nodes[j][2] == 'Z' {
					// using negative numbers to indicate that the node has
					// reached Z
					min_steps_nodes[j] = min_steps_nodes[j] * -1
				}
			}
		}
	}
	for i := 0; i < len(min_steps_nodes); i++ {
		min_steps_nodes[i] = min_steps_nodes[i] * -1
	}
	lcm = calculateLCM(min_steps_nodes)
	fmt.Println(
		"The number of steps that are required for all nodes to reach ##Z is",
		lcm)
}
