#ifndef UINT_H
#define UINT_H

#include <stdint.h>
#include <stddef.h>

size_t uint8_pack(char *s, uint8_t u);
size_t uint8_unpack(const char *s, uint8_t *u);

size_t uint16_pack(char *s, uint16_t u);
size_t uint16_unpack(const char *s, uint16_t *u);

size_t uint32_pack(char *s, uint32_t u);
size_t uint32_unpack(const char *s, uint32_t *u);

/* TODO: check whether that works, actually. */
#define int16_pack(s,u)   uint16_pack(s,u)
#define int16_unpack(s,u) uint16_unpack(s,(uint16_t*)u)

#define int32_pack(s,u)   uint32_pack(s,u)
#define int32_unpack(s,u) uint32_unpack(s,(uint32_t*)u)

#endif
