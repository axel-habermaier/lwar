#include "prelude.h"
#include "message.h"

#include <string.h>

static void id_pack(Buffer* buffer, Id id)
{
	buffer_write_uint16(buffer, id.n);
	buffer_write_uint16(buffer, id.gen);
}

size_t message_pack(Buffer* buffer, Message* m)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(m);

	if (!buffer_fits(buffer, sizeof(m->type) + sizeof(m->seqno)))
		return 0;

	size_t offset = buffer_tell(buffer);

	buffer_write_uint8(buffer, m->type);
	buffer_write_uint32(buffer, m->seqno);

	switch (m->type)
	{
	case MESSAGE_CONNECT:
    case MESSAGE_JOIN:
		if (!buffer_fits(buffer, sizeof(m->join.player) + sizeof(m->join.name)))
			return 0;

		id_pack(buffer, m->join.player);
		buffer_write_array(buffer, (uint8_t*)m->join.name, sizeof(m->join.name));
		break;
    case MESSAGE_LEAVE:
		break;
    case MESSAGE_CHAT:
		break;
    case MESSAGE_INPUT:
		break;
    case MESSAGE_ADD:
		break;
    case MESSAGE_REMOVE:
		break;
    case MESSAGE_UPDATE:
		break;
	default:
		LWAR_NO_SWITCH_DEFAULT;
	}

	return buffer_tell(buffer) - offset;
}

//static size_t str_pack(char *out, const char *in, size_t n) {
//    strncpy(out, in, n-1);
//    out[n-1] = 0;
//    return n;
//}
//
//static size_t str_unpack(const char *in, char *out, size_t n) {
//    return str_pack(out, in, n);
//}
//
//#define name_pack(out, in)   str_pack(out, in, MAX_NAME_LENGTH)
//#define name_unpack(out, in) str_unpack(out, in, MAX_NAME_LENGTH)
//
//static size_t id_pack(char *out, Id id) {
//    uint16_pack(out,   id.gen);
//    uint16_pack(out+2, id.n);
//    return 4;
//}
//
//static size_t id_unpack(const char *out, Id *id) {
//    uint16_unpack(out,   &id->gen);
//    uint16_unpack(out+2, &id->n);
//    return 4;
//}
//
//size_t message_pack(char *s, Message *m, size_t len) {
//    size_t n = 0;
//
//    if(n+11 > len) return 0;
//    n += uint32_pack(s+n, APP_ID);
//    n += uint8_pack (s+n, m->type);
//    n += uint32_pack(s+n, m->seqno);
//
//    switch(m->type) {
//    case MESSAGE_CONNECT:
//    case MESSAGE_JOIN:
//        if(n + 4+MAX_NAME_LENGTH > len) return 0;
//        n += id_pack(s+n, m->join.player);
//        n += name_pack(s+n, m->join.name);
//        break;
//    case MESSAGE_LEAVE:
//        if(n + 4 > len) return 0;
//        n += id_pack(s+n, m->leave.player);
//        break;
//    case MESSAGE_CHAT:
//        if(n + 6+m->chat.len > len) return 0;
//        n += id_pack(s+n, m->chat.player);
//        n += uint16_pack(s+n, m->chat.len);
//        n += str_pack(s+n, m->chat.text, m->chat.len);
//        break;
//    case MESSAGE_INPUT:
//        if(n + 13 > len) return 0;
//        n += id_pack(s+n, m->input.player);
//        n += uint8_pack(s+n, m->input.up);
//        n += uint8_pack(s+n, m->input.down);
//        n += uint8_pack(s+n, m->input.left);
//        n += uint8_pack(s+n, m->input.right);
//        n += uint8_pack(s+n, m->input.shooting);
//        n += uint32_pack(s+n, m->input.ack);
//        break;
//    case MESSAGE_ADD:
//        if(n + 9 > len) return 0;
//        n += id_pack(s+n, m->add.entity);
//        n += id_pack(s+n, m->add.player);
//        n += uint8_pack(s+n, m->add.type);
//        break;
//    case MESSAGE_REMOVE:
//        if(n + 4 > len) return 0;
//        n += id_pack(s+n, m->remove.entity);
//        break;
//    case MESSAGE_UPDATE:
//        if(n + 23 > len) return 0;
//        n += id_pack(s+n, m->update.entity);
//        n +=  int32_pack(s+n, m->update.x);
//        n +=  int32_pack(s+n, m->update.y);
//        n +=  int32_pack(s+n, m->update.vx);
//        n +=  int32_pack(s+n, m->update.vy);
//        n += uint16_pack(s+n, m->update.rot);
//        n += uint8_pack (s+n, m->update.health);
//        break;
//    }
//    return n;
//}
//
//size_t message_unpack(const char *s, Message *m, size_t len) {
//    size_t n = 0;
//    uint32_t app_id;
//    n += uint32_unpack(s+n, &app_id);
//    if(app_id != APP_ID) return 0;
//    n += uint8_unpack (s+n, &m->type);
//    n += uint32_unpack(s+n, &m->seqno);
//
//    switch(m->type) {
//    case MESSAGE_CONNECT:
//    case MESSAGE_JOIN:
//        if(n + 4+MAX_NAME_LENGTH > len) return 0;
//        n += id_unpack(s+n, &m->join.player);
//        n += name_unpack(s+n, m->join.name);
//        break;
//    case MESSAGE_LEAVE:
//        if(n + 4 > len) return 0;
//        n += id_unpack(s+n, &m->leave.player);
//        break;
//    case MESSAGE_CHAT:
//        if(n + 6 > len) return 0;
//        n += id_unpack(s+n, &m->chat.player);
//        n += uint16_unpack(s+n, &m->chat.len);
//        if(n + m->chat.len > len) return 0;
//        n += str_unpack(s+n, m->chat.text, m->chat.len);
//        break;
//    case MESSAGE_INPUT:
//        if(n + 13 > len) return 0;
//        n += id_unpack(s+n, &m->input.player);
//        n += uint8_unpack(s+n, &m->input.up);
//        n += uint8_unpack(s+n, &m->input.down);
//        n += uint8_unpack(s+n, &m->input.left);
//        n += uint8_unpack(s+n, &m->input.right);
//        n += uint8_unpack(s+n, &m->input.shooting);
//        n += uint32_unpack(s+n, &m->input.ack);
//        break;
//    case MESSAGE_ADD:
//        n += id_unpack(s+n, &m->add.entity);
//        n += id_unpack(s+n, &m->add.player);
//        n += uint8_unpack(s+n, &m->add.type);
//        break;
//    case MESSAGE_REMOVE:
//        if(n + 4 > len) return 0;
//        n += id_unpack(s+n, &m->remove.entity);
//        break;
//    case MESSAGE_UPDATE:
//        if(n + 23 > len) return 0;
//        n += id_unpack(s+n, &m->update.entity);
//        n +=  int32_unpack(s+n, &m->update.x);
//        n +=  int32_unpack(s+n, &m->update.y);
//        n +=  int32_unpack(s+n, &m->update.vx);
//        n +=  int32_unpack(s+n, &m->update.vy);
//        n += uint16_unpack(s+n, &m->update.rot);
//        n += uint8_unpack (s+n, &m->update.health);
//        break;
//    }
//    return n;
//}

