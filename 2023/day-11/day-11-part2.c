#include "stdio.h"
#include "stdlib.h"
#include "stdbool.h"

#define MAX_LINE_LEN 256
#define GALAXY '#'
#define EMPTY '.'

#define max(a, b) (((a) > (b)) ? (a) : (b))
#define min(a, b) (((a) > (b)) ? (b) : (a))

double distance(int x1, int y1,
             int x2, int y2,
             int *empty_x, int len_empty_x,
             int *empty_y, int len_empty_y) {
    int min_x = min(x1, x2);
    int max_x = max(x1, x2);
    int min_y = min(y1, y2);
    int max_y = max(y1, y2);

    double dist = max_x - min_x + max_y - min_y;

    for (int i = 0; i < len_empty_x && empty_x[i] < max_x; i++) {
        if (empty_x[i] > min_x) {
            dist += 1000000 - 1;
        }
    }

    for (int i = 0; i < len_empty_y && empty_y[i] < max_y; i++) {
        if (empty_y[i] > min_y) {
            dist += 1000000 - 1;
        }
    }

    return dist;
}

int init_empty_y(int *empty_y, char *line) {
    int len = 0;
    for (int i = 0; line[i] != '\0' && line[i] != '\n'; i++) {
        empty_y[i] = i;
        len = i + 1;
    }
    return len;
}

void remove_num_empty_y(int value, int *empty_y, int *len_empty_y) {
    bool found = false;
    for (int i = 0; i < *len_empty_y; i++) {
        if (!found && empty_y[i] > value) {
            break;
        }
        if (empty_y[i] == value) {
            found = true;
        }
        if (found && i < *len_empty_y - 1) {
            empty_y[i] = empty_y[i + 1];
        }
    }
    if (found) {
        *len_empty_y = *len_empty_y - 1;
    }
}

int main(int argc, char *argv[]) {
    char *filename = "input.txt";
    if (argc > 1) {
        filename = argv[1];
    }

    FILE *f = fopen(filename, "r");
    if (f == NULL) {
        printf("Could not open file %s\n", filename);
        return -1;
    }

    int galaxies[MAX_LINE_LEN * MAX_LINE_LEN][2];
    int len_galaxies = 0;

    int empty_x[MAX_LINE_LEN];
    int len_empty_x = 0;

    int empty_y[MAX_LINE_LEN];
    int len_empty_y = 0;

    char line[MAX_LINE_LEN];
    int line_idx = 0;

    while (fgets(line, MAX_LINE_LEN, f) != NULL) {
        if (line_idx == 0) {
            len_empty_y = init_empty_y(empty_y, line);
        }

        bool is_line_empty = true;
        for (int j = 0; line[j] != '\0' && line[j] != '\n'; j++) {
            if (line[j] == GALAXY) {
                is_line_empty = false;
                galaxies[len_galaxies][0] = line_idx;
                galaxies[len_galaxies][1] = j;
                len_galaxies++;
                remove_num_empty_y(j, empty_y, &len_empty_y);
            }
        }

        if (is_line_empty) {
            empty_x[len_empty_x] = line_idx;
            len_empty_x++;
        }

        line_idx++;
    }

    double sum = 0;
    for (int i = 0; i < len_galaxies; i++) {
        for (int j = i + 1; j < len_galaxies; j++) {
            sum += distance(
                galaxies[i][0], galaxies[i][1],
                galaxies[j][0], galaxies[j][1],
                empty_x, len_empty_x,
                empty_y, len_empty_y);
        }
    }

    printf("Sum of distances between galaxies: %.0f\n", sum);
    fclose(f);
    return 0;
}
