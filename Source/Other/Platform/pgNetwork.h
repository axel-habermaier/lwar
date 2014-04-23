#ifndef pgNetwork_h__
#define pgNetwork_h__

#include "pg.h"

//====================================================================================================================
// Network types
//====================================================================================================================

typedef struct pgPacket
{
	pgByte* data;
	pgUint32 sizeInBytes;
	pgUint32 capacityInBytes;
	pgIPAddress* address;
} pgPacket;

typedef struct pgIPAddress  pgIPAddress;
typedef struct pgSocket		pgSocket;

typedef pgVoid(*pgPacketReceivedCallback)(pgPacket* packet);

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgIPAddress* pgCreateIPv6Address(pgByte address[16]);
PG_API_EXPORT pgIPAddress* pgCreateIPv4Address(pgByte address[4]);
PG_API_EXPORT pgVoid pgDestroyIPAddress(pgIPAddress* address);

//====================================================================================================================
// Packet functions
//====================================================================================================================

PG_API_EXPORT pgPacket* pgCreatePacket(pgUint32 capacityInBytes);
PG_API_EXPORT pgVoid pgDestroyPacket(pgPacket* packet);

//====================================================================================================================
// UDP socket functions
//====================================================================================================================

PG_API_EXPORT pgSocket* pgCreateUdpSocket(pgPacketReceivedCallback callback);
PG_API_EXPORT pgBool pgDestroyUdpSocket(pgSocket* socket);

PG_API_EXPORT pgBool pgBindUdpSocket(pgSocket* socket, pgUint16 port);
PG_API_EXPORT pgBool pgUpdateUdpSocket(pgSocket* socket);

PG_API_EXPORT pgBool pgSendUdpData(pgSocket* socket, pgPacket* packet);

#endif