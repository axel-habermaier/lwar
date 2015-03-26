#include "Prelude.hpp"

PG_API_EXPORT UdpInterface* CreateUdpInterface()
{
	auto udpInterface = PG_NEW(UdpInterface);
	PG_NEW(UdpSocket, udpInterface);

	return udpInterface;
}

PG_API_EXPORT void FreeUdpInterface(UdpInterface* udpInterface)
{
	if (udpInterface == nullptr)
		return;

	PG_DELETE(static_cast<UdpSocket*>(udpInterface->_this));
	PG_DELETE(udpInterface);
}