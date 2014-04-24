#include "prelude.h"
#include <stdio.h>

//====================================================================================================================
// Helper functions and error state
//====================================================================================================================

static pgBool hasError = PG_FALSE;
static pgChar lastNetworkError[2048];

static pgVoid pgNetworkError(pgString message);
static PG_NORETURN pgVoid pgNetworkDie(pgString message);

static pgVoid pgMapIPAddress(struct in_addr ipv4, pgIPAddress* ipv6);

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgBool pgTryParseIPAddress(pgString address, pgIPAddress* ipAddress)
{
	struct in_addr ipv4 = { 0 };
	int result;

	PG_ASSERT_NOT_NULL(address);
	PG_ASSERT_NOT_NULL(ipAddress);

	result = inet_pton(AF_INET6, address, ipAddress);
	if (result == -1)
		pgNetworkDie("Unable to parse IPv6 address.");

	if (result == 1)
		return PG_TRUE;

	result = inet_pton(AF_INET, address, &ipv4);
	if (result == -1)
		pgNetworkDie("Unable to parse IPv4 address.");

	if (result == 0)
		return PG_FALSE;

	pgMapIPAddress(ipv4, ipAddress);
	return PG_TRUE;
}

PG_API_EXPORT pgString pgIPAddressToString(pgIPAddress* address)
{
	static pgChar str[INET6_ADDRSTRLEN];
	pgString result;

	PG_ASSERT_NOT_NULL(address);

	result = inet_ntop(AF_INET6, address, str, sizeof(str) / sizeof(pgChar));

	if (result != str)
		pgNetworkDie("Unable to convert IP address to string.");

	return str;
}

//====================================================================================================================
// UDP socket functions
//====================================================================================================================

PG_API_EXPORT pgSocket* pgCreateUdpSocket()
{
#ifdef WINDOWS
	u_long _true = 1;
#endif

	pgSocket* sock = NULL;
	PG_ALLOC(pgSocket, sock);

	sock->socket = socket(PF_INET6, SOCK_DGRAM, IPPROTO_UDP);
	if (socket_invalid(sock->socket))
	{
		pgNetworkError("Unable to initialize UDP socket.");
		return NULL;
	}

#ifdef WINDOWS
	if (socket_error(ioctlsocket(sock->socket, FIONBIO, &_true)))
#else
	if (socket_error(fcntl(sock->socket, F_SETFL, fcntl(sock->socket, F_GETFL, 0) | O_NDELAY)))
#endif
	{
		pgNetworkError("Unable to switch UDP socket to non-blocking mode.");
		return NULL;
	}

	int ipv6only = 0;
	if (setsockopt(sock->socket, IPPROTO_IPV6, IPV6_V6ONLY, (char*)&ipv6only, sizeof(ipv6only)) != 0)
	{
		pgNetworkError("Unable to switch UDP socket to dual-stack mode.");
		return NULL;
	}

	return sock;
}

PG_API_EXPORT pgBool pgDestroyUdpSocket(pgSocket* socket)
{
	if (socket == NULL)
		return PG_TRUE;

	if (socket_error(closesocket(socket->socket)))
	{
		pgNetworkError("Unable to close UDP socket.");
		return PG_FALSE;
	}

	PG_FREE(socket);
	return PG_TRUE;
}

