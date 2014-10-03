#ifndef STR_H
#define STR_H

#include <stddef.h>

typedef struct Str Str;

struct Str {
    unsigned char n;
    char *s;
};

size_t str_pack(char *out, Str in);
size_t str_unpack(const char *in, Str *out);

#endif
