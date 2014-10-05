#ifndef UNPACK_H
#define UNPACK_H

#include "id.h"
#include "str.h"

size_t id_unpack(const char *out, Id *id);
size_t str_unpack(const char *in, Str *out);
size_t header_unpack(const char *s, size_t *app_id, size_t *ack);
size_t message_unpack(const char *s, void *p);

#endif
