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
 * @param {{start:number, end:number}} initialRange
 * @param {string[][][]} blocksSequence
 * @returns {number}
 */
function applyBlocksSeed(initialRange, blocksSequence) {
    let ranges = [
        {
            start: initialRange.start,
            end: initialRange.end,
        },
    ];
    for (const block of blocksSequence) {
        const aux = [];
        for (const range of ranges) {
            let updated = true;
            while (range.end > range.start && updated) {
                updated = false;
                for (const map of block) {
                    const destination = Number(map[0]);
                    const source = Number(map[1]);
                    const maxSeeds = Number(map[2]);
                    if (
                        range.start >= source &&
                        range.start < source + maxSeeds
                    ) {
                        aux.push({
                            start: range.start - source + destination,
                            end: Math.min(
                                range.end - source + destination,
                                destination + maxSeeds - 1
                            ),
                        });
                        range.start = source + maxSeeds;
                        updated = true;
                        break;
                    }
                }
            }
        }
        ranges = aux;
    }

    return ranges.reduce((acc, value) => {
        if (value.start < acc) {
            return value.start;
        }
        return acc;
    }, Number.MAX_SAFE_INTEGER);
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

for (let i = 0; i < seeds.length; i += 2) {
    const seedRange = {
        start: Number(seeds[i]),
        end: Number(seeds[i]) + Number(seeds[i + 1]),
    };

    const result = applyBlocksSeed(seedRange, blocksSequence);
    if (result < lowestLocation) {
        lowestLocation = result;
    }
}

console.log(
    `The lowest location number that corresponds to any of the initial seed is ${lowestLocation}`
);
