#pragma once

struct IPEndPoint;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Represents a UDP-based socket that can be used to unreliably send and receive packets over the network.
//-------------------------------------------------------------------------------------------------------------------------------------------------------
class UdpSocket
{
public:
	UdpSocket(UdpInterface* udpInterface);
	~UdpSocket();

	PG_DECLARE_UDPINTERFACE_METHODS

private:
	PG_WINDOWS_ONLY(SOCKET _socket = INVALID_SOCKET);
	PG_LINUX_ONLY(int _socket = 0);

	std::string _errorMessage;
};
