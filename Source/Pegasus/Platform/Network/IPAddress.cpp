#include "Prelude.hpp"

IPAddress::IPAddress()
{
	static_assert(sizeof(in6_addr) == sizeof(_address), "Unexpected size.");
	Memory::Set(&_address, 16, 0);
}

IPAddress IPAddress::FromIPv6(byte* address)
{
	IPAddress ip;
	Memory::CopyArray(ip._address, address, 16);
	return ip;
}

IPAddress IPAddress::FromIPv4(byte* address)
{
	IPAddress ip;
	Memory::CopyArray(ip._address + 12, address, 4);
	ip._address[10] = 255;
	ip._address[11] = 255;
	return ip;
}

const byte* IPAddress::GetIPv6Address() const
{
	return _address;
}