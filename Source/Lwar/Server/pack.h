#ifndef PACK_H
#define PACK_H

#include "id.h"
#include "str.h"

size_t id_pack(char *out, Id id);
size_t str_pack(char *out, Str in);

size_t discovery_pack(char *s, void *p);

size_t header_pack(char *s, void *p);
size_t message_pack(char *s, void *p);

size_t update_pos_rotation_pack(char *s, void *p);
size_t update_pos_pack(char *s, void *p);
size_t update_ray_pack(char *s, void *p);
size_t update_circle_pack(char *s, void *p);
size_t update_ship_pack(char *s, void *p);

#endif
