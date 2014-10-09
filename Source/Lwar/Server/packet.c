#include "types.h"

#include "packet.h"

#include "connection.h"
#include "debug.h"
#include "pack.h"
#include "server.h"
#include "unpack.h"

#include <string.h>

void debug_packet(Packet *p);

static bool check_put(Packet *p,size_t n) {
    return    n != 0
           && p->end + n <= MAX_PACKET_LENGTH;
}

static bool check_get(Packet *p,size_t n) {
    return    n != 0
           && p->start + n <= p->end;
}

bool packet_hasdata(Packet *p) {
    return (p->start + HEADER_LENGTH < p->end);
}

bool packet_isempty(Packet *p) {
    return p->start == 0 && p->end == 0;
}

size_t packet_update_n(Packet *p, size_t s) {
    size_t i = p->end + UPDATE_HEADER_LENGTH;
    if(i < MAX_PACKET_LENGTH)
        return (MAX_PACKET_LENGTH - i) / s;
    return 0;
}

static void packet_init(Packet *p, const Address *adr, PacketType type, Connection *conn) {
    memset(p, 0, sizeof(Packet));
    p->type  = type;
    p->adr   = *adr;
    p->conn  = conn;
}

void packet_init_send(Packet *p, Address *adr) {
    packet_init(p, adr, PACKET_SEND, &server->conn_clients);
}

void packet_init_recv(Packet *p) {
    packet_init(p, &address_none, PACKET_RECV, &server->conn_clients);
}

void packet_init_discovery(Packet *p) {
    packet_init(p, &address_multicast, PACKET_SEND, &server->conn_discovery);
}

bool packet_put(Packet *p, Pack *pack, void *u) {
    assert(p->type == PACKET_SEND);
    assert(p->start <= p->end);

    size_t n = pack(p->p + p->end, u);
    if(check_put(p,n)) {
        p->end += n;
        return true;
    } else {
        return false;
    }
}

bool packet_get(Packet *p, Unpack *unpack, void *u) {
    assert(p->type == PACKET_RECV);
    assert(p->start <= p->end);
    if(p->start == p->end) return false;

    size_t n = unpack(p->p + p->start, u);
    if(check_get(p,n)) {
        p->start += n;
        return true;
    } else {
        return false;
    }
}

bool packet_peek(Packet *p, size_t *pos, Unpack *unpack, void *u) {
    assert(*pos <= p->end);
    if(*pos == p->end) return false;

    size_t n = unpack(p->p + *pos, u);
    if(n != 0 && *pos + n <= p->end) {
        *pos += n;
        return true;
    } else {
        return false;
    }
}

bool packet_recv(Packet *p) {
    assert(p->type == PACKET_RECV);

    p->start = 0;
    p->end = MAX_PACKET_LENGTH;
	p->adr = address_none;

    return conn_recv(p->conn, p->p, &p->end, &p->adr);
}

bool packet_send(Packet *p) {
    assert(p->type == PACKET_SEND);
    assert(p->start <= p->end);
    assert(p->adr.ip   != 0);
    assert(p->adr.port != 0);

    debug_packet(p);

    return conn_send(p->conn, p->p, p->end - p->start, &p->adr);
}
