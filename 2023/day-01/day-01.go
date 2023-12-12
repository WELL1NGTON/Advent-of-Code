package main

import (
	"fmt"
	"os"
)

var validDigits = []string{
	"one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
}

func getDigitValue(str string) int {
	if str[0] >= '0' && str[0] <= '9' {
		return int(str[0]) - int('0')
	}
	for i := 0; i < 9; i++ {
		if len(str) < len(validDigits[i]) {
			continue
		}

		if str[:len(validDigits[i])] == validDigits[i] {
			return i + 1
		}
	}
	return -1
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
	var line string
	var sum int = 0
	for {
		_, err := fmt.Fscanf(file, "%s", &line)
		if err != nil {
			break
		}
		// fmt.Println(line)
		var digit1_found bool = false
		var digit2_found bool = false
		var digit1 int = 0
		var digit2 int = 0
		for i := 0; i < len(line); i++ {
			if !digit1_found {
				digit1 = getDigitValue(line[i:])
				digit1_found = digit1 != -1
			}
			if !digit2_found {
				digit2 = getDigitValue(line[len(line)-i-1:])
				digit2_found = digit2 != -1
			}
			if digit1_found && digit2_found {
				break
			}
		}
		if digit1_found && digit2_found {
			sum += digit1*10 + digit2
		}
	}

	fmt.Println("The sum of all of the calibration values is", sum)
}
