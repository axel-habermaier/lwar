int conn_init();
int conn_bind();
void conn_shutdown();

int conn_recv(char *buf, size_t* size, Address* adr);
int conn_send(const char *buf, size_t size, Address* adr);
int address_create(Address *adr, const char *ip, uint16_t port);
