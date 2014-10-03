#include "str.h"
#include "uint.h"

#include <string.h>

size_t str_pack(char *out, Str in) {
    size_t i=0;
    i += uint8_pack(out+i, in.n);
    memcpy(out+i, in.s, in.n);
    return i + in.n;
}

size_t str_unpack(const char *in, Str *out) {
    size_t i=0;
    i += uint8_unpack(in+i, &out->n);
    out->s = strndup(in+i, out->n);
    return i + out->n;
}

