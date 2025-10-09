#!/usr/bin/env node
import { readFileSync } from 'fs';

const args = process.argv.slice(2);
const file = args[0] ?? 'input.txt';
const data = readFileSync(file, 'utf8').split('\n');

/** @type {Object.<number, number[]>} */
const orderRules = {};
/** @type number[][] */
const pagesToProduce = [];

let readingRules = true;
for (let line of data) {
    if (line.length == 0) {
        readingRules = false;
        continue;
    }
    if (readingRules) {
        const rule = line.split('|', 2).map((v) => Number(v.trim()));
        if (orderRules[rule[1]]) {
            orderRules[rule[1]].push(rule[0]);
        } else {
            orderRules[rule[1]] = [rule[0]];
        }
    } else {
        pagesToProduce.push(line.split(',').map((v) => Number(v.trim())));
    }
}

let sum = 0;

/** @type Set<Number> */
const cantAppear = new Set();
for (let pages of pagesToProduce) {
    let isCorrect = true;
    for (let page of pages) {
        if (cantAppear.has(page)) {
            isCorrect = false;
            break;
        }
        if (orderRules[page]) {
            orderRules[page].forEach((v) => cantAppear.add(v));
        }
    }
    if (isCorrect) {
        sum += pages[Math.floor(pages.length / 2)];
    }
    cantAppear.clear();
}

console.log(`Sum of middle pages from correctly-ordered updates: ${sum}`);
