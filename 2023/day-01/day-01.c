#include "stdio.h"

#define TRUE 1
#define FALSE 0

// https://adventofcode.com/2023/day/1

char *validDigits[] = {
    "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
};

int getDigitValue(char *str, int start, int maxLength) {
    char c = str[start];
    if (c >= '0' && c <= '9') {
        return c - '0';
    }
    if (maxLength < 3) {
        return -1;
    }
    for (int i = 0; i < 9; i++) {
        char *valid_digit = validDigits[i];
        int valid_digit_length = 0;
        while (valid_digit[valid_digit_length] != '\0') {
            valid_digit_length++;
        }
        for (int j = 0; j < valid_digit_length; j++) {
            if (start + j >= maxLength) {
                break;
            }
            if (str[start + j] != valid_digit[j]) {
                break;
            }
            if (j == valid_digit_length - 1) {
                return i + 1;
            }
        }
    }
    return -1;
}

int day01(char *filename) {
    FILE *f = fopen(filename, "r");
    if (f == NULL) {
        printf("Could not open file %s\n", filename);
        return -1;
    }
    int sum = 0;
    char line[100];
    while (fgets(line, 100, f) != NULL) {
        int digit1Found = FALSE;
        int digit2Found = FALSE;
        int digit1 = 0;
        int digit2 = 0;
        int lineLength = 0;
        while (line[lineLength] != '\0') {
            lineLength++;
        }
        for (int i = 0; (!digit1Found || !digit2Found) && i < lineLength; i++) {
            int digit_value = getDigitValue(line, i, lineLength);
            if (digit_value >= 0) {
                if (!digit1Found) {
                    digit1Found = TRUE;
                    digit1 = digit_value;
                }
            }
            digit_value = getDigitValue(line, lineLength - i - 1, lineLength);
            if (digit_value >= 0) {
                if (!digit2Found) {
                    digit2Found = TRUE;
                    digit2 = digit_value;
                }
            }
        }
        sum = sum + digit1 * 10 + digit2;
    }
    fclose(f);

    return sum;
}

int main(int argc, char *argv[]) {
    char *filename = "input.txt";
    if (argc > 1) {
        filename = argv[1];
    }
    int result = day01(filename);
    if (result < 0) {
        return -1;
    }
    printf("The sum of all of the calibration values is %d\n", result);
    return 0;
}
