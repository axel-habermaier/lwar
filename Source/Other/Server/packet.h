typedef struct Packet Packet;

enum {
    APP_ID = 0xf27087c5,
    UPDATE_HEADER_LENGTH = 2 * sizeof(uint8_t),  /* msg type, n */
    HEADER_LENGTH        = 3 * sizeof(uint32_t), /* app_id, ack, time */
	MAX_PACKET_LENGTH    = 512,
};

struct Packet {
    /* some stores serialized messages in p+a...p+b */
    Address adr;

    /* allow some overflow: since string length is a byte, this can be max 256 + some backup */
    char    p[MAX_PACKET_LENGTH + MAX_NAME_LENGTH + 16];
    size_t  a,b;

    /* temp storage for incoming packets */
    size_t  ack, time;

    /* connection failed */
    bool    io_failed;
};

bool packet_hasdata(Packet *p);

/* return how many updates + 1 header still fit into p */
size_t packet_update_n(Packet *p, size_t len);

void packet_init(Packet *p, Address *adr, size_t ack, size_t time);

bool packet_put(Packet *p, Pack *pack, void *u);
bool packet_get(Packet *p, Unpack *unpack, void *u);

bool packet_recv(Packet *p);
bool packet_send(Packet *p);

void packet_debug(Packet *p);
