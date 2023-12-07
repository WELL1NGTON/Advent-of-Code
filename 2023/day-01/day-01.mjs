#!/usr/bin/env node
import { readFileSync } from 'fs';

const validDigits = [
    'one',
    'two',
    'three',
    'four',
    'five',
    'six',
    'seven',
    'eight',
    'nine',
];

function getDigitValue(str) {
    if (str[0] >= '0' && str[0] <= '9') {
        return Number(str[0]);
    }
    for (let i = 0; i < 9; i++) {
        if (str.startsWith(validDigits[i])) {
            return i + 1;
        }
    }
    return -1;
}

const args = process.argv.slice(2);
const file = args[0] ?? 'input.txt';
const data = readFileSync(file, 'utf8').split('\n');

let sum = 0;
for (const line of data) {
    let digit1Found = false;
    let digit2Found = false;
    let digit1 = 0;
    let digit2 = 0;
    for (let i = 0; i < line.length && (!digit1Found || !digit2Found); i++) {
        if (!digit1Found) {
            const digit = getDigitValue(line.substring(i, line.length));
            digit1Found = digit > 0;
            digit1 = digit;
        }
        if (!digit2Found) {
            const digit = getDigitValue(
                line.substring(line.length - i - 1, line.length)
            );
            digit2Found = digit > 0;
            digit2 = digit;
        }
    }
    sum += digit1 * 10 + digit2;
}

console.log(`The sum of all of the calibration values is ${sum}`);
