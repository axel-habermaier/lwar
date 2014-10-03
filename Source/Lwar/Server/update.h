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

size_t update_ship_pack(char *s, void *p);
size_t update_pos_pack(char *s, void *p);
size_t update_ray_pack(char *s, void *p);
size_t update_circle_pack(char *s, void *p);

#endif
