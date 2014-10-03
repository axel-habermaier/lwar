#ifndef CONFIG_H
#define CONFIG_H

enum {
    /* network */
    APP_ID              = 0xf27087c5,
	NETWORK_REVISION    =   21,

	SERVER_PORT			= 32422,
	MULTICAST_PORT      = SERVER_PORT + 1,
	MULTICAST_TTL		= 1,

	DISCOVERY_INTERVAL  =  5 * 1000 /*ms*/, /* send discovery every 5 seconds   */
    TIMEOUT_INTERVAL    = 15 * 1000 /*ms*/, /* drop connection after 15 seconds */

    /* capacity */
    MAX_CLIENTS         =    8,
    MAX_ENTITIES        = 4096,
    MAX_ENTITY_TYPES    =   32,
    MAX_COLLISIONS      =   32, /* should be n^2-1 for priority queue */
    MAX_QUEUE           = 4096,
    MAX_STRINGS         =  128,

    NUM_SLOTS           =    4,

    MAX_NAME_LENGTH     =   32,
    MAX_CHAT_LENGTH     =  256,

    /* gameplay */

    MISBEHAVIOR_LIMIT   =   10,

    MAX_PLANETS         = 11,
    MIN_PLANET_DIST     = 2500,
    MAX_PLANET_DIST     = 2500,
};


static const char MULTICAST_GROUP[] = "FF05::3";

#endif
