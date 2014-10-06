#include "types.h"

#include "connection.h"

#include "config.h"
#include "debug.h"
#include "log.h"

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

/* Unix */
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


/* Windows */
#ifdef _MSC_VER
#include <winsock2.h>
#include <ws2tcpip.h>

typedef SOCKET Socket;
#define socket_error(s)   ((s) == SOCKET_ERROR)
#define socket_invalid(s) ((s) == INVALID_SOCKET)
#endif


#define socket_valid(s)   (! socket_invalid(s))

static int numConnections = 0;

struct Connection {
	Socket socket;
};

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

	log_error("%s %s\n", msg, errmsg);
}

bool conn_init(Connection* connection)
{
#ifdef _MSC_VER
	if (numConnections == 0)
	{
		WSADATA wsaData; 
	
		if (WSAStartup(MAKEWORD(1,1), &wsaData) != 0)
		{
			conn_error("Winsock startup failed.");
			return false;
		}
	}
#endif

	memset(connection, 0, sizeof(Connection));
	++numConnections;

	connection->socket = socket(PF_INET6, SOCK_DGRAM, IPPROTO_UDP);
	if (socket_invalid(connection->socket))
	{
		conn_error("Unable to initialize socket.");
		conn_shutdown(connection);
		return false;
	}

#ifdef _MSC_VER
	u_long _true = 1;
	if (socket_error(ioctlsocket(connection->socket, FIONBIO, &_true)))
#endif

#ifdef __unix__
	if (socket_error(fcntl(connection->socket, F_SETFL, fcntl(connection->socket, F_GETFL, 0) | O_NDELAY)))
#endif
	{
		conn_error("Unable to switch to non-blocking mode.");
		conn_shutdown(connection);
		return false;
	}

	int ipv6only = 0;
	if (setsockopt(connection->socket, IPPROTO_IPV6, IPV6_V6ONLY, (char*)&ipv6only, sizeof(ipv6only)) != 0)
	{
		conn_error("Unable to switch to dual-stack mode.");
		conn_shutdown(connection);
		return false;
	}

	return true;
}

void conn_shutdown(Connection* connection)
{
	assert(numConnections > 0);

	if (socket_error(closesocket(connection->socket)))
		conn_error("Unable to close socket.");

	--numConnections;

#ifdef _MSC_VER
	if (numConnections == 0)
		WSACleanup();
#endif

    memset(connection, 0, sizeof(Connection));
}

bool conn_isup(Connection *connection) {
    return !memchk(connection, 0, sizeof(Connection));
}

bool conn_bind(Connection* connection)
{
	struct sockaddr_in6 addr;
	memset(&addr, 0, sizeof(addr));
	addr.sin6_family = AF_INET6;
	addr.sin6_addr = in6addr_any;
	addr.sin6_port = htons(SERVER_PORT);

	if (socket_error(bind(connection->socket, (struct sockaddr*)&addr, sizeof(addr))))
	{
		conn_error("Unable to bind socket.");
		conn_shutdown(connection);
		return false;
	}

	return true;
}

bool conn_multicast(Connection* connection)
{
	int loop = 1;
	if (socket_error(setsockopt(connection->socket, IPPROTO_IPV6, IPV6_MULTICAST_LOOP, (char*)&loop, sizeof(loop))))
	{
		conn_error("Failed to enable multicast looping.");
		conn_shutdown(connection);
		return false;
	}

	int ttl = MULTICAST_TTL;
	if (socket_error(setsockopt(connection->socket, IPPROTO_IPV6, IPV6_MULTICAST_HOPS, (char*)&ttl, sizeof(ttl))))
	{
		conn_error("Failed to set multicast TTL.");
		conn_shutdown(connection);
		return false;
	}

	struct in6_addr result;
	inet_pton(AF_INET6, MULTICAST_GROUP, &result);

	struct sockaddr_in6 addr;
	memset(&addr, 0, sizeof(addr));
	addr.sin6_family = AF_INET6;
	addr.sin6_addr = result;
	addr.sin6_port = htons(MULTICAST_PORT);

	struct ipv6_mreq group;
	memset(&group, 0, sizeof(group));
	group.ipv6mr_multiaddr = addr.sin6_addr;

	if (socket_error(setsockopt(connection->socket, IPPROTO_IPV6, IPV6_ADD_MEMBERSHIP, (char*)&group, sizeof(group))))
	{
		conn_error("Failed to add multicast membership.");
		conn_shutdown(connection);
		return false;
	}

	return true;
}

bool conn_recv(Connection* connection, char *buf, size_t* size, Address* adr)
{
	struct sockaddr_storage from;
	struct sockaddr_in6* from6;
	struct sockaddr_in* from4;
	socklen_t len = sizeof(from);

	memset(&from, 0, len);
	
	int read_bytes = recvfrom(connection->socket, buf, *size, 0, (struct sockaddr*)&from, &len);
#ifdef _MSC_VER
	if (WSAGetLastError() == WSAEWOULDBLOCK)
#endif
#ifdef __unix__
	if (socket_error(read_bytes) && errno == EAGAIN)
#endif
	{
		*size = 0;
		return true;
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
		return false;
	}

	*size = read_bytes;
	
	return true;
}

bool conn_send(Connection* connection, const char *buf, size_t size, Address* adr)
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

	int sent = sendto(connection->socket, buf, size, 0, addr, len);
	if (socket_error(sent))
	{
		conn_error("Sending failed");
		return false;
	}

	if (sent != size)
	{
		conn_error ("Message was sent only partially.");
		return false;
	}

	return true;
}
