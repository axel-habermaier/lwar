#include <assert.h>
#include <stddef.h>
#include <stdint.h>
#include <string.h>

#include "server.h"
#include "message.h"
#include "packet.h"
#include "connection.h"
#include "log.h"

static int check_bounds(Packet *p,size_t n) {
    return    n != 0
           && p->b <= MAX_PACKET_LENGTH;
}

int packet_hasdata(Packet *p) {
    return (p->a + HEADER_LENGTH < p->b);
}

size_t packet_update_n(Packet *p) {
    size_t i = p->b + UPDATE_HEADER_LENGTH;
    if(i < MAX_PACKET_LENGTH)
        return (MAX_PACKET_LENGTH - i) / UPDATE_LENGTH;
    return 0;
}

void packet_init(Packet *p, Address *adr, size_t ack, size_t time) {
    Header h = { APP_ID, ack, time };
    memset(p->p, 0, sizeof(p->p));
    p->io_failed = 0;

    p->adr  = *adr;
    p->a    = 0;
    p->b    = header_pack(p->p, &h);
    p->ack  = ack;
    p->time = time;
}

int packet_put(Packet *p, Message *m, size_t seqno) {
    assert(p->a <= p->b);
    size_t n = message_pack(p->p + p->b, m, seqno);
    p->b += n;
    return check_bounds(p,n);
}

int packet_get(Packet *p, Message *m, size_t *seqno) {
    if(p->a >= p->b) return 0;
    size_t n = message_unpack(p->p + p->a, m, seqno);
    p->a += n;
    return check_bounds(p,n);
}

int packet_put_u(Packet *p, Update *u) {
    assert(p->a <= p->b);
    size_t n = update_pack(p->p + p->b, u);
    p->b += n;
    return check_bounds(p,n);
}

int packet_get_u(Packet *p, Update *u) {
    if(p->a >= p->b) return 0;
    size_t n = update_unpack(p->p + p->a, u);
    p->a += n;
    return check_bounds(p,n);
}

int packet_recv(Packet *p) {
    p->a = 0;
    p->b = MAX_PACKET_LENGTH;
    if(!conn_recv(p->p, &p->b, &p->adr)) {
        p->io_failed = 1;
        return 0;
    }
    p->io_failed = 0;
    if(p->b == 0) return 0; /* EAGAIN */

    Header h;
    p->a = header_unpack(p->p, &h);
    if(h.app_id != APP_ID) return 0; /* TODO: warn */
    p->ack  = h.ack;
    p->time = h.time;
    return 1;
}

int packet_send(Packet *p) {
    assert(p->a <= p->b);
    if(!conn_send(p->p, p->b - p->a, &p->adr)) {
        p->io_failed = 1;
        return 0;
    }
    p->io_failed = 0;
    return 1;
}

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
