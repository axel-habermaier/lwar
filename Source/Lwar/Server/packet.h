#ifndef PACKET_H
#define PACKET_H

#include "address.h"
#include "config.h"
#include "update.h"

typedef enum PacketType PacketType;
typedef struct Packet Packet;

enum {
    UPDATE_HEADER_LENGTH = sizeof(uint32_t) + 2 * sizeof(uint8_t),  /* msg type, n */
    HEADER_LENGTH        = 3 * sizeof(uint32_t), /* app_id, ack, time */
	MAX_PACKET_LENGTH    = 512,
};

enum PacketType {
    PACKET_NONE,
    PACKET_SEND,
    PACKET_RECV,
    PACKET_DISCOVERY,
};

struct Packet {
    /* some stores serialized messages in p+start...p+end */
    PacketType type;

    Address adr;

    /* allow some overflow: since string length is a byte, this can be max 256 + some backup */
    char    p[MAX_PACKET_LENGTH + MAX_NAME_LENGTH + 16];
    size_t  start, end;

    /* temp storage for incoming packets */
    size_t  ack;
    size_t  time;

    Connection *conn;
    /* connection failed */
    bool    io_failed;
};

bool packet_hasdata(Packet *p);

/* return how many updates + 1 header still fit into p */
size_t packet_update_n(Packet *p, size_t len);

void packet_init_send(Packet *p, Address *adr);
void packet_init_recv(Packet *p);
void packet_init_discovery(Packet *p);

bool packet_put(Packet *p, Pack *pack, void *u);
bool packet_get(Packet *p, Unpack *unpack, void *u);

bool packet_recv(Packet *p);
bool packet_send(Packet *p);

#endif