static const char *strmsg[] = {
    "connect",
    "join",
    "leave",
    "chat",
    "input",
    "add",
    "remove",
    "update",
};

void message_print(Message *m) {
    sys_printf("%s @%d ", strmsg[m->type], m->seqno);

    switch(m->type) {
    case MESSAGE_CONNECT:
        sys_printf("%s", m->join.name);
        break;
    case MESSAGE_JOIN:
        sys_printf("%d ", m->join.player.n);
        sys_printf("%s", m->join.name);
        break;
    case MESSAGE_LEAVE:
        sys_printf("%d", m->leave.player.n);
        break;
    case MESSAGE_CHAT:
        sys_printf("%d ", m->chat.player.n);
        sys_printf("%.*s", strlen(m->chat.text), m->chat.text);
        break;
    case MESSAGE_INPUT:
        sys_printf("%d ", m->input.player.n);
        if(m->input.up)    sys_printf("↑");
        if(m->input.down)  sys_printf("↓");
        if(m->input.left)  sys_printf("←");
        if(m->input.right) sys_printf("→");
        if(m->input.shooting) sys_printf("*");
        sys_printf(" %d ", m->input.ack);
        break;
    case MESSAGE_ADD:
        sys_printf("%d ", m->add.entity.n);
        sys_printf("%d ", m->add.player.n);
        sys_printf("%d", m->add.type);
        break;
    case MESSAGE_REMOVE:
        sys_printf("%d", m->remove.entity.n);
        break;
    case MESSAGE_UPDATE:
        sys_printf("%d ", m->update.entity.n);
        sys_printf("(%d,%d) Δ(%d,%d) ", m->update.x, m->update.y, m->update.vx, m->update.vy);
        sys_printf("%d° %d%%", (int)(((float)((m->update.rot)*360)) / M_PI), m->update.health);
        break;
    }
    sys_printf("\n");
}
