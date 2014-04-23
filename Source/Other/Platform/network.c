#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgNetworkError(pgString message);

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgIPAddress* pgCreateIPv6Address(pgByte address[16])
{

}

PG_API_EXPORT pgIPAddress* pgCreateIPv4Address(pgByte address[4])
{
}

PG_API_EXPORT pgVoid pgDestroyIPAddress(pgIPAddress* address)
{

}

//====================================================================================================================
// UDP socket functions
//====================================================================================================================

PG_API_EXPORT pgSocket* pgCreateUdpSocket(pgPacketReceivedCallback callback)
{
#ifdef WINDOWS
	u_long _true = 1;
#endif

	pgSocket* sock;
	PG_ASSERT_NOT_NULL(callback);

	PG_ALLOC(pgSocket, sock);

	sock->socket = socket(PF_INET6, SOCK_DGRAM, IPPROTO_UDP);
	if (socket_invalid(sock->socket))
	{
		pgNetworkError("Unable to initialize UDP socket.");
		return PG_FALSE;
	}

#ifdef WINDOWS
	if (socket_error(ioctlsocket(sock->socket, FIONBIO, &_true)))
#else
	if (socket_error(fcntl(sock->socket, F_SETFL, fcntl(sock->socket, F_GETFL, 0) | O_NDELAY)))
#endif
	{
		pgNetworkError("Unable to switch UDP socket to non-blocking mode.");
		return PG_FALSE;
	}

	int ipv6only = 0;
	if (setsockopt(sock->socket, IPPROTO_IPV6, IPV6_V6ONLY, (char*)&ipv6only, sizeof(ipv6only)) != 0)
	{
		pgNetworkError("Unable to switch UDP socket to dual-stack mode.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

PG_API_EXPORT pgBool pgDestroyUdpSocket(pgSocket* socket)
{
	PG_ASSERT_NOT_NULL(socket);

	if (socket_error(closesocket(socket->socket)))
	{
		pgNetworkError("Unable to close UDP socket.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

PG_API_EXPORT pgBool pgBindUdpSocket(pgSocket* socket, pgUint16 port)
{
	struct sockaddr_in6 addr;
	PG_ASSERT_NOT_NULL(socket);

	memset(&addr, 0, sizeof(addr));
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

PG_API_EXPORT pgBool pgUpdateUdpSocket(pgSocket* socket)
{
}

PG_API_EXPORT pgBool pgSendUdpData(pgSocket* socket, pgPacket* packet)
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
		conn_error("Message was sent only partially.");
		return false;
	}

	return true;
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgNetworkError(pgString message)
{
	pgString msg;

#ifdef WINDOWS
	msg = pgGetWin32ErrorMessage(GetLastError());
#else
	pgChar buffer[2048];
	strerror_r(errno, buffer, sizeof(buffer) / sizeof(pgChar));

	msg = buffer;
#endif

	PG_ERROR("%s. %s", message, msg);
}