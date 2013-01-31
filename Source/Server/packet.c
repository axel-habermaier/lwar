#include <assert.h>
#include <stddef.h>
#include <stdint.h>

#include "server.h"
#include "message.h"
#include "packet.h"
#include "connection.h"

static int check_bounds(Packet *p,size_t n) {
    return    n != 0
           && p->b <= MAX_PACKET_LENGTH;
}

int packet_hasdata(Packet *p) {
    return (p->a + HEADER_LENGTH < p->b);
}

#define packet_len(p) ((long long)((p)->b) - (long long)((p)->a))

void packet_init(Packet *p, Address *adr, size_t ack, size_t time) {
    Header h = { APP_ID, ack, time };
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

int packet_recv(Packet *p) {
    p->a = 0;
    p->b = MAX_PACKET_LENGTH;
    if(!conn_recv(p->p, &p->b, &p->adr)) {
        p->io_failed = 1;
        return 0;
    }
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
    return 1;
}
