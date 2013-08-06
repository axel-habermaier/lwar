typedef enum   MessageType MessageType;
typedef struct Message Message;

size_t id_pack(char *out, Id id);
size_t id_unpack(const char *out, Id *id);

size_t header_pack(char *s, size_t app_id, size_t ack, size_t time);
size_t header_unpack(const char *s, size_t *app_id, size_t *ack, size_t *time);

size_t message_pack(char *s, void *p);
size_t message_unpack(const char *s, void *p);

/* void header_debug(Header *h, const char *s); */
void message_debug(Message *m, const char *s);
/* void update_debug(Update *u, const char *s); */

enum {
    MAX_NAME_LENGTH = 32,
    MAX_CHAT_LENGTH = 256,
};

enum MessageType {
    MESSAGE_CONNECT        =   1,
    MESSAGE_DISCONNECT     =   2,
    MESSAGE_JOIN           =   3,
    MESSAGE_LEAVE          =   4,
    MESSAGE_CHAT           =   5,
    MESSAGE_ADD            =   6,
    MESSAGE_REMOVE         =   7,
    MESSAGE_SELECTION      =   8,
    MESSAGE_NAME           =   9,
    MESSAGE_SYNCED         =  10,
	MESSAGE_KILL		   =  11,

    MESSAGE_STATS          = 101,
    /* */
    MESSAGE_INPUT          = 103,
    MESSAGE_FULL           = 104,
    MESSAGE_COLLISION      = 105,

    MESSAGE_UPDATE         = 110,
    MESSAGE_UPDATE_POS     = 111,
    MESSAGE_UPDATE_RAY     = 112,
    MESSAGE_UPDATE_CIRCLE  = 113,
};

enum LeaveReason {
	LEAVE_QUIT			= 1,
	LEAVE_DROPPED		= 2,
	LEAVE_MISBEHAVED	= 3
};

int is_reliable(Message *m);

struct Message {
    MessageType type;
    size_t seqno;

    union {
		 struct {
			Str nick;
        } connect;

        struct {
            Id player_id;
			Str nick;
        } join;

        struct {
            Id player_id;
			uint8_t reason;
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
            uint8_t n;
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
    };
};
