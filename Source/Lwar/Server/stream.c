#include "types.h"

#include "stream.h"

#include "message.h"
#include "unpack.h"
#include "pack.h"
#include "packet.h"
#include "performance.h"
#include "server.h"

/*
static struct {
    size_t nsend,nresend,nrecv;
} stats;

static void packet_scan(Packet *p) {
    Header h;
    Message m;

    int ok = packet_get(p, header_unpack, &h);
    if(!ok) return;

    if((uint32_t)(h.app_id) != APP_ID)
        return;

    Client *c = client_lookup(&p->adr);
    if(c) {
        c->last_in_ack   = max(h.ack, c->last_in_ack);
        c->last_activity = max(server->cur_clock, c->last_activity);
    }

    while(packet_get(p, message_unpack, &m)) {
        if(check_seqno(c, &m, m.seqno)) {
            if(is_reliable(&m))
                debug_message(&m, src_fmt(c));
            message_handle(c, &p->adr, &m, m.seqno);
        }
    }
}


handle all pending incoming messages
void protocol_recv() {
    timer_start(TIMER_RECV);
    stats.nrecv = 0;

    Packet p;
    packet_init_recv(&p);
    while(packet_recv(&p)) {
        stats.nrecv ++;
        packet_scan(&p);
    }
    if(p.io_failed) {
        Client *c = client_lookup(&p.adr);
        if(c) protocol_timeout(c);
    }

    timer_stop(TIMER_RECV);
    counter_set(COUNTER_RECV, stats.nrecv);
}
*/

static bool packet_scan_header(Packet *p, Header *h) {
    int ok = packet_get(p, header_unpack, &h);
    if(!ok) return false;

    if((uint32_t)(h->app_id) != APP_ID)
        return false;

    h->adr = p->adr;
    return true;
}

/* TODO: needs to evaluate h->ack */
bool stream_recv(cr_t *state, Header *h, Message *m) {
    static Packet p;

    cr_begin(state);
    packet_init_recv(&p);

    while(packet_recv(&p)) {
        int ok = packet_scan_header(&p, h);
        if(!ok) continue;

        while(packet_get(&p, message_unpack, m)) {
            cr_yield(state, true);
        }
    }

    cr_return(state, false);
}

void stream_send(cr_t *state, Header *h, Message *m) {
    static Packet p;

    cr_begin(state);

again:
    packet_init_send(&p, &h->adr);
    packet_put(&p, header_pack, h);

    while(m) {
        if(!packet_put(&p, message_pack, m)) {
            packet_send(&p);
            goto again;
        }
        cr_pause(state);
    }

    if(packet_hasdata(&p))
        packet_send(&p);

    cr_end(state);
}
