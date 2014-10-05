#ifndef STR_H
#define STR_H

#include <stddef.h>

typedef struct Str Str;

struct Str {
    unsigned char n;
    char *s;
};

#endif
