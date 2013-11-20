#include <stdbool.h>

enum {
	SERVER_PORT			= 32422,
	MULTICAST_PORT      = SERVER_PORT + 1,
	MULTICAST_TTL		= 1,
	DISCOVERY_INTERVAL  = 60000 / 12,
};

#define MULTICAST_GROUP "FF05::3"

typedef struct Address {
	uint8_t ip[16];
	uint16_t port;
	bool isIPv6;
} Address;

bool address_create(Address *adr, const char *ip, uint16_t port);
bool address_eq(Address *adr0, Address *adr1);

static const Address address_none = {{}, 0,0};
extern Address address_multicast;