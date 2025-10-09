package main

import (
	"bufio"
	"fmt"
	"os"
)

const X byte = 88
const M byte = 77
const A byte = 65
const S byte = 83

const max_line_len int = 256
const max_lines int = 256

func is_mas(m [max_lines][max_line_len]byte, pos_i int, pos_j int, lines int, columns int) bool {
	if m[pos_i][pos_j] != A {
		return false
	}
	if pos_i+1 >= lines || pos_j+1 >= columns || pos_i-1 < 0 || pos_j-1 < 0 {
		return false
	}
	if m[pos_i+1][pos_j-1] == m[pos_i-1][pos_j+1] ||
		m[pos_i-1][pos_j-1] == m[pos_i+1][pos_j+1] {
		return false
	}
	count_m := 0
	count_s := 0
	letters := [4]byte{
		m[pos_i-1][pos_j-1],
		m[pos_i+1][pos_j-1],
		m[pos_i-1][pos_j+1],
		m[pos_i+1][pos_j+1],
	}
	for i := 0; i < len(letters); i++ {
		if letters[i] == M {
			count_m++
		} else if letters[i] == S {
			count_s++
		} else {
			return false
		}
	}
	if count_m != 2 {
		return false
	}
	return true
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
	var m = [max_lines][max_line_len]byte{}
	i := 0
	j := 0
	for scanner.Scan() {
		line := scanner.Text()
		if line == "" {
			continue
		}
		for j = 0; j < len(line); j++ {
			m[i][j] = line[j]
		}
		i++
	}
	lines := i
	columns := j
	count := 0
	for i := 0; i < lines; i++ {
		for j := 0; j < columns; j++ {
			if is_mas(m, i, j, lines, columns) {
				count++
			}
		}
	}
	fmt.Println(count)
}
