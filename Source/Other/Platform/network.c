#include "prelude.h"
#include <stdio.h>

//====================================================================================================================
// Helper functions and error state
//====================================================================================================================

static pgBool hasError = PG_FALSE;
static pgChar lastNetworkError[2048];

static pgVoid pgNetworkError(pgString message);
static PG_NORETURN pgVoid pgNetworkDie(pgString message);

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgIPAddress* pgCreateIPAddress(pgString address)
{
	struct in6_addr ipv6 = { 0 };
	struct in_addr ipv4 = { 0 };
	pgBool isIPv6 = PG_FALSE;
	pgIPAddress* ipAddress = NULL;
	int result;

	PG_ASSERT_NOT_NULL(address);
	
	result = inet_pton(AF_INET6, address, &ipv6);
	if (result == -1)
		pgNetworkDie("Unable to parse IPv6 address.");

	if (result == 1)
		isIPv6 = PG_TRUE;
	else
	{
		result = inet_pton(AF_INET, address, &ipv4);
		if (result == -1)
			pgNetworkDie("Unable to parse IPv4 address.");

		if (result == 0)
			return NULL;
	}

	PG_ALLOC(pgIPAddress, ipAddress);
	ipAddress->isIPv6 = isIPv6;
	
	if (isIPv6)
		ipAddress->ipv6 = ipv6;
	else
		ipAddress->ipv4 = ipv4;

	memcpy(ipAddress->str, address, strlen(address));
	return ipAddress;
}

PG_API_EXPORT pgVoid pgDestroyIPAddress(pgIPAddress* address)
{
	PG_FREE(address);
}

PG_API_EXPORT pgString pgIPAddressToString(pgIPAddress* address)
{
	pgString result = NULL;

	PG_ASSERT_NOT_NULL(address);

	if (address->str[0] != '\0')
		return address->str;

	if (address->isIPv6)
		result = inet_ntop(AF_INET6, &address->ipv6, address->str, sizeof(address->str) / sizeof(address->str[0]));
	else
		result = inet_ntop(AF_INET, &address->ipv4, address->str, sizeof(address->str) / sizeof(address->str[0]));

	if (result == address->str)
		return address->str;

	pgNetworkDie("Unable to convert IP address to string.");
}

PG_API_EXPORT pgBool pgIpAddressesAreEqual(pgIPAddress* address1, pgIPAddress* address2)
{
	pgByte* ip1;
	pgByte* ip2;

	PG_ASSERT_NOT_NULL(address1);
	PG_ASSERT_NOT_NULL(address2);

	if (address1->isIPv6 && address2->isIPv6)
		return memcmp(&address1->ipv6, &address2->ipv6, sizeof(address1->ipv6)) == 0;

	if (!address1->isIPv6 && !address2->isIPv6)
		return memcmp(&address1->ipv4, &address2->ipv4, sizeof(address1->ipv4)) == 0;

	ip1 = (pgByte*)address1;
	ip2 = (pgByte*)address2;

	if (address1->isIPv6 && !address2->isIPv6)
		return memcmp(ip1 + 12, ip2, sizeof(address1->ipv4)) == 0;

	if (!address1->isIPv6 && address2->isIPv6)
		return memcmp(ip1, ip2 + 12, sizeof(address1->ipv4)) == 0;

	return PG_FALSE;
}

PG_API_EXPORT pgAddressFamily pgGetAddressFamily(pgIPAddress* address)
{
	PG_ASSERT_NOT_NULL(address);

	if (address->isIPv6)
		return PG_IPV6;
	else
		return PG_IPV4;
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

PG_API_EXPORT pgReceiveStatus pgTryReceiveUdpPacket(pgSocket* socket, pgPacket* packet)
{
	struct sockaddr_storage from = { 0 };
	struct sockaddr_in6* from6 = NULL;
	struct sockaddr_in* from4 = NULL;
	socklen_t len = sizeof(from);

	PG_ASSERT_NOT_NULL(socket);
	PG_ASSERT_NOT_NULL(packet);
	PG_ASSERT_NOT_NULL(packet->address);
	PG_ASSERT_NOT_NULL(packet->data);

	packet->size = recvfrom(socket->socket, (char*)packet->data, packet->capacity, 0, (struct sockaddr*)&from, &len);

#ifdef WINDOWS
	if (WSAGetLastError() == WSAEWOULDBLOCK)
#else
	if (socket_error(read_bytes) && errno == EAGAIN)
#endif
	{
		return PG_RECEIVE_NO_DATA;
	}

	if (socket_error(packet->size))
	{
		pgNetworkError("Receiving of UDP packet failed.");
		return PG_RECEIVE_ERROR;
	}

	switch (from.ss_family)
	{
	case AF_INET:
		from4 = (struct sockaddr_in*)&from;
		packet->port = from4->sin_port;
		packet->address->ipv4 = from4->sin_addr;
		packet->address->isIPv6 = PG_FALSE;
		break;
	case AF_INET6:
		from6 = (struct sockaddr_in6*)&from;
		packet->port = htons(from6->sin6_port);
		packet->address->ipv6 = from6->sin6_addr;
		packet->address->isIPv6 = PG_TRUE;
		break;
	default:
		PG_WARN("Received a UDP packet form a socket with an unsupported address family.");
		return PG_RECEIVE_ERROR;
	}

	packet->address->str[0] = '\0';
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
	
	if (packet->address->isIPv6)
		ipv6.sin6_addr = packet->address->ipv6;
	else
	{
		// Map IPv4 address to IPv6
		byte* addr = (byte*)&ipv6.sin6_addr;
		memcpy(addr + 12, &packet->address->ipv4, sizeof(packet->address->ipv4));
		addr[10] = 255;
		addr[11] = 255;
	}
	
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