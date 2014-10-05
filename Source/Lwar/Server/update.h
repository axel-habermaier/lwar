#ifndef UPDATE_H
#define UPDATE_H

#include "list.h"

struct Format {
    List _l;
    size_t id;

    Pack *pack;
    Unpack *unpack;
    List  all;
    size_t len;
    size_t n;
};

void format_register(Format *f);

#endif
