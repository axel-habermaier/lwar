#include "prelude.h"
#include "network.h"

#ifdef WINDOWS
#include <winsock2.h>
#include <ws2tcpip.h>
#endif

#ifdef LINUX
#include <arpa/inet.h>
#include <errno.h>
#include <fcntl.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>

typedef int SOCKET;
#define INVALID_SOCKET -1
#define SOCKET_ERROR   -1
#endif

typedef SOCKET Socket;
typedef struct sockaddr_in sockaddr_in;
typedef struct sockaddr sockaddr;

static Socket udp_socket;

#define APP_ID 0xf27087c5,
typedef uint32_t AppId;

// ---------------------------------------------------------------------------------------------------------------------
// Helper functions

// Logs a socket error and closes the faulted socket, always returns false for convenience
static bool net_error(const char* const msg);

// Initializes the socket and sets it to non-blocking mode
static bool net_init_socket();

// Closes the socket
static bool net_close_socket();

static bool net_error(const char* const msg)
{
	char errmsg[1024];

#ifdef WINDOWS
	int error = WSAGetLastError();

	if (FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, NULL, (error), NULL, errmsg, sizeof(errmsg), NULL) == 0) 
		strcpy(errmsg, "Unknown error.");
#endif

#ifdef LINUX
    strerror_r(errno, errmsg, sizeof(errmsg));
#endif

	LWAR_DEBUG("%s - (%s)", msg, errmsg);
	net_close_socket();
	return false;
}

static bool net_init_socket()
{
	if (udp_socket != INVALID_SOCKET)
		return true;

	net_close_socket();

	udp_socket = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (udp_socket == INVALID_SOCKET)
		return net_error("Unable to initialize socket.");

#ifdef WINDOWS
	u_long nonBlocking = 1;
	if (ioctlsocket(udp_socket, FIONBIO, &nonBlocking) == SOCKET_ERROR)
#endif

#ifdef LINUX
	if (fcntl(udp_socket, F_SETFL, fcntl(udp_socket, F_GETFL, 0) | O_NDELAY) == SOCKET_ERROR)
#endif
		return net_error("Unable to switch to non-blocking mode.");

	return true;
}

static bool net_close_socket()
{
	if (udp_socket == INVALID_SOCKET)
		return true;

#ifdef WINDOWS
	if (closesocket(udp_socket) == SOCKET_ERROR)
#endif

#ifdef LINUX
	if (close(udp_socket) == SOCKET_ERROR)
#endif
	{
		udp_socket = INVALID_SOCKET; // Prevents endless recursion
		return net_error("Unable to close socket.");
	}

	udp_socket = INVALID_SOCKET;
	return true;
}

// ---------------------------------------------------------------------------------------------------------------------
// Public API

bool net_init()
{
#ifdef WINDOWS
	WSADATA wsaData; 
	
	if (WSAStartup(MAKEWORD(1,1), &wsaData) != 0)
		return net_error("Winsock startup failed.");
#endif

	return true;
}

bool net_shutdown()
{
	bool ret = net_close_socket();

#ifdef WINDOWS
	ret &= WSACleanup() == 0;
#endif

	return ret;
}

bool net_receive(Packet* packet, size_t* size, EndPoint* endPoint)
{
	net_init_socket();

	sockaddr_in from;
	memset(&from, 0, sizeof(sockaddr_in));
	socklen_t len = sizeof(sockaddr_in);

	int read_bytes = recvfrom(udp_socket, (char*)packet->data, sizeof(packet), 0, (sockaddr*)&from, &len);
#ifdef WINDOWS
	if (WSAGetLastError() == WSAEWOULDBLOCK)
#endif
#ifdef LINUX
	if (socket_error(read_bytes) && errno == EAGAIN)
#endif
	{
		*size = 0;
		return true;
	}

	if (read_bytes == SOCKET_ERROR)
		return net_error("Receiving failed.");

	*size = read_bytes;
	endPoint->ip = from.sin_addr.s_addr;
	endPoint->port = ntohs(from.sin_port);

	return true;
}

bool net_send(const Packet* packet, size_t size, EndPoint* endPoint)
{
	net_init_socket();

	sockaddr_in addr;
	memset(&addr, 0, sizeof(sockaddr_in));
	addr.sin_family = AF_INET;
	addr.sin_port = htons(endPoint->port);
	addr.sin_addr.s_addr = endPoint->ip;

	int sent_bytes = sendto(udp_socket, (char*)packet->data, size, 0, (sockaddr*)&addr, sizeof(sockaddr_in));
	if (sent_bytes == SOCKET_ERROR)
		return net_error("Sending failed");

	LWAR_ASSERT(sent_bytes >= 0, "We've caught the error, so sent_bytes should be positive");
	if ((size_t)sent_bytes != size)
		return net_error("Message was sent only partially.");

	return true;
}

bool net_bind(uint16_t port)
{
	net_init_socket();

	sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = INADDR_ANY;
	addr.sin_port = htons(port);

	if (bind(udp_socket, (sockaddr*)&addr, sizeof(sockaddr_in)) == SOCKET_ERROR)
		return net_error("Unable to bind socket.");

	return true;
}

bool net_endpoint(EndPoint* endPoint, const IPAddress address, uint16_t port)
{
	endPoint->port = port;
	return inet_pton(AF_INET, address, &endPoint->ip) == 1;
}

void net_init_packet(Packet* packet)
{
	LWAR_ASSERT_NOT_NULL(packet);
	buffer_init(&packet->buffer, packet->data, sizeof(AppId), sizeof(packet->data) - sizeof(AppId));
}