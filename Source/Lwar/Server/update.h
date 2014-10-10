#ifndef UPDATE_H
#define UPDATE_H

#include "types.h"
#include "list.h"
#include "message.h"

struct Format {
    List _l;
    MessageType type;

    Pack *pack;
    Unpack *unpack;
    List  all;
    size_t len;
    size_t n;
};

void format_register(Format *f);

#endif
