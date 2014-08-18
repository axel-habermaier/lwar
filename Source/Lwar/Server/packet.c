#include <assert.h>
#include <stddef.h>
#include <stdint.h>
#include <string.h>

#include "server.h"
#include "message.h"
#include "packet.h"
#include "connection.h"
#include "log.h"

static bool check_put(Packet *p,size_t n) {
    return    n != 0
           && p->b + n <= MAX_PACKET_LENGTH;
}

static bool check_get(Packet *p,size_t n) {
    return    n != 0
           && p->a + n <= p->b;
}

bool packet_hasdata(Packet *p) {
    return (p->a + HEADER_LENGTH < p->b);
}

size_t packet_update_n(Packet *p, size_t s) {
    size_t i = p->b + UPDATE_HEADER_LENGTH;
    if(i < MAX_PACKET_LENGTH)
        return (MAX_PACKET_LENGTH - i) / s;
    return 0;
}

void packet_init(Packet *p, Address *adr, size_t ack, size_t time) {
    memset(p->p, 0, sizeof(p->p));
    p->ack  = ack;
    p->time = time;
    p->adr  = *adr;
    p->a    = 0;
    p->b    = header_pack(p->p, APP_ID, p->ack, p->time);
    p->io_failed = 0;
}

bool packet_put(Packet *p, Pack *pack, void *u) {
    assert(p->a <= p->b);
    size_t n = pack(p->p + p->b, u);
    if(check_put(p,n)) {
        p->b += n;
        return true;
    } else {
        return false;
    }
}

bool packet_get(Packet *p, Unpack *unpack, void *u) {
    if(p->a >= p->b) return false;
    size_t n = unpack(p->p + p->a, u);
    if(check_get(p,n)) {
        p->a += n;
        return true;
    } else {
        return false;
    }
}

bool packet_recv(Packet *p) {
    p->a = 0;
    p->b = MAX_PACKET_LENGTH;
	p->adr = address_none;
    if(!conn_recv(&server->conn_clients, p->p, &p->b, &p->adr)) {
        p->io_failed = true;
        return false;
    }
    p->io_failed = false;
    if(p->b == 0) return false; /* EAGAIN */

    size_t app_id;
    p->a = header_unpack(p->p, &app_id, &p->ack, &p->time);
    if((int32_t)app_id != APP_ID) return false; /* TODO: warn */
    return true;
}

bool packet_send(Packet *p) {
    assert(p->a <= p->b);
    assert(p->adr.ip   != 0);
    assert(p->adr.port != 0);

    if(!conn_send(&server->conn_clients, p->p, p->b - p->a, &p->adr)) {
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
    p->a = header_unpack(p->p, &h);
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
    p->a = a;
    p->b = b;
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
