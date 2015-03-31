#pragma once

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Represents an IPv4 or IPv6 internet protocol address.
//-------------------------------------------------------------------------------------------------------------------------------------------------------
struct IPAddress
{
public:
	IPAddress();

	static IPAddress FromIPv6(byte* address);
	static IPAddress FromIPv4(byte* address);

	const byte* GetIPv6Address() const;

private:
	byte _address[16];
	bool _isIPv4 = false;
};
