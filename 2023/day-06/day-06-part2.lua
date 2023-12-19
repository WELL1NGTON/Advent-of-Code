#!/usr/bin/env lua

file_path = arg[1]
if file_path == nil then
    file_path = "input.txt"
end

file = io.open(file_path, "r")

if file == nil then
    print("The file " .. file_path .. " does not exist.")
    os.exit(1)
end
lines = {}
i = 1
for line in file:lines("l") do
    lines[i] = line
    i = i + 1
end
io.close(file)

line_times = lines[1]
line_distances = lines[2]

if type(line_times) ~= "string" or type(line_distances) ~= "string" then
    print("The file " .. file_path .. " is empty or has less than 2 lines.")
    os.exit(1)
end

line_times = string.gsub(line_times, " ", "")
total_time = tonumber(string.gmatch(line_times, "%d+")())

line_distances = string.gsub(line_distances, " ", "")
min_dist = tonumber(string.gmatch(line_distances, "%d+")())

half = math.floor(total_time / 2)
min_time = 0
for j = 1, half, 1 do
    if j * (total_time - j) > min_dist then
        min_time = j
        break;
    end
end

if min_time ~= 0 then
    print(total_time - 2 * min_time + 1)
else
    print(0)
end
