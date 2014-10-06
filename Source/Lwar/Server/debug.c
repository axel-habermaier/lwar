#include "types.h"

#include "debug.h"

#include "packet.h"
#include "pool.h"
#include "log.h"
#include "message.h"
#include "server.h"

jmp_buf assert_handler;
FailedAssertion failed_assertion;

bool memchk(const void *p, char c, size_t n) {
    const char *s = (const char*)p;
    size_t i;
    for(i=0; i<n; i++) {
        if(s[i] != c)
            return false;
    }
    return true;
}

void debug_assert(bool test, const char *what, const char *file, size_t line) {
    if(test)
        return;

    if(memchk(assert_handler, 0, sizeof(assert_handler))) {
        log_die("assertion failed (uncaught) %s:%zu '%s'",
                file, line, what);
    }

    failed_assertion.what = what;
    failed_assertion.file = file;
    failed_assertion.line = line;

    longjmp(assert_handler,1);
}

void debug_check_pool(Pool *pool) {
    List *l;

    size_t i=0;
    list_for_each(l,&pool->allocated) {
        i ++;
    }

    assert(pool->i == i);

    size_t n=i;
    list_for_each(l,&pool->free) {
        n ++;
        memchk(l+1, (char)0xFF, pool->size - sizeof(List));
    }

    assert(pool->n == n);
}

void debug_dump_pool(Pool *pool) {
    log_debug("%s pool %p\n", pool->dynamic ? "dynamic" : "static", pool->mem);
    log_debug("  allocated:  %zu (%lu%%)\n", pool->i, pool->i*100 / pool->n);
    log_debug("  capacity:   %zu", pool->n);
    log_debug("  item size:  %zu", pool->size);
}

void debug_dump_entity_graph() {
    Entity *e, *c;
    entities_foreach(e) {
        children_foreach(e,c) {
            log_debug("\"%s:%d\" -> \"%s:%d",
                e->type->name, e->id.n,
                c->type->name, c->id.n);
        }
    }
}


/*
void header_debug(Header *h, const char *s) {
    log_debug("%sheader: ack %d, time %d", s, h->ack, h->time);
}

void update_debug(Update *u, const char *s) {
    log_debug("%s pos %d (%d,%d)", s, u->entity_id.n, (int)u->x, (int)u->y);
}
*/

void debug_message(Message *m, const char *s) {
    switch(m->type) {
    case MESSAGE_CONNECT:
        log_debug("%sconnect", s);
        break;
    case MESSAGE_DISCONNECT:
        log_debug("%sdisconnect", s);
        break;
    case MESSAGE_JOIN:
        log_debug("%sjoin %d: %.*s", s, m->join.player_id.n, m->join.nick.n, m->join.nick.s);
        break;
    case MESSAGE_LEAVE:
        log_debug("%sleave %d", s, m->leave.player_id.n);
        break;
    case MESSAGE_CHAT:
        log_debug("%schat %d: %.*s", s, m->chat.player_id.n, m->chat.msg.n, m->chat.msg.s);
        break;
    case MESSAGE_ADD:
        log_debug("%sadd %d: player %d, type %d", s, m->add.entity_id.n, m->add.player_id.n, m->add.type_id);
        break;
    case MESSAGE_REMOVE:
        log_debug("%srem %d", s, m->remove.entity_id.n);
        break;
    case MESSAGE_SELECTION:
        log_debug("%sselect %d: ship %d, weapons [%d,%d,%d,%d]",
                  s, m->selection.player_id.n,
                  m->selection.ship_type,
                  m->selection.weapon_type1,
                  m->selection.weapon_type2,
                  m->selection.weapon_type3,
                  m->selection.weapon_type4);
        break;
    case MESSAGE_NAME:
        log_debug("%sname %d: %.*s", s, m->name.player_id.n, m->name.nick.n, m->name.nick.s);
        break;
	case MESSAGE_KILL:
        log_debug("%skill %d by %d ", s, m->kill.victim_id.n, m->kill.killer_id.n);
        break;
    case MESSAGE_SYNCED:
        log_debug("%ssynced", s);
        break;
    case MESSAGE_REJECT:
        log_debug("%sreject", s);
        break;
    case MESSAGE_STATS:
		log_debug("%sstats", s);
        break;
    case MESSAGE_UPDATE:
    case MESSAGE_UPDATE_POS:
    case MESSAGE_UPDATE_RAY:
    case MESSAGE_UPDATE_CIRCLE:
        log_debug("%supdate #%d", s, m->update.n);
        break;
    case MESSAGE_INPUT:
        log_debug("%sinput %d", s, m->input.player_id.n);
        break;
    case MESSAGE_COLLISION:
        log_debug("%scollision %d, %d", s, m->collision.entity_id[0].n, m->collision.entity_id[1].n);
        break;
    }
}

/*
	case MESSAGE_DISCOVERY:
        log_debug("%sdiscovery (%d rev %d), port %d", s, m->discovery.app_id, m->discovery.rev, m->discovery.port);
		break;
*/

/*
void debug_packet(Packet *p) {
    size_t start = p->start;
    size_t end = p->end;
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
