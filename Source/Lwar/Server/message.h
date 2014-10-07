#ifndef MESSAGE_H
#define MESSAGE_H

#include "address.h"
#include "id.h"
#include "str.h"
#include "config.h"

typedef enum   HeaderState HeaderState;
typedef enum   LeaveReason LeaveReason;
typedef enum   MessageType MessageType;
typedef enum   RejectReason RejectReason;

bool is_reliable(Message *m);
bool is_update(Message *m);

void message_add(Message *m, Entity *e);
void message_collision(Message *m, Collision *c);
void message_join(Message *m, Client *c);
void message_kill(Message *m, Player *k, Player *v);
void message_leave(Message *m, Client *c, LeaveReason reason);
void message_reject(Message *m, RejectReason reason);
void message_remove(Message *m, Entity *e);
void message_stats(Message *m);
void message_synced(Message *m);
void message_update(Message *m, MessageType type, size_t n); /* TODO: use data structure, e.g. Format. */

enum MessageType {
    MESSAGE_CONNECT         =   1,
    /* */
    MESSAGE_JOIN            =   3,
    MESSAGE_LEAVE           =   4,
    MESSAGE_CHAT            =   5,
    MESSAGE_ADD             =   6,
    MESSAGE_REMOVE          =   7,
    MESSAGE_SELECTION       =   8,
    MESSAGE_NAME            =   9,
    MESSAGE_SYNCED          =  10,
    MESSAGE_KILL            =  11,

    MESSAGE_STATS           = 101,
    /* */
    MESSAGE_INPUT           = 103,
    /* */            
    MESSAGE_COLLISION       = 105,
    MESSAGE_DISCONNECT      = 106,
    MESSAGE_REJECT          = 107,

    /* Note: adapt is_update in message.c after adding more updates */
    MESSAGE_UPDATE          = 110,
    MESSAGE_UPDATE_POS      = 111,
    MESSAGE_UPDATE_RAY      = 112,
    MESSAGE_UPDATE_CIRCLE   = 113,
    MESSAGE_UPDATE_SHIP     = 114,
};

enum {
    MESSAGE_DISCOVERY       = 200,
};

enum LeaveReason {
    LEAVE_QUIT              = 1,
    LEAVE_DROPPED           = 2,
    LEAVE_MISBEHAVED        = 3,
};

enum RejectReason {
    REJECT_FULL             = 1,
    REJECT_VERSION_MISMATCH = 2,
};

enum HeaderState {
    HEADER_OK,
    HEADER_APP_ID_MISMATCH,
    HEADER_IO_FAILED,
};

struct Header {
    uint32_t app_id;
    uint32_t ack;
    uint32_t time;
    Address  adr;
    HeaderState state;
};

struct Discovery {
    uint32_t type;
    uint32_t app_id;
    uint8_t  rev;
    uint16_t port;
};

struct Message {
    MessageType type;
    size_t seqno;

    union {
         struct {
            uint8_t rev;
            Str nick;
        } connect;

        struct {
            Id player_id;
            Str nick;
        } join;

        struct {
            Id player_id;
            LeaveReason reason;
        } leave;

        struct {
            Id player_id;
            Str msg;
        } chat;

        struct {
            Id entity_id;
            Id player_id;
            uint8_t type_id;
        } add;

        struct {
            Id entity_id;
        } remove;

        struct {
            Id player_id;
            uint8_t ship_type;
            uint8_t weapon_type1;
            uint8_t weapon_type2;
            uint8_t weapon_type3;
            uint8_t weapon_type4;
        } selection;

        struct {
            Id player_id;
            Str nick;
        } name;

        struct {
            Id killer_id;
            Id victim_id;
        } kill;

        struct {
            RejectReason reason;
        } reject;

        struct {
            uint8_t n;
            Format *f; /* Note: not directly serialized, needs special case in stream. */
        } update;

        struct {
            Id player_id;
            uint32_t frameno;
            uint8_t  forwards;
            uint8_t  backwards;
            uint8_t  turn_left;
            uint8_t  turn_right;
            uint8_t  strafe_left;
            uint8_t  strafe_right;
            uint8_t  fire1;
            uint8_t  fire2;
            uint8_t  fire3;
            uint8_t  fire4;
            int16_t  aim_x;
            int16_t  aim_y;
        } input;

        struct {
            uint8_t n;
            struct {
                Id player_id;
                uint16_t kills;
                uint16_t deaths;
                uint16_t ping;
            } info[MAX_CLIENTS];
        } stats;

        struct {
            Id entity_id[2];
            int16_t x;
            int16_t y;
        } collision;

        struct {
            uint32_t app_id;
            uint8_t  rev;
            uint16_t port;
        } discovery;
    };
};

#endif
