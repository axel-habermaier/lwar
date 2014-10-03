#include "types.h"

#include "packet.h"

#include "connection.h"
#include "debug.h"
#include "log.h"
#include "message.h"
#include "packet.h"
#include "state.h"

#include <stdint.h>
#include <string.h>

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

size_t packet_update_n(Packet *p, size_t s) {
    size_t i = p->end + UPDATE_HEADER_LENGTH;
    if(i < MAX_PACKET_LENGTH)
        return (MAX_PACKET_LENGTH - i) / s;
    return 0;
}

void packet_init_send(Packet *p, Address *adr, size_t ack, size_t time) {
    memset(p->p, 0, sizeof(p->p));
    p->type  = PACKET_SEND;
    p->adr   = *adr;
    p->ack   = ack;
    p->time  = time;
    p->start = 0;
    p->end   = header_pack(p->p, APP_ID, p->ack, p->time);
    p->io_failed = 0;
}

void packet_init_recv(Packet *p) {
    memset(p->p, 0, sizeof(p->p));
    p->type  = PACKET_RECV;
    p->adr   = address_none;
    p->ack   = 0;
    p->time  = 0;
    p->start = 0;
    p->end   = 0;
    p->io_failed = 0;
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

bool packet_recv(Packet *p) {
    assert(p->type == PACKET_RECV);

    p->start = 0;
    p->end = MAX_PACKET_LENGTH;
	p->adr = address_none;
    if(!conn_recv(&server->conn_clients, p->p, &p->end, &p->adr)) {
        p->io_failed = true;
        return false;
    }
    p->io_failed = false;
    if(p->end == 0) return false; /* EAGAIN */

    size_t app_id;
    p->start = header_unpack(p->p, &app_id, &p->ack);
    if((int32_t)app_id != APP_ID) return false; /* TODO: warn */
    return true;
}

bool packet_send(Packet *p) {
    assert(p->type == PACKET_SEND);
    assert(p->start <= p->end);
    assert(p->adr.ip   != 0);
    assert(p->adr.port != 0);

    if(!conn_send(&server->conn_clients, p->p, p->end - p->start, &p->adr)) {
        p->io_failed = true;
        return false;
    }
    p->io_failed = false;
    return true;
}

/*
void packet_debug(Packet *p) {
    size_t a = p->a;
    size_t b = p->b;
    Header h;
    Message m;
    Update u;
    size_t seqno;

    log_debug("packet {");
    p->start = header_unpack(p->p, &h);
    header_debug(&h, "  ");
    while(packet_get(p,&m,&seqno)) {
        message_debug(&m, "  ");
        if(m.type == MESSAGE_UPDATE) {
            size_t i;
            for(i=0; i<m.update.n; i++) {
                packet_get_u(p, &u);
                update_debug(&u, "    ");
            }
        }
    }
    log_debug("}");
    p->start = a;
    p->end = b;
}
*/

void packet_send_discovery()
{
	// TODO: Include information about the server (name, player count, estimated ping, etc.)
	char buffer[16];

	Message m;
	m.seqno = 0;
	m.type = MESSAGE_DISCOVERY;
	m.discovery.app_id = APP_ID;
	m.discovery.rev = NETWORK_REVISION;
	m.discovery.port = SERVER_PORT;
	size_t size = message_pack(buffer, &m);
	assert(size <= sizeof(buffer) / sizeof(char));
	
	conn_send(&server->conn_discovery, buffer, size, &address_multicast);
}
