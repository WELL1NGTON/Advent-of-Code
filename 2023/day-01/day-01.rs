use std::env;
use std::fs;

const VALID_DIGITS: [&str; 9] = [
    "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
];

fn get_valid_digit(search: &str) -> i32 {
    let first_char = search.chars().nth(0).unwrap();
    if first_char >= '0' && first_char <= '9' {
        return first_char as i32 - '0' as i32;
    }
    for (i, valid_digit) in VALID_DIGITS.iter().enumerate() {
        if search.starts_with(valid_digit) {
            return (i + 1) as i32;
        }
    }

    return -1;
}

fn main() {
    let args: Vec<String> = env::args().collect();
    let file_path: &str;

    if args.len() > 1 {
        file_path = &args[1];
    } else {
        file_path = "input.txt";
    }

    let file = fs::read_to_string(file_path);

    if let Ok(contents) = file {
        let mut sum = 0;
        for line in contents.lines() {
            let mut digit1_found = false;
            let mut digit2_found = false;
            let mut digit1 = 0;
            let mut digit2 = 0;
            for (i, _) in line.chars().enumerate() {
                if !digit1_found {
                    let search_str = line.get(i..line.len()).unwrap();
                    let digit_value = get_valid_digit(search_str);
                    digit1_found = digit_value >= 0;
                    digit1 = digit_value;
                }
                if !digit2_found {
                    let search_str = line.get(line.len() - i - 1..line.len()).unwrap();
                    let digit_value = get_valid_digit(search_str);
                    digit2_found = digit_value >= 0;
                    digit2 = digit_value;
                }
            }
            if digit1_found && digit2_found {
                sum += digit1 * 10 + digit2;
            }
        }
        println!("The sum of all of the calibration values is {}", sum);
    }
}
