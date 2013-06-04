#include <assert.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>

#include "uint.h"
#include "server.h"
#include "log.h"
#include "message.h"

int is_reliable(Message *m) {
    return m->type < 100;
}

static size_t str_pack(char *out, Str in) {
    size_t i=0;
    i += uint8_pack(out+i, in.n);
    memcpy(out+i, in.s, in.n);
    return i + in.n;
}

static size_t str_unpack(const char *in, Str *out) {
    size_t i=0;
    i += uint8_unpack(in+i, &out->n);
    out->s = (char*)malloc(out->n);
    memcpy(out->s, in+i, out->n);
    return i + out->n;
}

size_t id_pack(char *out, Id id) {
    uint16_pack(out,   id.gen);
    uint16_pack(out+2, id.n);
    return 4;
}

size_t id_unpack(const char *out, Id *id) {
    uint16_unpack(out,   &id->gen);
    uint16_unpack(out+2, &id->n);
    return 4;
}

size_t header_pack(char *s, size_t app_id, size_t ack, size_t time) {
    size_t i=0;
    i += uint32_pack(s+i, app_id);
    i += uint32_pack(s+i, ack);
    i += uint32_pack(s+i, time);
    return i;
}

size_t header_unpack(const char *s, size_t *app_id, size_t *ack, size_t *time) {
    size_t i=0;
    uint32_t _app_id,_ack,_time;
    i += uint32_unpack(s+i, &_app_id);
    i += uint32_unpack(s+i, &_ack);
    i += uint32_unpack(s+i, &_time);
    *app_id = _app_id;
    *ack    = _ack;
    *time   = _time;
    return i;
}

size_t message_pack(char *s, void *p) {
    Message *m = (Message *)p;
    size_t i=0;
    i += uint8_pack(s+i, m->type);

    if(is_reliable(m)) {
        assert(m->seqno);
        i += uint32_pack(s+i, m->seqno);
    } else {
        assert(!m->seqno);
    }

    switch(m->type) {
    case MESSAGE_CONNECT:
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
		i += uint8_pack(s+i, m->leave.reason);
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
    case MESSAGE_FULL:
        break;
    case MESSAGE_STATS:
        assert(0);
        break;
    case MESSAGE_UPDATE:
    case MESSAGE_UPDATE_POS:
    case MESSAGE_UPDATE_RAY:
    case MESSAGE_UPDATE_CIRCLE:
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
    case MESSAGE_COLLISION:
        i += id_pack(s+i, m->collision.entity_id[0]);
        i += id_pack(s+i, m->collision.entity_id[1]);
        i += int16_pack(s+i, m->collision.x);
        i += int16_pack(s+i, m->collision.y);
        break;
    }
    return i;
}

size_t message_unpack(const char *s, void *p) {
    Message *m = (Message*)p;
    size_t i=0;
    uint8_t _type;
    i += uint8_unpack(s+i, &_type);
    m->type = (MessageType)_type;

    if(is_reliable(m)) {
        uint32_t _seqno;
        i += uint32_unpack(s+i, &_seqno);
        assert(_seqno);
        m->seqno = _seqno;
    }

    switch(m->type) {
    case MESSAGE_CONNECT:
		i += str_unpack(s+i, &m->connect.nick);
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_JOIN:
        i += id_unpack(s+i, &m->join.player_id);
		i += str_unpack(s+i, &m->join.nick);
        break;
    case MESSAGE_LEAVE:
        i += id_unpack(s+i, &m->leave.player_id);
		i += uint8_unpack(s+i, &m->leave.reason);
        break;
    case MESSAGE_CHAT:
        i += id_unpack(s+i, &m->chat.player_id);
        i += str_unpack(s+i, &m->chat.msg);
        break;
    case MESSAGE_ADD:
        i += id_unpack(s+i, &m->add.entity_id);
        i += id_unpack(s+i, &m->add.player_id);
        i += uint8_unpack(s+i, &m->add.type_id);
        break;
    case MESSAGE_REMOVE:
        i += id_unpack(s+i, &m->remove.entity_id);
        break;
    case MESSAGE_SELECTION:
        i += id_unpack(s+i, &m->selection.player_id);
        i += uint8_unpack(s+i, &m->selection.ship_type);
        i += uint8_unpack(s+i, &m->selection.weapon_type1);
        i += uint8_unpack(s+i, &m->selection.weapon_type2);
        i += uint8_unpack(s+i, &m->selection.weapon_type3);
        i += uint8_unpack(s+i, &m->selection.weapon_type4);
        break;
    case MESSAGE_NAME:
        i += id_unpack(s+i, &m->name.player_id);
        i += str_unpack(s+i, &m->name.nick);
        break;
	case MESSAGE_KILL:
		i += id_unpack(s+i, &m->kill.killer_id);
		i += id_unpack(s+i, &m->kill.victim_id);
		break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_FULL:
        break;
    case MESSAGE_STATS:
        assert(0);
        break;
    case MESSAGE_UPDATE:
    case MESSAGE_UPDATE_POS:
    case MESSAGE_UPDATE_RAY:
    case MESSAGE_UPDATE_CIRCLE:
        i += uint8_unpack(s+i, &m->update.n);
        break;
    case MESSAGE_INPUT:
        i += id_unpack(s+i, &m->input.player_id);
        i += uint32_unpack(s+i, &m->input.frameno);
        i += uint8_unpack(s+i, &m->input.forwards);
        i += uint8_unpack(s+i, &m->input.backwards);
        i += uint8_unpack(s+i, &m->input.turn_left);
        i += uint8_unpack(s+i, &m->input.turn_right);
        i += uint8_unpack(s+i, &m->input.strafe_left);
        i += uint8_unpack(s+i, &m->input.strafe_right);
        i += uint8_unpack(s+i, &m->input.fire1);
        i += uint8_unpack(s+i, &m->input.fire2);
        i += uint8_unpack(s+i, &m->input.fire3);
        i += uint8_unpack(s+i, &m->input.fire4);
        i += int16_unpack(s+i, &m->input.aim_x);
        i += int16_unpack(s+i, &m->input.aim_y);
        break;
    case MESSAGE_COLLISION:
        i += id_unpack(s+i, &m->collision.entity_id[0]);
        i += id_unpack(s+i, &m->collision.entity_id[1]);
        i += int16_unpack(s+i, &m->collision.x);
        i += int16_unpack(s+i, &m->collision.y);
        break;
    }
    return i;
}

/*
void header_debug(Header *h, const char *s) {
    log_debug("%sheader: ack %d, time %d", s, h->ack, h->time);
}

void update_debug(Update *u, const char *s) {
    log_debug("%s pos %d (%d,%d)", s, u->entity_id.n, (int)u->x, (int)u->y);
}
*/

void message_debug(Message *m, const char *s) {
    switch(m->type) {
    case MESSAGE_CONNECT:
        log_debug("%sconnect", s);
        break;
    case MESSAGE_DISCONNECT:
        log_debug("%sdisconnect", s);
        break;
    case MESSAGE_JOIN:
        log_debug("%sjoin %d", s, m->join.player_id.n);
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
    case MESSAGE_SYNCED:
        log_debug("%ssynced", s);
        break;
    case MESSAGE_FULL:
        log_debug("%sfull", s);
        break;
    case MESSAGE_STATS:
        log_debug("%stats", s);
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
