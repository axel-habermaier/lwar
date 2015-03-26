#include "Prelude.hpp"

IPEndPoint::IPEndPoint(const IPAddress& address, uint16 port)
	: _address(address)
	, _port(port)
{
}

const IPAddress& IPEndPoint::GetAddress() const
{
	return _address;
}

uint16 IPEndPoint::GetPort() const
{
	return _port;
}