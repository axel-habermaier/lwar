#include "types.h"

#include "stream.h"

#include "debug.h"
#include "message.h"
#include "packet.h"
#include "pack.h"
#include "performance.h"
#include "server.h"
#include "unpack.h"

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

static void packet_init_send_header(Packet *p, Header *h) {
    packet_init_send(p, &h->adr);
    packet_put(p, header_pack, h);
}

static void packet_flush(Packet *p) {
    if(packet_hasdata(p))
        packet_send(p);
}

static void send_message(Packet *p, Header *h, Message *m) {
    while(!packet_put(p, message_pack, m)) {
        packet_send(p);
        packet_init_send_header(p, h);
    }
}

static void send_update_messate(Packet *p, Header *h, Message *m) {
    Entity *e;
    Format *f = m->update.f;
    size_t k = 0;
    size_t n = f->n;

    updates_foreach(f,e) {
        if(e->dead) { n --; continue; }
    retry:
        if(!k) {
            k = min(n, packet_update_n(p,f->len));
            if(k) {
                m->update.n = k;
                packet_put(p, message_pack, m);
            } else {
                packet_send(p);
                packet_init_send_header(p, h);
            }
            goto retry;
        } else {
            packet_put(p, f->pack, e);
            k --;
            n --;
        }
    }
    assert(k == 0);
    assert(n == 0);
}

void stream_send(cr_t *state, Header *h, Message *m) {
    static Packet p;

    cr_begin(state);

    packet_init_send_header(&p, h);

    while(m) {
        if(is_update(m)) {
            send_update_messate(&p, h, m);
        } else {
            send_message(&p, h, m);
        }

        cr_pause(state);
    }
    
    packet_flush(&p);

    cr_end(state);
}
