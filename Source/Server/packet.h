enum {
    APP_ID = 0xf27087c5,
	MAX_PACKET_LENGTH = 512,
};

typedef struct Packet Packet;

struct Packet {
    /* some stores serialized messages in p+a...p+b */
    Address adr;
    char    p[MAX_PACKET_LENGTH];
    size_t  a,b;
};

void packet_init(Packet *p);
int  packet_put(Packet *p, Message *m, size_t seqno);
int  packet_get(Packet *p, Message *m, size_t *seqno);

int  packet_recv(Packet *p);
int  packet_send(Packet *p);
