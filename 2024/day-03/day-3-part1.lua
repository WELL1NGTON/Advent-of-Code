#!/usr/bin/env lua

local NUMBERS = "0123456789"

---check if start of string is a valid number
---@param s string input string
---@param stop_char string char that stops the search
---@return boolean is_valid true if number is valid
---@return integer end_position position where the search stopped
---@return integer|nil value number value if is a valid number
local function is_valid_number(s, stop_char)
    i = 1
    number_str = ""
    while i <= #s do
        if s:sub(i, i) == stop_char then
            number_str = s:sub(1, i - 1)
            break
        end
        if NUMBERS:find(s:sub(i, i), 1, true) == nil and not (i == 1 and s:sub(i, i) == "-") then
            return false, i, nil
        end
        i = i + 1
    end
    if i == 1 or number_str == "-" then
        return false, i, nil
    end
    return true, i, tonumber(s:sub(1, i - 1))
end

---execute mul functions in corrupted line
---@param s string input string
---@return integer result sum of all mul functions executed
local function exec_mul_functions(s)
    if s:len() == 0 then
        return 0
    end

    pos_mul = s:find("mul(", 1, true)
    if pos_mul == nil then
        return 0
    end

    is_valid, end_position, value = is_valid_number(s:sub(pos_mul + 4), ",")
    if not is_valid then
        return 0 + exec_mul_functions(s:sub(pos_mul + end_position))
    end
    v1 = value
    pos_mul = pos_mul + end_position

    is_valid, end_position, value = is_valid_number(s:sub(pos_mul + 4), ")")
    if not is_valid then
        return 0 + exec_mul_functions(s:sub(pos_mul + end_position))
    end

    return v1 * value + exec_mul_functions(s:sub(pos_mul + end_position))
end

local file_path = arg[1]
if file_path == nil then
    file_path = "input.txt"
end

file = io.open(file_path, "r")

if file == nil then
    print("The file " .. file_path .. " does not exist.")
    os.exit(1)
end

---@type integer
sum = 0
for line in file:lines("l") do
    if type(line) ~= "string" or #line == 0 then
        break
    end
    sum = sum + exec_mul_functions(line --[[@as string]])
end

print('The sum of all multiplications is: ' .. sum)
io.close(file)
