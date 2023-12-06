#!/usr/bin/env lua

ZERO_AS_BYTE = string.byte('0')
NINE_AS_BYTE = string.byte('9')

valid_digits = {
    "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }

--- Returns the value of the given digit character `c`.
--- @param str string
--- @return integer
function get_digit_value(str)
    char1_byte = string.byte(str)
    if char1_byte >= ZERO_AS_BYTE and char1_byte <= NINE_AS_BYTE then
        return char1_byte - ZERO_AS_BYTE
    end
    if string.len(str) < 3 then
        return -1
    end
    for i, digit in ipairs(valid_digits) do
        if string.len(str) < string.len(digit) then
            goto continue
        end
        if string.sub(str, 1, string.len(digit)) == digit then
            return i
        end
        ::continue::
    end
    return -1
end

--- Path to the input file gotten from the command line arguments.
file_path = arg[1]
if file_path == nil then
    file_path = "input.txt"
end

if not io.open(file_path, "r") then
    print("The file " .. file_path .. " does not exist.")
    os.exit(1)
end

sum = 0
for line in io.lines(file_path, "l") do
    i = 1
    digit1_found = false
    digit2_found = false
    digit1 = 0
    digit2 = 0
    line_len = string.len(line)
    while i <= line_len and (not digit1_found or not digit2_found) do
        if not digit1_found then
            digit_value = get_digit_value(string.sub(line, i, line_len))
            if digit_value >= 0 then
                digit1_found = true
                digit1 = digit_value
            end
        end
        if not digit2_found then
            digit_value = get_digit_value(
                string.sub(line, line_len - i + 1, line_len))
            if digit_value >= 0 then
                digit2_found = true
                digit2 = digit_value
            end
        end
        i = i + 1
    end
    sum = sum + digit1 * 10 + digit2
end

print("The sum of all of the calibration values is " .. sum)
