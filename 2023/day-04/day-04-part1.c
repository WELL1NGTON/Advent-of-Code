#include "stdio.h"
#include "stdlib.h"
#include "stdbool.h"

#define MAX_LINE_LEN 256
#define WIN_NUMBERS 16

int parse_num(char *str, int idx, long *result) {
    int aux = idx;
    while (str[aux] >= '0' && str[aux] <= '9') {
        aux++;
    }
    char *end = str + aux - 1;
    *result = strtol(&str[idx], &end, 10);
    return aux;
}

void ordered_insert(long *arr, int length, long num) {
    for (int i = 0; i < length; i++) {
        if (arr[i] > num) {
            long aux = arr[i];
            arr[i] = num;
            num = aux;
        }
    }
    arr[length] = num;
}

int binary_search(long *arr, int length, long num) {
    int start = 0;
    int end = length;
    while (end >= start) {
        int idx = (end + start) / 2;
        if (num == arr[idx]) {
            return idx;
        }
        else if (num > arr[idx]) {
            start = idx + 1;
        }
        else {
            end = idx - 1;
        }
    }
    return -1;
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
    int sum = 0;
    char line[MAX_LINE_LEN];
    long win_nums[WIN_NUMBERS];
    while (fgets(line, MAX_LINE_LEN, f) != NULL) {
        int win_len = 0;
        int idx = 0;
        long linha;
        // get line number
        while (line[idx - 1] != ':') {
            if (line[idx] < '0' || line[idx] > '9') {
                idx++;
                continue;
            }
            idx = parse_num(line, idx, &linha);
        }
        // get winner numbers
        while (line[idx - 1] != '|') {
            if (line[idx] < '0' || line[idx] > '9') {
                idx++;
                continue;
            }
            long num;
            idx = parse_num(line, idx, &num);
            ordered_insert(win_nums, win_len, num);
            win_len++;
        }
        // count matching numbers
        int count = 0;
        while (line[idx - 1] != '\0' && line[idx - 1] != '\n')
        {
            if (line[idx] < '0' || line[idx] > '9') {
                idx++;
                continue;
            }
            long my_num;
            idx = parse_num(line, idx, &my_num);
            if (binary_search(win_nums, win_len, my_num) != -1) {
                count++;
            }
        }

        if (count > 0) {
            sum += 1 << (count - 1); // 2 to the power of (count - 1)
        }
    }

    printf("Total points: %d\n", sum);
    fclose(f);
    return 0;
}
