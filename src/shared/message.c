#include "prelude.h"
#include "message.h"

#include <string.h>

static void id_pack(Buffer* buffer, Id id)
{
	buffer_write_uint16(buffer, id.n);
	buffer_write_uint16(buffer, id.gen);
}

static Id id_unpack(Buffer* buffer)
{
	Id id;
	id.n = buffer_read_uint16(buffer);
	id.gen = buffer_read_uint16(buffer);
	return id;
}

#define ID_SIZE 2 * 2
#define PLAYERNAME_SIZE 32
#define COMMON_PREFIX_SIZE 1 + 4

static size_t msg_len[] = 
{
	ID_SIZE + PLAYERNAME_SIZE, // connect
	ID_SIZE + PLAYERNAME_SIZE, // join
	ID_SIZE, // leave
	ID_SIZE + 2, // chat + variable length msg
	ID_SIZE + 5 + 4, // input
	ID_SIZE + ID_SIZE + 1, // add
	ID_SIZE, // remove
	ID_SIZE + 4 * 4 + 2 + 1 // update
};

size_t message_pack(Buffer* buffer, Message* m)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(m);

	size_t req_size = msg_len[m->type] + COMMON_PREFIX_SIZE;
	if (m->type == MESSAGE_CHAT)
		req_size += strlen(m->chat.text);

	if (!buffer_fits(buffer, req_size))
		return 0;

	size_t offset = buffer_tell(buffer);

	buffer_write_uint8(buffer, m->type);
	buffer_write_uint32(buffer, m->seqno);

	switch (m->type)
	{
	case MESSAGE_CONNECT:
    case MESSAGE_JOIN:
		id_pack(buffer, m->join.player);
		buffer_write_array(buffer, (uint8_t*)m->join.name, sizeof(m->join.name));
		break;
    case MESSAGE_LEAVE:
		id_pack(buffer, m->leave.player);
		break;
    case MESSAGE_CHAT:
	{
		size_t len = strlen(m->chat.text);
		buffer_write_uint16(buffer, (uint16_t)len);
		buffer_write_array(buffer, (uint8_t*)m->chat.text, len);
		id_pack(buffer, m->chat.player);
		break;
	}
    case MESSAGE_INPUT:
		id_pack(buffer, m->input.player);
		buffer_write_uint32(buffer, m->input.ack);
		buffer_write_bool(buffer, m->input.up);
		buffer_write_bool(buffer, m->input.down);
		buffer_write_bool(buffer, m->input.left);
		buffer_write_bool(buffer, m->input.right);
		buffer_write_bool(buffer, m->input.shooting);
		break;
    case MESSAGE_ADD:
		id_pack(buffer, m->add.entity);
		id_pack(buffer, m->add.player);
		buffer_write_uint8(buffer, m->add.type);
		break;
    case MESSAGE_REMOVE:
		id_pack(buffer, m->remove.entity);
		break;
    case MESSAGE_UPDATE:
		id_pack(buffer, m->update.entity);
		buffer_write_int32(buffer, m->update.x);
		buffer_write_int32(buffer, m->update.y);
		buffer_write_int32(buffer, m->update.vx);
		buffer_write_int32(buffer, m->update.vy);
		buffer_write_uint16(buffer, m->update.rot);
		buffer_write_uint8(buffer, m->update.health);
		break;
	default:
		LWAR_NO_SWITCH_DEFAULT;
	}

	return buffer_tell(buffer) - offset;
}

size_t message_unpack(Buffer* buffer, Message* m)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(m);

	if (!buffer_fits(buffer, COMMON_PREFIX_SIZE))
		return 0;

	m->type = buffer_read_uint8(buffer);
	m->seqno = buffer_read_uint32(buffer);

	size_t req_size = msg_len[m->type];
	size_t chat_size = 0;
	if (m->type == MESSAGE_CHAT)
		chat_size = buffer_read_uint16(buffer);

	if (!buffer_fits(buffer, req_size + chat_size))
		return 0;

	switch (m->type)
	{
	case MESSAGE_CONNECT:
    case MESSAGE_JOIN:
		m->join.player = id_unpack(buffer);
		buffer_read_array(buffer, (uint8_t*)m->join.name, sizeof(m->join.name));
		break;
    case MESSAGE_LEAVE:
		m->leave.player = id_unpack(buffer);
		break;
    case MESSAGE_CHAT:
	{
		buffer_read_array(buffer, (uint8_t*)m->chat.text, chat_size);
		m->chat.player = id_unpack(buffer);
		break;
	}
    case MESSAGE_INPUT:
		m->input.player = id_unpack(buffer);
		m->input.ack = buffer_read_uint32(buffer);
		m->input.up = buffer_read_bool(buffer);
		m->input.down = buffer_read_bool(buffer);
		m->input.left = buffer_read_bool(buffer);
		m->input.right = buffer_read_bool(buffer);
		m->input.shooting = buffer_read_bool(buffer);
		break;
    case MESSAGE_ADD:
		m->add.entity = id_unpack(buffer);
		m->add.player = id_unpack(buffer);
		m->add.type = buffer_read_uint8(buffer);
		break;
    case MESSAGE_REMOVE:
		m->remove.entity = id_unpack(buffer);
		break;
    case MESSAGE_UPDATE:
		m->update.entity = id_unpack(buffer);
		m->update.x = buffer_read_int32(buffer);
		m->update.y = buffer_read_int32(buffer);
		m->update.vx = buffer_read_int32(buffer);
		m->update.vy = buffer_read_int32(buffer);
		m->update.rot = buffer_read_uint16(buffer);
		m->update.health = buffer_read_uint8(buffer);
		break;
	default:
		LWAR_NO_SWITCH_DEFAULT;
	}

	return req_size + chat_size;
}

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
