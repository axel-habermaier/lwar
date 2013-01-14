// All connection functions return true on success and false on error, out parameters are used to return results

typedef char IPAddress[32];
typedef uint8_t Packet[512];

typedef struct 
{
	uint32_t ip;
	uint16_t port;
} EndPoint;

// Initializes and shuts down the network layer
bool net_init();
bool net_shutdown();

// Tries to read a received UDP data packet. Returns false if there was a socket error, otherwise writes the received
// data into the packet and returns the number of received bytes as well as the endpoint that sent the data.
// If no data has been received, size is 0 and the contents of packet and endpoint are undefined
bool net_receive(Packet packet, size_t* size, EndPoint* endPoint);

// Sends the data in the packet to the given endpoint. The number of bytes to send is indicates by the size parameter.
// Returns false if there was a socket error.
bool net_send(const Packet packet, size_t size, EndPoint* endPoint);

// Binds the socket to the given port so that it can start receiving incoming packets (server-only)
bool net_bind(uint16_t port);

// Creates an endpoint from a given IP adddress string and port
bool net_endpoint(EndPoint* endPoint, const IPAddress address, uint16_t port);