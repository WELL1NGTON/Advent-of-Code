#!/usr/bin/env node
import { readFileSync } from 'fs';

const operational = '.';
const damaged = '#';
const unknown = '?';
const verbose = false;

const args = process.argv.slice(2);
const file = args[0] ?? 'input.txt';
const data = readFileSync(file, 'utf8').split('\n');

/**
 * @param {string} springs
 * @param {number[]} infoDamaged
 * @returns {number}
 */
function possibilities(springs, infoDamaged) {
    const unknownIndex = springs.indexOf(unknown);

    if (unknownIndex !== -1) {
        return (
            possibilities(
                springs.substring(0, unknownIndex) +
                    operational +
                    springs.substring(unknownIndex + 1),
                infoDamaged
            ) +
            possibilities(
                springs.substring(0, unknownIndex) +
                    damaged +
                    springs.substring(unknownIndex + 1),
                infoDamaged
            )
        );
    }

    let springsCopy = springs;
    for (const d of infoDamaged) {
        const idx_damaged = springsCopy.indexOf(damaged);
        if (idx_damaged === -1) {
            return 0;
        }

        springsCopy = springsCopy.substring(idx_damaged);

        const idx_operational = springsCopy.indexOf(operational);

        let damagedGroup = springsCopy;
        if (idx_operational !== -1) {
            damagedGroup = springsCopy.substring(0, idx_operational);
        }

        if (damagedGroup.length !== d || damagedGroup.includes(operational)) {
            return 0;
        }

        springsCopy = springsCopy.substring(damagedGroup.length);
    }
    if (springsCopy.includes(damaged)) {
        return 0;
    }
    return 1;
}

let sum = 0;
for (const line of data) {
    if (!line) {
        continue;
    }
    const [springs, infoStr] = line.split(' ');
    const infoDamaged = infoStr.split(',').map((s) => +s);

    const result = possibilities(springs, infoDamaged);
    if (verbose) {
        console.log(springs, infoDamaged, result);
    }
    sum += result;
}

console.log('Result:', sum);
