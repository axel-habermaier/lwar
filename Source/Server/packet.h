enum {
    APP_ID = 0xf27087c5,
    HEADER_LENGTH = 3*sizeof(uint32_t),
	MAX_PACKET_LENGTH = 512,
};

typedef struct Packet Packet;

struct Packet {
    /* some stores serialized messages in p+a...p+b */
    Address adr;

    /* allow some overflow: since string length is a byte, this can be max 256 */
    char    p[MAX_PACKET_LENGTH * 256];
    size_t  a,b;

    /* temp storage for incoming packets */
    size_t  ack, time;

    /* connection failed */
    int     io_failed;
};

int  packet_hasdata(Packet *p);

void packet_init(Packet *p, Address *adr, size_t ack, size_t time);
int  packet_put(Packet *p, Message *m, size_t seqno);
int  packet_get(Packet *p, Message *m, size_t *seqno);

int  packet_recv(Packet *p);
int  packet_send(Packet *p);
