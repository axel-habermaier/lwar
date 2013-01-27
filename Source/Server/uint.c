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
  out[0]=in&255;
  out[1]=in>>8;
  return 2;
}

size_t uint16_unpack(const char *in,uint16_t *out) {
  *out = ((unsigned short)((unsigned char) in[1]) << 8) + (unsigned char)in[0];
  return 2;
}

size_t uint32_pack(char *out,uint32_t in) {
  *out=in&0xff; in>>=8;
  *++out=in&0xff; in>>=8;
  *++out=in&0xff; in>>=8;
  *++out=in&0xff;
  return 4;
}

size_t uint32_unpack(const char *in,uint32_t *out) {
  *out = (((uint32_t)(unsigned char)in[3])<<24) |
         (((uint32_t)(unsigned char)in[2])<<16) |
         (((uint32_t)(unsigned char)in[1])<<8) |
          (uint32_t)(unsigned char)in[0];
  return 4;
}
