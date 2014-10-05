#include "message.h"

#include "uint.h"
#include "debug.h"
#include "log.h"

#include <stdlib.h>

static bool has_seqno(Message *m) {
	return m->type != MESSAGE_DISCOVERY;
}

bool is_reliable(Message *m) {
    return m->type < 100;
}

size_t message_pack(char *s, void *p) {
    Message *m = (Message *)p;
    size_t i=0;
    int j;

    i += uint8_pack(s+i, m->type);

	if (has_seqno(m)) {
		assert(m->seqno);
		i += uint32_pack(s+i, m->seqno);
	}
	else
		assert(!m->seqno);

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
	case MESSAGE_DISCOVERY:
		i += uint32_pack(s+i, m->discovery.app_id);
		i += uint8_pack(s+i, m->discovery.rev);
		i += uint16_pack(s+i, m->discovery.port);
		break;
    }
    return i;
}

size_t message_unpack(const char *s, void *p) {
    Message *m = (Message*)p;
    size_t i=0;
    int j;
    uint8_t _type;
    i += uint8_unpack(s+i, &_type);
    m->type = (MessageType)_type;

	if (has_seqno(m)) {
		uint32_t _seqno;
		i += uint32_unpack(s+i, &_seqno);
		assert(_seqno);
		m->seqno = _seqno;
	}
	else
		m->seqno = 0;

    switch(m->type) {
    case MESSAGE_CONNECT:
		i += uint8_unpack(s+i, &m->connect.rev);
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
		i += uint8_unpack(s+i, (uint8_t*)&m->leave.reason);
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
   case MESSAGE_REJECT:
		i += uint8_unpack(s+i, (uint8_t*)&m->reject.reason);
        break;
    case MESSAGE_STATS:
        i += uint8_unpack(s+i, &m->stats.n);
		for (j = 0; j < m->stats.n; ++j) {
			i += id_unpack(s+i, &m->stats.info[j].player_id);
			i += uint16_unpack(s+i, &m->stats.info[j].kills);
			i += uint16_unpack(s+i, &m->stats.info[j].deaths);
			i += uint16_unpack(s+i, &m->stats.info[j].ping);
		}
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
	case MESSAGE_DISCOVERY:
		i += uint32_unpack(s+i, &m->discovery.app_id);
		i += uint8_unpack(s+i, &m->discovery.rev);
		i += uint16_unpack(s+i, &m->discovery.port);
		break;
    }
    return i;
}
