typedef enum   MessageType MessageType;
typedef struct Header  Header ;
typedef struct Message Message;

size_t header_pack(char *s, Header *h);
size_t header_unpack(const char *s, Header *h);

size_t message_pack(char *s, Message *m, size_t seqno);
size_t message_unpack(const char *s, Message *m, size_t *seqno);

void message_debug(Message *m, const char *s);

enum {
    SCALE_BITS = 8,
    SCALE      = (1 << SCALE_BITS),
    MAX_NAME_LENGTH = 32,
    MAX_CHAT_LENGTH = 500,
};

enum MessageType {
    MESSAGE_CONNECT     = 1,
    MESSAGE_DISCONNECT  = 2,
    MESSAGE_JOIN        = 3,
    MESSAGE_LEAVE       = 4,
    MESSAGE_CHAT        = 5,
    MESSAGE_ADD         = 6,
    MESSAGE_REMOVE      = 7,
    MESSAGE_SELECTION   = 8,
    MESSAGE_NAME        = 9,
    MESSAGE_SYNCED      = 10,

    MESSAGE_INPUT       = 103,
    MESSAGE_FULL        = 104,
};

int is_reliable(Message *m);

struct Header {
    uint32_t app_id;
    uint32_t ack;
    uint32_t time;
};

struct Message {
    MessageType type;

    union {
        struct {
            Id player_id;
        } join;

        struct {
            Id player_id;
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
            uint8_t weapon_type;
        } selection;

        struct {
            Id player_id;
            Str nick;
        } name;

        struct {
            Id player_id;
            uint32_t frameno;
            uint8_t up;
            uint8_t down;
            uint8_t left;
            uint8_t right;
            uint8_t shooting;
        } input;
    };
};
