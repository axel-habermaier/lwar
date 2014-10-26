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
*/

static bool packet_scan_header(Packet *p, Header *h) {
    int ok = packet_get(p, header_unpack, h);
    if(!ok) return false;

    if((uint32_t)(h->app_id) != APP_ID)
        return false;

    h->adr = p->adr;
    return true;
}

bool stream_recv(cr_t *state, Header *h, Message *m) {
    static Packet p;
	bool ok;

    cr_begin(state);
    packet_init_recv(&p);

    while(packet_recv(&p)) {
        if(p.end == 0) /* EAGAIN */
            cr_yield(state, false);

        ok = packet_scan_header(&p, h);
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

static bool packet_flush(Packet *p) {
    if(packet_hasdata(p))
        return packet_send(p);
    return true;
}

static bool send_message(Packet *p, Header *h, Message *m) {
    while(!packet_put(p, message_pack, m)) {
        if(!packet_send(p))
            return false;
        packet_init_send_header(p, h);
    }
    return true;
}

static bool send_update_message(Packet *p, Header *h, Message *m) {
    Entity *e;
    Format *f = m->update.f;
    size_t k = 0;
    size_t n = f->n;

    updates_foreach(f,e) {
        assert(!e->dead);
    retry:
        if(!k) {
            k = min(n, packet_update_n(p,f->len));
            if(k) {
                m->update.n = k;
                packet_put(p, message_pack, m);
            } else {
                if(!packet_send(p))
                    return false;
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
    return true;
}

bool stream_send(cr_t *state, Header *h, Message *m) {
    static Packet p;
    bool ok = true;

    cr_begin(state);

    packet_init_send_header(&p, h);

    while(m && ok) {
        if(is_update(m)) {
            ok = send_update_message(&p, h, m);
        } else {
            ok = send_message(&p, h, m);
        }

        cr_yield(state, ok);
    }
    
    ok = packet_flush(&p);

    cr_return(state, ok);
}

bool stream_send_flush(Header *h, Message *m) {
    Packet p;
    packet_init_send_header(&p, h);
    packet_put(&p, message_pack, m);
    return packet_send(&p);
}
