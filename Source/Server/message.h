typedef struct Message Message;

size_t message_pack(char *s, Message *m, size_t len);
size_t message_unpack(const char *s, Message *m, size_t len);

void message_print(Message *m);

enum {
    APP_ID     = 0xf27087c5,
    SCALE_BITS = 8,
    SCALE      = (1 << SCALE_BITS),
    MAX_NAME_LENGTH = 32,
    MAX_CHAT_LENGTH = 500,
};

enum {
    MESSAGE_CONNECT,
    MESSAGE_JOIN,
    MESSAGE_LEAVE,
    MESSAGE_CHAT,

    MESSAGE_INPUT,

    MESSAGE_ADD,
    MESSAGE_REMOVE,
    MESSAGE_UPDATE,
};

#define IS_RELIABLE(t) \
    ((t) != MESSAGE_INPUT && (t) != MESSAGE_UPDATE)

struct Message {
    /* uint32_t app_id; */
    uint8_t type;

    union {
        uint32_t seqno;
        uint32_t time; /* used only for update messages */
    };

    union {
        struct {
            Id player;
            char name[MAX_NAME_LENGTH];
        } join;

        struct {
            Id player;
        } leave;

        struct {
            Id player;
            uint16_t len;
            char text[MAX_CHAT_LENGTH]; /* could be little bit shorter */
        } chat;

        struct {
            Id player;
            uint8_t  up,down,left,right,shooting;
            uint32_t ack;
        } input;

        struct {
            Id entity;
            Id player;
            uint8_t type;
        } add;

        struct {
            Id entity;
        } remove;

        struct {
            Id entity;
            int32_t  x, y;   /* scaled by SCALE */
            int32_t  vx,vy;
            uint16_t rot;    /* in radians, scaled by SCALE */
            uint8_t  health; /* in percent */
        } update;
    };
};
