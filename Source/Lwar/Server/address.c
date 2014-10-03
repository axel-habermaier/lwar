#include "address.h"

#include <string.h>

#ifdef __unix__
#include <arpa/inet.h>
#include <netinet/in.h>
#endif

#ifdef _MSC_VER
// #include <winsock2.h>
#include <ws2tcpip.h>
#endif

Address address_multicast;

bool address_eq(Address *adr0, Address *adr1) {
    if (adr0->port != adr1->port)
		return false;

	return memcmp(adr0->ip, adr1->ip, sizeof(adr1->ip)) == 0;
}

bool address_create(Address* adr, const char* ip, uint16_t port)
{
	memset(adr, 0, sizeof(Address));

	struct in6_addr result;
	if (!inet_pton(AF_INET6, MULTICAST_GROUP, &result))
		return false;

	memcpy(&adr->ip, &result, sizeof(result));
	adr->port = htons(port);
	adr->isIPv6 = true;

	return true;
}
