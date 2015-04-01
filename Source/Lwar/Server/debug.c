#include "types.h"

#include "debug.h"

#include "unpack.h"
#include "packet.h"
#include "pool.h"
#include "log.h"
#include "message.h"
#include "server.h"

#include <stdlib.h>

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

    abort();
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
        log_debug("%sjoin %d:%d %.*s", s, ID_ARG(m->join.player_id), STR_ARG(m->join.nick));
        break;
    case MESSAGE_LEAVE:
        log_debug("%sleave %d:%d", s, ID_ARG(m->leave.player_id));
        break;
    case MESSAGE_CHAT:
        log_debug("%schat %d:%d %.*s", s, ID_ARG(m->chat.player_id), STR_ARG(m->chat.msg));
        break;
    case MESSAGE_ADD:
        log_debug("%sadd %d:%d player %d:%d, type %d, parent %d:%d", s, ID_ARG(m->add.entity_id), ID_ARG(m->add.player_id), m->add.type_id, ID_ARG(m->add.parent_id));
        break;
    case MESSAGE_REMOVE:
        log_debug("%srem %d:%d", s, ID_ARG(m->remove.entity_id));
        break;
    case MESSAGE_SELECTION:
        log_debug("%sselect %d:%d ship %d, weapons [%d,%d,%d,%d]",
                  s, ID_ARG(m->selection.player_id),
                  m->selection.ship_type,
                  m->selection.weapon_type1,
                  m->selection.weapon_type2,
                  m->selection.weapon_type3,
                  m->selection.weapon_type4);
        break;
    case MESSAGE_NAME:
        log_debug("%sname %d:%d %.*s", s, ID_ARG(m->name.player_id), STR_ARG(m->name.nick));
        break;
	case MESSAGE_KILL:
        log_debug("%skill %d:%d by %d:%d ", s, ID_ARG(m->kill.victim_id), ID_ARG(m->kill.killer_id));
        break;
    case MESSAGE_SYNCED:
        log_debug("%ssynced %d:%d", s, ID_ARG(m->synced.player_id));
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
    case MESSAGE_UPDATE_SHIP:
        log_debug("%supdate #%d", s, m->update.n);
        break;
    case MESSAGE_INPUT:
        log_debug("%sinput %d:%d", s, ID_ARG(m->input.player_id));
        break;
    case MESSAGE_COLLISION:
        log_debug("%scollision %d:%d, %d:%d", s, ID_ARG(m->collision.entity_id[0]), ID_ARG(m->collision.entity_id[1]));
        break;
    }
}

/*
	case MESSAGE_DISCOVERY:
        log_debug("%sdiscovery (%d rev %d), port %d", s, m->discovery.app_id, m->discovery.rev, m->discovery.port);
		break;
*/

void debug_header(Header *h, const char *s) {
    log_debug("%sack %d", s, h->ack);
}

void debug_packet(Packet *p) {
    /*Header h;
    Message m;
    size_t pos = 0;

    log_debug("packet {");
    packet_peek(p,&pos,header_unpack, &h);
    debug_header(&h, "  ");
    while(packet_peek(p,&pos,message_unpack,&m)) {
        debug_message(&m, "  ");
        if(is_update(&m)) {
            Format *f;
            formats_foreach(f) {
                if(f->type != m.type)
                    continue;
                pos += f->len * m.update.n;
            }
        }
    }
    log_debug("}");*/
}
