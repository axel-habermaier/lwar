#include <assert.h>
#include <stdint.h>
#include <string.h>

#include "uint.h"
#include "server.h"
#include "message.h"

int is_reliable(Message *m) {
    return    m->type != MESSAGE_HEADER
           && m->type < 100;
}

static size_t str_pack(char *out, const char *in, size_t n) {
    strncpy(out, in, n-1);
    out[n-1] = 0;
    return n;
}

static size_t str_unpack(const char *in, char *out, size_t n) {
    return str_pack(out, in, n);
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

size_t message_pack(char *s, Message *p, size_t seqno, size_t n) {
    size_t i=0;
    if(p->type != MESSAGE_HEADER) {
        if(n-i < sizeof(p->type)) return 0;
        i += uint8_pack(s+i, p->type);
    }
    if(is_reliable(p)) {
        if(n-i < sizeof(uint32_t)) return 0;
        i += uint32_pack(s+i, seqno);
    }
    switch(p->type) {
    case MESSAGE_SELECTION:
        if(n-i < sizeof(p->selection)) return 0;
        i += id_pack(s+i, p->selection.player_id);
        i += uint8_pack(s+i, p->selection.ship_type);
        i += uint8_pack(s+i, p->selection.weapon_type);
        break;
    case MESSAGE_HEADER:
        if(n-i < sizeof(p->header)) return 0;
        i += uint32_pack(s+i, p->header.app_id);
        i += uint32_pack(s+i, p->header.ack);
        i += uint32_pack(s+i, p->header.time);
        break;
    case MESSAGE_CONNECT:
        break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_FULL:
        break;
    case MESSAGE_JOIN:
        if(n-i < sizeof(p->join)) return 0;
        i += id_pack(s+i, p->join.player_id);
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_REMOVE:
        if(n-i < sizeof(p->remove)) return 0;
        i += id_pack(s+i, p->remove.entity_id);
        break;
    case MESSAGE_LEAVE:
        if(n-i < sizeof(p->leave)) return 0;
        i += id_pack(s+i, p->leave.player_id);
        break;
    case MESSAGE_ADD:
        if(n-i < sizeof(p->add)) return 0;
        i += id_pack(s+i, p->add.entity_id);
        i += id_pack(s+i, p->add.player_id);
        i += uint8_pack(s+i, p->add.type_id);
        break;
    case MESSAGE_INPUT:
        if(n-i < sizeof(p->input)) return 0;
        i += id_pack(s+i, p->input.player_id);
        i += uint8_pack(s+i, p->input.up);
        i += uint8_pack(s+i, p->input.down);
        i += uint8_pack(s+i, p->input.left);
        i += uint8_pack(s+i, p->input.right);
        i += uint8_pack(s+i, p->input.shooting);
        break;
    }
    return i;
}

size_t message_unpack(const char *s, Message *p, size_t *seqno, size_t n) {
    size_t i=0;
    if(n-i < sizeof(p->type)) return 0;
    i += uint8_unpack(s+i, &p->type);

    if(is_reliable(p)) {
        uint32_t tmp;
        if(n-i < sizeof(uint32_t)) return 0;
        i += uint32_unpack(s+i, &tmp);
        assert(seqno);
        *seqno = tmp;
    }
    switch(p->type) {
    case MESSAGE_SELECTION:
        if(n-i < sizeof(p->selection)) return 0;
        i += id_unpack(s+i, &p->selection.player_id);
        i += uint8_unpack(s+i, &p->selection.ship_type);
        i += uint8_unpack(s+i, &p->selection.weapon_type);
        break;
    case MESSAGE_HEADER:
        if(n-i < sizeof(p->header)) return 0;
        i += uint32_unpack(s+i, &p->header.app_id);
        i += uint32_unpack(s+i, &p->header.ack);
        i += uint32_unpack(s+i, &p->header.time);
        break;
    case MESSAGE_CONNECT:
        break;
    case MESSAGE_SYNCED:
        break;
    case MESSAGE_FULL:
        break;
    case MESSAGE_JOIN:
        if(n-i < sizeof(p->join)) return 0;
        i += id_unpack(s+i, &p->join.player_id);
        break;
    case MESSAGE_DISCONNECT:
        break;
    case MESSAGE_REMOVE:
        if(n-i < sizeof(p->remove)) return 0;
        i += id_unpack(s+i, &p->remove.entity_id);
        break;
    case MESSAGE_LEAVE:
        if(n-i < sizeof(p->leave)) return 0;
        i += id_unpack(s+i, &p->leave.player_id);
        break;
    case MESSAGE_ADD:
        if(n-i < sizeof(p->add)) return 0;
        i += id_unpack(s+i, &p->add.entity_id);
        i += id_unpack(s+i, &p->add.player_id);
        i += uint8_unpack(s+i, &p->add.type_id);
        break;
    case MESSAGE_INPUT:
        if(n-i < sizeof(p->input)) return 0;
        i += id_unpack(s+i, &p->input.player_id);
        i += uint8_unpack(s+i, &p->input.up);
        i += uint8_unpack(s+i, &p->input.down);
        i += uint8_unpack(s+i, &p->input.left);
        i += uint8_unpack(s+i, &p->input.right);
        i += uint8_unpack(s+i, &p->input.shooting);
        break;
    }
    return i;
}

/*
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
    log_info("%s @%d ", strmsg[m->type], m->seqno);

    switch(m->type) {
    case MESSAGE_CONNECT:
        log_info("%s", m->join.name);
        break;
    case MESSAGE_JOIN:
        log_info("%d ", m->join.player.n);
        log_info("%s", m->join.name);
        break;
    case MESSAGE_LEAVE:
        log_info("%d", m->leave.player.n);
        break;
    case MESSAGE_CHAT:
        log_info("%d ", m->chat.player.n);
        log_info("%.*s", m->chat.len, m->chat.text);
        break;
    case MESSAGE_INPUT:
        log_info("%d ", m->input.player.n);
        if(m->input.up)    log_info("↑");
        if(m->input.down)  log_info("↓");
        if(m->input.left)  log_info("←");
        if(m->input.right) log_info("→");
        if(m->input.shooting) log_info("*");
        log_info(" %d ", m->input.ack);
        break;
    case MESSAGE_ADD:
        log_info("%d ", m->add.entity.n);
        log_info("%d ", m->add.player.n);
        log_info("%d", m->add.type);
        break;
    case MESSAGE_REMOVE:
        log_info("%d", m->remove.entity.n);
        break;
    case MESSAGE_UPDATE:
        log_info("%d ", m->update.entity.n);
        log_info("(%d,%d) Δ(%d,%d) ", m->update.x, m->update.y, m->update.vx, m->update.vy);
        log_info("%d° %d%%", (int)(((float)((m->update.rot)*360)) / M_PI), m->update.health);
        break;
    }
    log_info("\n");
}
*/
