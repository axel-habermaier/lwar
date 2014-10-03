#ifndef CONFIG_H
#define CONFIG_H

enum {
	SERVER_PORT			= 32422,
	MULTICAST_PORT      = SERVER_PORT + 1,
	MULTICAST_TTL		= 1,
	DISCOVERY_INTERVAL  = 60000 / 12, /* 12 times each minute */
};

static const char MULTICAST_GROUP[] = "FF05::3";

#endif
