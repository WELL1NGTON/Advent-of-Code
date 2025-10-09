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

func xmas_line(m [max_lines][max_line_len]byte, pos_i int, pos_j int, lines int, columns int) bool {
	if pos_j+3 >= columns {
		return false
	}
	if m[pos_i][pos_j] == X &&
		m[pos_i][pos_j+1] == M &&
		m[pos_i][pos_j+2] == A &&
		m[pos_i][pos_j+3] == S {
		return true
	}
	if m[pos_i][pos_j] == S &&
		m[pos_i][pos_j+1] == A &&
		m[pos_i][pos_j+2] == M &&
		m[pos_i][pos_j+3] == X {
		return true
	}
	return false
}

func xmas_column(m [max_lines][max_line_len]byte, pos_i int, pos_j int, lines int, columns int) bool {
	if pos_i+3 >= columns {
		return false
	}
	if m[pos_i][pos_j] == X &&
		m[pos_i+1][pos_j] == M &&
		m[pos_i+2][pos_j] == A &&
		m[pos_i+3][pos_j] == S {
		return true
	}
	if m[pos_i][pos_j] == S &&
		m[pos_i+1][pos_j] == A &&
		m[pos_i+2][pos_j] == M &&
		m[pos_i+3][pos_j] == X {
		return true
	}
	return false
}

func xmas_diagonal1(m [max_lines][max_line_len]byte, pos_i int, pos_j int, lines int, columns int) bool {
	if pos_i+3 >= lines || pos_j+3 >= columns {
		return false
	}
	if m[pos_i][pos_j] == X &&
		m[pos_i+1][pos_j+1] == M &&
		m[pos_i+2][pos_j+2] == A &&
		m[pos_i+3][pos_j+3] == S {
		return true
	}
	if m[pos_i][pos_j] == S &&
		m[pos_i+1][pos_j+1] == A &&
		m[pos_i+2][pos_j+2] == M &&
		m[pos_i+3][pos_j+3] == X {
		return true
	}
	return false
}

func xmas_diagonal2(m [max_lines][max_line_len]byte, pos_i int, pos_j int, lines int, columns int) bool {
	if pos_i+3 >= lines || pos_j-3 < 0 {
		return false
	}
	if m[pos_i][pos_j] == X &&
		m[pos_i+1][pos_j-1] == M &&
		m[pos_i+2][pos_j-2] == A &&
		m[pos_i+3][pos_j-3] == S {
		return true
	}
	if m[pos_i][pos_j] == S &&
		m[pos_i+1][pos_j-1] == A &&
		m[pos_i+2][pos_j-2] == M &&
		m[pos_i+3][pos_j-3] == X {
		return true
	}
	return false
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
			if xmas_line(m, i, j, lines, columns) {
				count++
			}
			if xmas_column(m, i, j, lines, columns) {
				count++
			}
			if xmas_diagonal1(m, i, j, lines, columns) {
				count++
			}
			if xmas_diagonal2(m, i, j, lines, columns) {
				count++
			}
		}
	}
	fmt.Println(count)
}
