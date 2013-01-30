#include <assert.h>
#include <stddef.h>
#include <stdint.h>

#include "server.h"
#include "message.h"
#include "packet.h"
#include "connection.h"

void packet_init(Packet *p) {
    p->a = 0;
    p->b = 0;
}

int packet_put(Packet *p, Message *m, size_t seqno) {
    assert(p->a <= p->b);
    size_t n = message_pack(p->p + p->b, m, seqno, MAX_PACKET_LENGTH - p->b);
    p->b += n;
    return n != 0;
}

int packet_get(Packet *p, Message *m, size_t *seqno) {
    assert(p->a <= p->b);
    size_t n = message_unpack(p->p + p->a, m, seqno, p->b - p->a);
    p->a += n;
    return n != 0;
}

int packet_recv(Packet *p) {
    p->a = 0;
    p->a = MAX_PACKET_LENGTH;
    return    conn_recv(p->p, &p->b, &p->adr)
           && p->b != 0; /* EAGAIN */
}

int packet_send(Packet *p) {
    assert(p->a <= p->b);
    return conn_send(p->p, p->b - p->a, &p->adr);
}
