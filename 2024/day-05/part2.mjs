#!/usr/bin/env node
import { readFileSync } from 'fs';

const args = process.argv.slice(2);
const file = args[0] ?? 'input.txt';
const data = readFileSync(file, 'utf8').split('\n');

class PageRulesGraph {
    constructor() {
        /** @type {Object.<number, number[]>} */
        this.rules = [];
    }

    /**
     * @param {number} page - vertex number
     */
    addPage(page) {
        if (!this.rules[page]) {
            this.rules[page] = [];
        }
    }

    /**
     * @param {number} page - vertex number
     */
    removePage(page) {
        while (this.rules[page]) {
            const adjacentVertex = this.rules[page].pop();
            this.removeRule(page, adjacentVertex);
        }
        delete this.rules[page];
    }

    /**
     * @param {number} pageLeft - source vertex
     * @param {number} pageRight - destination vertex
     */
    addRule(pageLeft, pageRight) {
        if (!this.rules[pageLeft]) {
            this.addPage(pageLeft);
        }
        if (!this.rules[pageRight]) {
            this.addPage(pageRight);
        }
        this.rules[pageLeft].push(pageRight);
    }

    /**
     * @param {number} pageLeft - source vertex
     * @param {number} pageRight - destination vertex
     */
    removeRule(pageLeft, pageRight) {
        this.rules[pageLeft] = this.rules[pageLeft].filter(
            (v) => v !== pageRight
        );
    }

    toString() {
        let result = '';
        for (let vertex in this.rules) {
            result += `${vertex} -> ${this.rules[vertex].join(', ')}\n`;
        }
        return result;
    }

    /**
     * @param {number[]} pages
     * @returns {number[]} sorted pages
     */
    sortPages(pages) {
        /** @type {[number,number[]][]} */
        const rulesFollow = [];
        for (let page of pages) {
            rulesFollow.push([
                page,
                this.rules[page].filter((r) => {
                    return pages.find((p) => p === r);
                }),
            ]);
        }

        return rulesFollow
            .sort((r1, r2) => r2[1].length - r1[1].length)
            .map((r) => r[0]);
    }
}

/** @type PageRulesGraph */
const rules = new PageRulesGraph();
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
        rules.addRule(rule[0], rule[1]);
    } else {
        pagesToProduce.push(line.split(',').map((v) => Number(v.trim())));
    }
}

let sum = 0;

// console.log(`${rules}`);

for (let pages of pagesToProduce) {
    const sorted = rules.sortPages(pages);
    if (pages.every((p, i) => p === sorted[i])) {
        continue;
    }
    sum += sorted[Math.floor(sorted.length / 2)];
}

console.log('Sum of middle pages of sorted updates that where unsorted:', sum);
