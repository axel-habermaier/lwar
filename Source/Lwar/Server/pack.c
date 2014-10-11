#include "types.h"

#include "pack.h"

#include "debug.h"
#include "player.h"
#include "message.h"
#include "entity.h"
#include "uint.h"

#include <limits.h>
#include <string.h>

#ifdef _MSC_VER
    #define strndup(s, n) _strdup(s)
#endif

size_t id_pack(char *out, Id id) {
    uint16_pack(out,   id.gen);
    uint16_pack(out+2, id.n);
    return 4;
}

size_t str_pack(char *out, Str in) {
    size_t i=0;
    i += uint8_pack(out+i, in.n);
    memcpy(out+i, in.s, in.n);
    return i + in.n;
}


size_t header_pack(char *s, void *p) {
    Header *h = (Header*)p;
    size_t i=0;
    i += uint32_pack(s+i, h->app_id);
    i += uint32_pack(s+i, h->ack);
    // i += uint32_pack(s+i, h->time);
    return i;
}

size_t discovery_pack(char *s, void *p) {
    Discovery *d = (Discovery*)p;
    size_t i=0;
    i += uint32_pack(s+i, d->type);
    i += uint32_pack(s+i, d->app_id);
    i += uint8_pack(s+i, d->rev);
    i += uint16_pack(s+i, d->port);
    return i;
}

size_t message_pack(char *s, void *p) {
    Message *m = (Message *)p;
    size_t i=0;
    int j;

    i += uint8_pack(s+i, m->type);

    assert(m->seqno);
    i += uint32_pack(s+i, m->seqno);

    switch(m->type) {
    case MESSAGE_CONNECT:
        i += uint8_pack(s+i, m->connect.rev);
        i += str_pack(s+i, m->connect.nick);
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_JOIN:
        i += id_pack(s+i, m->join.player_id);
        i += str_pack(s+i, m->join.nick);
        break;
    case MESSAGE_LEAVE:
        i += id_pack(s+i, m->leave.player_id);
        i += uint8_pack(s+i, (uint8_t)m->leave.reason);
        break;
    case MESSAGE_CHAT:
        i += id_pack(s+i, m->chat.player_id);
        i += str_pack(s+i, m->chat.msg);
        break;
    case MESSAGE_ADD:
        i += id_pack(s+i, m->add.entity_id);
        i += id_pack(s+i, m->add.player_id);
        i += uint8_pack(s+i, m->add.type_id);
        break;
    case MESSAGE_REMOVE:
        i += id_pack(s+i, m->remove.entity_id);
        break;
    case MESSAGE_SELECTION:
        i += id_pack(s+i, m->selection.player_id);
        i += uint8_pack(s+i, m->selection.ship_type);
        i += uint8_pack(s+i, m->selection.weapon_type1);
        i += uint8_pack(s+i, m->selection.weapon_type2);
        i += uint8_pack(s+i, m->selection.weapon_type3);
        i += uint8_pack(s+i, m->selection.weapon_type4);
        break;
    case MESSAGE_NAME:
        i += id_pack(s+i, m->name.player_id);
        i += str_pack(s+i, m->name.nick);
        break;
    case MESSAGE_KILL:
        i += id_pack(s+i, m->kill.killer_id);
        i += id_pack(s+i, m->kill.victim_id);
        break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_REJECT:
        i += uint8_pack(s+i, (uint8_t)m->reject.reason);
        break;
    case MESSAGE_UPDATE:
    case MESSAGE_UPDATE_POS:
    case MESSAGE_UPDATE_RAY:
    case MESSAGE_UPDATE_CIRCLE:
    case MESSAGE_UPDATE_SHIP:
        i += uint8_pack(s+i, m->update.n);
        break;
    case MESSAGE_INPUT:
        i += id_pack(s+i, m->input.player_id);
        i += uint32_pack(s+i, m->input.frameno);
        i += uint8_pack(s+i, m->input.forwards);
        i += uint8_pack(s+i, m->input.backwards);
        i += uint8_pack(s+i, m->input.turn_left);
        i += uint8_pack(s+i, m->input.turn_right);
        i += uint8_pack(s+i, m->input.strafe_left);
        i += uint8_pack(s+i, m->input.strafe_right);
        i += uint8_pack(s+i, m->input.fire1);
        i += uint8_pack(s+i, m->input.fire2);
        i += uint8_pack(s+i, m->input.fire3);
        i += uint8_pack(s+i, m->input.fire4);
        i += int16_pack(s+i, m->input.aim_x);
        i += int16_pack(s+i, m->input.aim_y);
        break;
    case MESSAGE_STATS:
        i += uint8_pack(s+i, m->stats.n);
        for (j = 0; j < m->stats.n; ++j) {
            i += id_pack(s+i, m->stats.info[j].player_id);
            i += uint16_pack(s+i, m->stats.info[j].kills);
            i += uint16_pack(s+i, m->stats.info[j].deaths);
            i += uint16_pack(s+i, m->stats.info[j].ping);
        }
        break;
    case MESSAGE_COLLISION:
        i += id_pack(s+i, m->collision.entity_id[0]);
        i += id_pack(s+i, m->collision.entity_id[1]);
        i += int16_pack(s+i, m->collision.x);
        i += int16_pack(s+i, m->collision.y);
        break;
    }
    return i;
}

size_t update_pos_rotation_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, deg100(e->phi));
    return i;
}

size_t update_pos_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    return i;
}

size_t update_ray_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    Id none = { 0, USHRT_MAX };
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, deg100(e->phi));
    i += uint16_pack(s+i, e->len);
    i += id_pack(s+i, !e->target ? none : e->target->id);
    return i;
}

size_t update_circle_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, e->radius);
    return i;
}

size_t update_ship_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    
    i += uint8_pack(s+i, 100 * e->health / e->type->init_health);
    i += uint8_pack(s+i, 100 * e->health / e->type->init_health); /* TODO: actually use some shield */

    Slot *sl;
    SlotType *st;
    slots_foreach(e->player,sl,st) {
        Entity *r = sl->entity;
        i += uint8_pack(s+i, 100 * r->energy / r->type->init_energy);
    }

    return i;
}
