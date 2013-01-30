size_t uint8_pack(char *s, uint8_t u);
size_t uint8_unpack(const char *s, uint8_t *u);

size_t uint16_pack(char *s, uint16_t u);
size_t uint16_unpack(const char *s, uint16_t *u);

size_t uint32_pack(char *s, uint32_t u);
size_t uint32_unpack(const char *s, uint32_t *u);

/* TODO: */
#define int32_pack(s,u)   uint32_pack(s,u)
#define int32_unpack(s,u) uint32_unpack(s,(uint32_t*)u)
