#include <assert.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#ifdef __unix__
#include <arpa/inet.h>
#include <errno.h>
#include <fcntl.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>

typedef int Socket;
#define socket_error(s)   ((s) < 0)
#define socket_invalid(s) ((s) < 0)
#define closesocket close
#endif

#ifdef _MSC_VER
#include <winsock2.h>
#include <ws2tcpip.h>

typedef SOCKET Socket;
#define socket_error(s)   ((s) == SOCKET_ERROR)
#define socket_invalid(s) ((s) == INVALID_SOCKET)
#endif

#define socket_valid(s)   (! socket_invalid(s))

#include "server.h"
#include "connection.h"
#include "log.h"

enum 
{
	SERVER_PORT = 32422,
	/* IP_STRLENGTH = 22, */
};

typedef struct
{
	Socket socket;
	int initialized;
} Connection;

static Connection connection;

static void conn_error(const char* const msg)
{
	char errmsg[1024];
#ifdef _MSC_VER
	int error;
	error = WSAGetLastError();

	if (FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, NULL, (error), NULL, errmsg, sizeof(errmsg), NULL) == 0) 
		strcpy(errmsg, "Unknown error.");
#endif

#ifdef __unix__
    strerror_r(errno, errmsg, sizeof(errmsg));
#endif

	log_error("%s - %s\n", msg, errmsg);
}

int conn_init()
{
#ifdef _MSC_VER
	WSADATA wsaData; 
	
	if (WSAStartup(MAKEWORD(1,1), &wsaData) != 0)
	{
		conn_error("Winsock startup failed.");
		return 0;
	}
#endif

	connection.socket = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (socket_invalid(connection.socket))
	{
		conn_error("Unable to initialize socket.");
		conn_shutdown();
		return 0;
	}

	connection.initialized = 1;

#ifdef _MSC_VER
	u_long _true = 1;
	if (socket_error(ioctlsocket(connection.socket, FIONBIO, &_true)))
#endif

#ifdef __unix__
	if (socket_error(fcntl(connection.socket, F_SETFL, fcntl(connection.socket, F_GETFL, 0) | O_NDELAY)))
#endif
	{
		conn_error("Unable to switch to non-blocking mode.");
		conn_shutdown();
		return 0;
	}

	return 1;
}

void conn_shutdown()
{
	if (connection.initialized == 0)
		return;

	if (socket_error(closesocket(connection.socket)))
		conn_error("Unable to close socket.");

	connection.initialized = 0;
#ifdef _MSC_VER
	WSACleanup();
#endif
}

int conn_bind()
{
	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = INADDR_ANY;
	addr.sin_port = htons(SERVER_PORT);

	if (socket_error(bind(connection.socket, (struct sockaddr*)&addr, sizeof(struct sockaddr_in))))
	{
		conn_error("Unable to bind socket.");
		conn_shutdown();
		return 0;
	}

	return 1;
}

int conn_recv(char *buf, size_t* size, Address* adr)
{
	struct sockaddr_in from;
	memset(&from, 0, sizeof(struct sockaddr_in));
	socklen_t len = sizeof(struct sockaddr_in);

	int read_bytes = recvfrom(connection.socket, buf, *size, 0, (struct sockaddr*)&from, &len);
#ifdef _MSC_VER
	if (WSAGetLastError() == WSAEWOULDBLOCK)
#endif
#ifdef __unix__
	if (socket_error(read_bytes) && errno == EAGAIN)
#endif
	{
		*size = 0;
		return 1;
	}

	adr->ip = from.sin_addr.s_addr;
	adr->port = from.sin_port;
	if (socket_error(read_bytes))
	{
		conn_error("Receiving failed.");
		return 0;
	}

	*size = read_bytes;
	
	return 1;
}

int conn_send(const char *buf, size_t size, Address* adr)
{
	struct sockaddr_in addr;
	memset(&addr, 0, sizeof(struct sockaddr_in));
	addr.sin_family = AF_INET;
	addr.sin_port = adr->port;
	addr.sin_addr.s_addr = adr->ip;

	int sent = sendto(connection.socket, buf, size, 0, (struct sockaddr*)&addr, sizeof(struct sockaddr_in));
	if (socket_error(sent))
	{
		conn_error("Sending failed");
		return 0;
	}

	if (sent != size)
	{
		conn_error ("Message was sent only partially.");
		return 0;
	}

	return 1;
}

int address_create(Address *adr, const char *ip, uint16_t port) {
	adr->port = htons(port);
	return inet_pton(AF_INET, ip, &adr->ip);
}

bool address_eq(Address *adr0, Address *adr1) {
    return    adr0->ip   == adr1->ip
           && adr0->port == adr1->port;
}
