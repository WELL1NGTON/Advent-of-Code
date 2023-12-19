#!/usr/bin/env lua

file_path = arg[1]
if file_path == nil then
    file_path = "input.txt"
end

-- if not io.open(file_path, "r") then
--     print("The file " .. file_path .. " does not exist.")
--     os.exit(1)
-- end


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
    print(type(line_times))
    print(type(line_distances))
    print("The file " .. file_path .. " is empty or has less than 2 lines.")
    os.exit(1)
end

i = 1
times = {}
for number in string.gmatch(line_times, "%d+") do
    times[i] = tonumber(number)
    i = i + 1
end

i = 1
distances = {}
for number in string.gmatch(line_distances, "%d+") do
    distances[i] = tonumber(number)
    i = i + 1
end

results = {}
for i = 1, #times, 1 do
    min_dist = distances[i]
    total_time = times[i]
    half = math.floor(total_time / 2)
    min_time = 0
    for j = 1, half, 1 do
        if j * (total_time - j) > min_dist then
            min_time = j
            break;
        end
    end
    if min_time ~= 0 then
        results[i] = total_time - 2 * min_time + 1
    else
        results[i] = 0
    end
end

total = 1
for i, r in ipairs(results) do
    -- print(i .. ": " .. r)
    total = total * r
end

print(total)
