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

	connection.socket = socket(PF_INET6, SOCK_DGRAM, IPPROTO_UDP);
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

	int ipv6only = 0;
	if (setsockopt(connection.socket, IPPROTO_IPV6, IPV6_V6ONLY, (char*)&ipv6only, sizeof(ipv6only)) != 0)
	{
		conn_error("Unable to switch to dual-stack mode.");
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
	struct sockaddr_in6 addr;
	memset(&addr, 0, sizeof(addr));
	addr.sin6_family = AF_INET6;
	addr.sin6_addr = in6addr_any;
	addr.sin6_port = htons(SERVER_PORT);

	if (socket_error(bind(connection.socket, (struct sockaddr*)&addr, sizeof(addr))))
	{
		conn_error("Unable to bind socket.");
		conn_shutdown();
		return 0;
	}

	return 1;
}

int conn_recv(char *buf, size_t* size, Address* adr)
{
	struct sockaddr_storage from;
	struct sockaddr_in6* from6;
	struct sockaddr_in* from4;
	socklen_t len = sizeof(from);

	memset(&from, 0, len);
	
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
	
	switch (from.ss_family)
	{
	case AF_INET:
		from4 = (struct sockaddr_in*)&from;
		adr->port = from4->sin_port;
		memset(adr->ip, 0, sizeof(adr->ip));
		memcpy(adr->ip, &from4->sin_addr, sizeof(int32_t));
		adr->isIPv6 = false;
		break;
	case AF_INET6:
		from6 = (struct sockaddr_in6*)&from;
		adr->port = from6->sin6_port;
		memcpy(adr->ip, &from6->sin6_addr, sizeof(adr->ip));
		adr->isIPv6 = true;
		break;
	default:
		log_die("Unsupported address family.");
	}

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
	struct sockaddr_in addr4;
	struct sockaddr_in6 addr6;
	struct sockaddr* addr;
	socklen_t len;

	if (adr->isIPv6)
	{
		memset(&addr6, 0, sizeof(addr6));
		addr6.sin6_family = AF_INET6;
		addr6.sin6_port = adr->port;
		memcpy(&addr6.sin6_addr, adr->ip, sizeof(adr->ip));
		addr = (struct sockaddr*)&addr6;
		len = sizeof(addr6);
	}
	else
	{
		memset(&addr4, 0, sizeof(addr4));
		addr4.sin_family = AF_INET;
		addr4.sin_port = adr->port;
		memcpy(&addr4.sin_addr, adr->ip, sizeof(int32_t));
		addr = (struct sockaddr*)&addr4;
		len = sizeof(addr4);
	}

	int sent = sendto(connection.socket, buf, size, 0, addr, len);
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

bool address_eq(Address *adr0, Address *adr1) {
	int32_t i;

    if (adr0->port != adr1->port)
		return false;

	for (i = 0; i < sizeof(adr0->ip); ++i)
	{
		if (adr0->ip[0] != adr1->ip[1])
			return false;
	}

	return true;
}
