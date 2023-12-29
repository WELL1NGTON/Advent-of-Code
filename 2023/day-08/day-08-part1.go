package main

import (
	"bufio"
	"fmt"
	"os"
)

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
	for scanner.Scan() {
		line := scanner.Text()
		if line == "" {
			continue
		}
		map_commands[line[0:3]] = []string{line[7:10], line[12:15]}
	}
	current_node := "AAA"
	count := 0
	for current_node != "ZZZ" {
		for i := 0; i < len(commands); i++ {
			if commands[i] == 'L' {
				current_node = map_commands[current_node][0]
			} else if commands[i] == 'R' {
				current_node = map_commands[current_node][1]
			}
			count++
			if current_node == "ZZZ" {
				break
			}
		}
	}
	fmt.Println("The number of steps that are required to reach ZZZ is", count)
}
