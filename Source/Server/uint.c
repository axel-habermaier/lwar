#include <stdint.h>
#include <stddef.h>

#include "uint.h"

size_t uint8_pack(char *s, uint8_t u) {
    *s = u;
    return 1;
}

size_t uint8_unpack(const char *s, uint8_t *u) {
    *u = *s;
    return 1;
}

size_t uint16_pack(char *out,uint16_t in) {
  out[0]=in>>8;
  out[1]=in&255;
  out += 2;
  return 2;
}

size_t uint16_unpack(const char *in,uint16_t *out) {
  *out = ((unsigned short)((unsigned char) in[0]) << 8) + (unsigned char)in[1];
  return 2;
}

size_t uint32_pack(char *out,uint32_t in) {
  out[3]=in&0xff; in>>=8;
  out[2]=in&0xff; in>>=8;
  out[1]=in&0xff; in>>=8;
  out[0]=in&0xff;
  out += 4;
  return 4;
}

size_t uint32_unpack(const char *in,uint32_t *out) {
  *out = (((uint32_t)(unsigned char)in[0])<<24) | // big endian: lowest address stores most significant byte
         (((uint32_t)(unsigned char)in[1])<<16) |
         (((uint32_t)(unsigned char)in[2])<<8) |
          (uint32_t)(unsigned char)in[3];
  return 4;
}
