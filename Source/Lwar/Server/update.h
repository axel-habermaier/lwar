#ifndef UPDATE_H
#define UPDATE_H

#include <stddef.h>

typedef struct Format Format;

void format_register(Format *f);

size_t update_ship_pack(char *s, void *p);
size_t update_pos_pack(char *s, void *p);
size_t update_ray_pack(char *s, void *p);
size_t update_circle_pack(char *s, void *p);

#endif
