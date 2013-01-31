#include <assert.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>

#include "uint.h"
#include "server.h"
#include "message.h"

int is_reliable(Message *m) {
    return m->type < 100;
}

static size_t str_pack(char *out, Str in) {
    size_t i=0;
    i += uint8_pack(out+i, in.n);
    memcpy(out, in.s, in.n);
    return i + in.n;
}

static size_t str_unpack(const char *in, Str *out) {
    size_t i=0;
    i += uint8_unpack(in+i, &out->n);
    out->s = (char*)malloc(out->n);
    memcpy(out->s, in+i, out->n);
    return i + out->n;
}

static size_t id_pack(char *out, Id id) {
    uint16_pack(out,   id.gen);
    uint16_pack(out+2, id.n);
    return 4;
}

static size_t id_unpack(const char *out, Id *id) {
    uint16_unpack(out,   &id->gen);
    uint16_unpack(out+2, &id->n);
    return 4;
}

size_t header_pack(char *s, Header *h) {
    size_t i=0;
    i += uint32_pack(s+i, h->app_id);
    i += uint32_pack(s+i, h->ack);
    i += uint32_pack(s+i, h->time);
    return i;
}

size_t header_unpack(const char *s, Header *h) {
    size_t i=0;
    i += uint32_unpack(s+i, &h->app_id);
    i += uint32_unpack(s+i, &h->ack);
    i += uint32_unpack(s+i, &h->time);
    return i;
}

size_t message_pack(char *s, Message *m, size_t seqno) {
    size_t i=0;
    i += uint8_pack(s+i, m->type);

    if(is_reliable(m)) {
        i += uint32_pack(s+i, seqno);
    }

    switch(m->type) {
    case MESSAGE_CONNECT:
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_JOIN:
        i += id_pack(s+i, m->join.player_id);
        break;
    case MESSAGE_LEAVE:
        i += id_pack(s+i, m->leave.player_id);
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
        i += uint8_pack(s+i, m->selection.weapon_type);
        break;
    case MESSAGE_NAME:
        i += id_pack(s+i, m->name.player_id);
        i += str_pack(s+i, m->name.nick);
        break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_FULL:
        break;
    case MESSAGE_INPUT:
        i += id_pack(s+i, m->input.player_id);
        i += uint32_pack(s+i, m->input.frameno);
        i += uint8_pack(s+i, m->input.up);
        i += uint8_pack(s+i, m->input.down);
        i += uint8_pack(s+i, m->input.left);
        i += uint8_pack(s+i, m->input.right);
        i += uint8_pack(s+i, m->input.shooting);
        break;
    }
    return i;
}

size_t message_unpack(const char *s, Message *m, size_t *seqno) {
    size_t i=0;
    i += uint8_unpack(s+i, &m->type);

    if(is_reliable(m)) {
        uint32_t tmp;
        i += uint32_unpack(s+i, &tmp);
        assert(seqno);
        *seqno = tmp;
    }

    switch(m->type) {
    case MESSAGE_CONNECT:
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_JOIN:
        i += id_unpack(s+i, &m->join.player_id);
        break;
    case MESSAGE_LEAVE:
        i += id_unpack(s+i, &m->leave.player_id);
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
        i += uint8_unpack(s+i, &m->selection.weapon_type);
        break;
    case MESSAGE_NAME:
        i += id_unpack(s+i, &m->name.player_id);
        i += str_unpack(s+i, &m->name.nick);
        break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_FULL:
        break;
    case MESSAGE_INPUT:
        i += id_unpack(s+i, &m->input.player_id);
        i += uint32_unpack(s+i, &m->input.frameno);
        i += uint8_unpack(s+i, &m->input.up);
        i += uint8_unpack(s+i, &m->input.down);
        i += uint8_unpack(s+i, &m->input.left);
        i += uint8_unpack(s+i, &m->input.right);
        i += uint8_unpack(s+i, &m->input.shooting);
        break;
    }
    return i;
}