PG_API_EXPORT pgBool pgBindUdpSocket(pgSocket* socket, pgUint16 port)
{
	struct sockaddr_in6 addr = { 0 };
	PG_ASSERT_NOT_NULL(socket);

	addr.sin6_family = AF_INET6;
	addr.sin6_addr = in6addr_any;
	addr.sin6_port = htons(port);

	if (socket_error(bind(socket->socket, (struct sockaddr*)&addr, sizeof(addr))))
	{
		pgNetworkError("Failed to bind UDP socket.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

PG_API_EXPORT pgBool pgBindUdpSocketMulticast(pgSocket* socket, pgInt32 timeToLive, pgIPAddress* ipAddress, pgUint16 port)
{
	int loop = 1;
	struct sockaddr_in6 addr = { 0 };
	struct ipv6_mreq group = { 0 };

	PG_ASSERT_NOT_NULL(socket);
	PG_ASSERT_NOT_NULL(ipAddress);

	if (socket_error(setsockopt(socket->socket, IPPROTO_IPV6, IPV6_MULTICAST_LOOP, (char*)&loop, sizeof(loop))))
	{
		pgNetworkError("Failed to enable multicast looping.");
		return PG_FALSE;
	}

	if (socket_error(setsockopt(socket->socket, IPPROTO_IPV6, IPV6_MULTICAST_HOPS, (char*)&timeToLive, sizeof(timeToLive))))
	{
		pgNetworkError("Failed to set multicast TTL.");
		return PG_FALSE;
	}

	addr.sin6_family = AF_INET6;
	addr.sin6_port = htons(port);
	memcpy(&addr.sin6_addr, ipAddress, sizeof(pgIPAddress));

	group.ipv6mr_multiaddr = addr.sin6_addr;
	if (socket_error(setsockopt(socket->socket, IPPROTO_IPV6, IPV6_ADD_MEMBERSHIP, (char*)&group, sizeof(group))))
	{
		pgNetworkError("Failed to add multicast membership.");
		return PG_FALSE;
	}

	addr.sin6_addr = in6addr_any;
	if (socket_error(bind(socket->socket, (struct sockaddr*)&addr, sizeof(addr))))
	{
		pgNetworkError("Failed to bind multicast UDP socket.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

PG_API_EXPORT pgReceiveStatus pgTryReceiveUdpPacket(pgSocket* socket, pgPacket* packet)
{
	struct sockaddr_storage from = { 0 };
	struct sockaddr_in6* from6 = NULL;
	struct sockaddr_in* from4 = NULL;
	socklen_t len = sizeof(from);
	int size;

	PG_ASSERT_NOT_NULL(socket);
	PG_ASSERT_NOT_NULL(packet);
	PG_ASSERT_NOT_NULL(packet->address);
	PG_ASSERT_NOT_NULL(packet->data);

	size = recvfrom(socket->socket, (char*)packet->data, packet->capacity, 0, (struct sockaddr*)&from, &len);
	packet->size = (pgUint32)size;

#ifdef WINDOWS
	if (WSAGetLastError() == WSAEWOULDBLOCK)
#else
	if (socket_error(size) && errno == EAGAIN)
#endif
	{
		return PG_RECEIVE_NO_DATA;
	}

	if (socket_error(size))
	{
		pgNetworkError("Receiving of UDP packet failed.");
		return PG_RECEIVE_ERROR;
	}

	switch (from.ss_family)
	{
	case AF_INET:
		from4 = (struct sockaddr_in*)&from;
		packet->port = htons(from4->sin_port);
		pgMapIPAddress(from4->sin_addr, packet->address);
		break;
	case AF_INET6:
		from6 = (struct sockaddr_in6*)&from;
		packet->port = htons(from6->sin6_port);
		memcpy(packet->address, &from6->sin6_addr, sizeof(from6->sin6_addr));
		break;
	default:
		PG_ERROR("Received a UDP packet from a socket with an unsupported address family.");
		return PG_RECEIVE_ERROR;
	}

	return PG_RECEIVE_DATA_AVAILABLE;
}

PG_API_EXPORT pgBool pgSendUdpPacket(pgSocket* socket, pgPacket* packet)
{
	struct sockaddr_in6 ipv6 = { 0 };
	int sent = 0;

	PG_ASSERT_NOT_NULL(socket);
	PG_ASSERT_NOT_NULL(packet);
	PG_ASSERT_NOT_NULL(packet->address);
	PG_ASSERT_NOT_NULL(packet->data);

	ipv6.sin6_family = AF_INET6;
	ipv6.sin6_port = htons(packet->port);
	memcpy(&ipv6.sin6_addr, packet->address, 16);
	
	sent = sendto(socket->socket, (const char*)packet->data, packet->size, 0, (struct sockaddr*)&ipv6, sizeof(struct sockaddr_in6));
	if (socket_error(sent))
	{
		pgNetworkError("Failed to send UDP packet.");
		return PG_FALSE;
	}

	if ((pgUint32)sent != packet->size)
	{
		pgNetworkError("UDP packet was sent only partially.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

//====================================================================================================================
// Network functions
//====================================================================================================================

PG_API_EXPORT pgString pgGetLastNetworkError()
{
	if (!hasError)
		return NULL;

	hasError = PG_FALSE;
	return lastNetworkError;
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgNetworkError(pgString message)
{
	int length, i;
	pgString osMessage;

#ifdef WINDOWS
	osMessage = pgGetWin32ErrorMessage(GetLastError());
#else
	pgChar buffer[2048];
	strerror_r(errno, buffer, sizeof(buffer) / sizeof(pgChar));

	osMessage = buffer;
#endif

	length = sprintf(lastNetworkError, "%s %s", message, osMessage);
	hasError = PG_TRUE;

	for (i = length; i > 0; --i)
	{
		if (lastNetworkError[i - 1] == '\r' || lastNetworkError[i - 1] == '\n')
			lastNetworkError[i - 1] = '\0';
		else
			break;
	}
}

static PG_NORETURN pgVoid pgNetworkDie(pgString message)
{
	pgNetworkError(message);
	PG_DIE("%s", lastNetworkError);
}

static pgVoid pgMapIPAddress(struct in_addr ipv4, pgIPAddress* ipv6)
{
	memset(ipv6, 0, sizeof(pgIPAddress));
	memset(&ipv6->ip[0] + 10, 255, 2);
	memcpy(&ipv6->ip[0] + 12, &ipv4, sizeof(ipv4));
}