enum {
    APP_ID = 0xf27087c5,
    UPDATE_HEADER_LENGTH = 2 * sizeof(uint8_t), /* msg type and n */
    UPDATE_LENGTH = 2*sizeof(uint16_t)  /* Id */
                  + 4*sizeof(int16_t)   /* x,v */
                  + sizeof(uint16_t) + sizeof(uint8_t), /* rot,health */
    HEADER_LENGTH = 3*sizeof(uint32_t),
	MAX_PACKET_LENGTH = 512,
};

typedef struct Packet Packet;

struct Packet {
    /* some stores serialized messages in p+a...p+b */
    Address adr;

    /* allow some overflow: since string length is a byte, this can be max 256 + some backup */
    char    p[MAX_PACKET_LENGTH + MAX_NAME_LENGTH + 16];
    size_t  a,b;

    /* temp storage for incoming packets */
    size_t  ack, time;

    /* connection failed */
    int     io_failed;
};

int  packet_hasdata(Packet *p);

/* return how many updates + 1 header still fit into p */
size_t packet_update_n(Packet *p);

void packet_init(Packet *p, Address *adr, size_t ack, size_t time);

int  packet_put(Packet *p, Message *m, size_t seqno);
int  packet_get(Packet *p, Message *m, size_t *seqno);

int  packet_put_u(Packet *p, Update *u);
int  packet_get_u(Packet *p, Update *u);

int  packet_recv(Packet *p);
int  packet_send(Packet *p);
