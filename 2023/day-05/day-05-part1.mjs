#!/usr/bin/env node
import { readFileSync } from 'fs';

const args = process.argv.slice(2);
const file = args[0] ?? 'input.txt';
const data = readFileSync(file, 'utf8').split('\n\n');

/**
 * @param {string[]} data
 * @param {string} section
 * @returns {string[][]}
 */
function getBlock(data, section) {
    return data
        .filter((x) => x.startsWith(section))[0]
        .split('\n')
        .slice(1)
        .map((x) => x.split(' ').filter((x) => x));
}

/**
 * @param {string} seed
 * @param {string[][][]} blocksSequence
 * @returns {number}
 */
function applyBlocksSeed(seed, blocksSequence) {
    let result = Number(seed);
    for (const block of blocksSequence) {
        for (const map of block) {
            const destination = Number(map[0]);
            const source = Number(map[1]);
            const range = Number(map[2]);

            if (result >= source && result < source + range) {
                result = result - source + destination;
                break;
            }
        }
    }
    return result;
}

const seeds = data
    .filter((x) => x.startsWith('seeds:'))[0]
    .split(' ')
    .slice(1);

const blocksSequence = [
    getBlock(data, 'seed-to-soil'),
    getBlock(data, 'soil-to-fertilizer'),
    getBlock(data, 'fertilizer-to-water'),
    getBlock(data, 'water-to-light'),
    getBlock(data, 'light-to-temperature'),
    getBlock(data, 'temperature-to-humidity'),
    getBlock(data, 'humidity-to-location'),
];

let lowestLocation = Number.MAX_SAFE_INTEGER;

for (let seed of seeds) {
    const result = applyBlocksSeed(seed, blocksSequence);
    if (result < lowestLocation) {
        lowestLocation = result;
    }
}

console.log(
    `The lowest location number that corresponds to any of the initial seed is ${lowestLocation}`
);
