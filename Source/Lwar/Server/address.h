#ifndef ADDRESS_H
#define ADDRESS_H

#include <stdbool.h>
#include <stdint.h>

typedef struct Address Address;

struct Address {
	uint8_t ip[16];
	uint16_t port;
	bool isIPv6;
};

bool address_create(Address *adr, const char *ip, uint16_t port);
bool address_eq(Address *adr0, Address *adr1);

static const Address address_none = {{0}, 0,0};

#endif
