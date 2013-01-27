enum 
{
	SERVER_PORT = 32422,
	MAX_PACKET_LENGTH = 512,
	IP_STRLENGTH = 22,
};

void conn_shutdown();
int conn_init();
int conn_receive(char buffer[MAX_PACKET_LENGTH], size_t* size, Address* address);
int conn_send(const char buffer[MAX_PACKET_LENGTH], size_t size, Address* address);
int conn_create_addr(Address* addr, const char ip[IP_STRLENGTH], uint16_t port);
int conn_bind();
