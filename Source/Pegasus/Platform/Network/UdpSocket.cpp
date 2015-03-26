#include "Prelude.hpp"

#if defined(PG_SYSTEM_WINDOWS)

	#define IsSocketError(x) ((x) == SOCKET_ERROR)
	#define IsSocketInvalid() (_socket == INVALID_SOCKET)

#elif defined(PG_SYSTEM_LINUX)

	#define IsSocketError(x) ((x) < 0)
	#define IsSocketInvalid() (_socket < 0)
	#define closesocket close

#endif

PG_DECLARE_UDPINTERFACE_API(UdpSocket, UDP)

UdpSocket::UdpSocket(UdpInterface* udpInterface)
{
	PG_ASSERT_NOT_NULL(udpInterface);

	udpInterface->_this = this;
	PG_INITIALIZE_UDPINTERFACE(UdpSocket, UDP, udpInterface);
}

UdpSocket::~UdpSocket()
{
	if (IsSocketError(closesocket(_socket)))
		PG_ERROR("Unable to close UDP socket.");
}

bool UdpSocket::Initialize()
{
	try
	{
		_socket = socket(PF_INET6, SOCK_DGRAM, IPPROTO_UDP);
		if (IsSocketInvalid())
			throw NetworkException("Unable to initialize UDP socket.");

		PG_WINDOWS_ONLY(u_long trueParam = 1);
		PG_WINDOWS_ONLY(auto failed = IsSocketError(ioctlsocket(_socket, FIONBIO, &trueParam)));
		PG_LINUX_ONLY(auto failed = IsSocketError(fcntl(_socket, F_SETFL, fcntl(_socket, F_GETFL, 0) | O_NDELAY)));

		if (failed)
			throw NetworkException("Unable to switch UDP socket to non-blocking mode.");

		int ipv6only = 0;
		if (setsockopt(_socket, IPPROTO_IPV6, IPV6_V6ONLY, reinterpret_cast<const char*>(&ipv6only), sizeof(ipv6only)) != 0)
			throw NetworkException("Unable to switch UDP socket to dual-stack mode.");

		return true;
	}
	catch (const NetworkException& e)
	{
		_errorMessage = e.Message();
		return false;
	}
}

bool UdpSocket::Send(byte* buffer, int32 sizeInBytes, IPEndPoint* remoteEndPoint)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(remoteEndPoint);

	try
	{
		sockaddr_in6 ipv6 = { 0 } ;
		ipv6.sin6_family = AF_INET6;
		ipv6.sin6_port = htons(remoteEndPoint->GetPort());
		Memory::CopyArray(reinterpret_cast<byte*>(&ipv6.sin6_addr), remoteEndPoint->GetAddress().GetIPv6Address(), sizeof(ipv6.sin6_addr));

		auto sent = sendto(_socket, reinterpret_cast<const char*>(buffer), sizeInBytes, 0, reinterpret_cast<sockaddr*>(&ipv6), sizeof(sockaddr_in6));
		if (IsSocketError(sent))
			throw NetworkException("Failed to send UDP packet.");

		if (sent != sizeInBytes)
			throw NetworkException("UDP packet was sent only partially.");

		return true;
	}
	catch (const NetworkException& e)
	{
		_errorMessage = e.Message();
		return false;
	}
}

ReceiveStatus UdpSocket::TryReceive(byte* buffer, int32 capacityInBytes, IPEndPoint* remoteEndPoint, int32* receivedBytes)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(remoteEndPoint);
	PG_ASSERT_NOT_NULL(receivedBytes);

	try
	{
		sockaddr_storage from;
		socklen_t len = sizeof(from);
		auto size = recvfrom(_socket, reinterpret_cast<char*>(buffer), capacityInBytes, 0, reinterpret_cast<sockaddr*>(&from), &len);
		*receivedBytes = size;

		PG_WINDOWS_ONLY(auto noPendingPacket = WSAGetLastError() == WSAEWOULDBLOCK);
		PG_LINUX_ONLY(auto noPendingPacket = IsSocketError(size) && errno == EAGAIN);

		if (noPendingPacket)
			return ReceiveStatus::NoPacketAvailable;

		if (IsSocketError(size))
			throw NetworkException("Receiving of UDP packet failed.");

		switch (from.ss_family)
		{
		case AF_INET:
		{
			auto in4 = reinterpret_cast<sockaddr_in*>(&from);
			new (remoteEndPoint)IPEndPoint(IPAddress::FromIPv4(reinterpret_cast<byte*>(&in4->sin_addr)), htons(in4->sin_port));
			break;
		}
		case AF_INET6:
		{
			auto in6 = reinterpret_cast<sockaddr_in6*>(&from);
			new (remoteEndPoint)IPEndPoint(IPAddress::FromIPv6(reinterpret_cast<byte*>(&in6->sin6_addr)), htons(in6->sin6_port));
			break;
		}
		default:
			PG_ERROR("Received a UDP packet with an unsupported address family.");
			return ReceiveStatus::NoPacketAvailable;
		}

		return ReceiveStatus::PacketReceived;
	}
	catch (const NetworkException& e)
	{
		_errorMessage = e.Message();
		return ReceiveStatus::Error;
	}
}

bool UdpSocket::Bind(uint16 port)
{
	sockaddr_in6 addr = { 0 };
	addr.sin6_family = AF_INET6;
	addr.sin6_addr = in6addr_any;
	addr.sin6_port = htons(port);

	if (!IsSocketError(bind(_socket, reinterpret_cast<sockaddr*>(&addr), sizeof(addr))))
		return true;

	_errorMessage = "Failed to bind UDP socket.";
	return false;
}

bool UdpSocket::BindMulticast(IPEndPoint* endPoint, int32 timeToLive)
{
	try
	{
		int32 loop = 1;
		if (IsSocketError(setsockopt(_socket, IPPROTO_IPV6, IPV6_MULTICAST_LOOP, reinterpret_cast<char*>(&loop), sizeof(loop))))
			throw NetworkException("Failed to enable multicast looping.");

		if (IsSocketError(setsockopt(_socket, IPPROTO_IPV6, IPV6_MULTICAST_HOPS, reinterpret_cast<char*>(&timeToLive), sizeof(timeToLive))))
			throw NetworkException("Failed to set multicast TTL.");

		sockaddr_in6 addr = { 0 };
		addr.sin6_family = AF_INET6;
		addr.sin6_port = htons(endPoint->GetPort());
		Memory::Copy(&addr.sin6_addr, &endPoint->GetAddress(), sizeof(addr.sin6_addr));

		ipv6_mreq group = { 0 };
		group.ipv6mr_multiaddr = addr.sin6_addr;
		if (IsSocketError(setsockopt(_socket, IPPROTO_IPV6, IPV6_ADD_MEMBERSHIP, reinterpret_cast<char*>(&group), sizeof(group))))
			throw NetworkException("Failed to add multicast membership.");

		addr.sin6_addr = in6addr_any;
		if (IsSocketError(bind(_socket, reinterpret_cast<sockaddr*>(&addr), sizeof(addr))))
			throw NetworkException("Failed to bind multicast UDP socket.");

		return true;
	}
	catch (const NetworkException& e)
	{
		_errorMessage = e.Message();
		return false;
	}
}

void* UdpSocket::GetErrorMessage()
{
	return const_cast<char*>(_errorMessage.c_str());
}