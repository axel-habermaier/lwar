#pragma once

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Represents an endpoint consisting of an IP address and a port number.
//-------------------------------------------------------------------------------------------------------------------------------------------------------
struct IPEndPoint
{
public:
	IPEndPoint() = default;
	IPEndPoint(const IPAddress& address, uint16 port);

	const IPAddress& GetAddress() const;
	uint16 GetPort() const;

private:
	IPAddress _address;
	uint16 _port;
};
