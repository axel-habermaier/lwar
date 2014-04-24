#ifndef pgNetwork_h__
#define pgNetwork_h__

#include "pg.h"

//====================================================================================================================
// Network types
//====================================================================================================================

typedef struct pgIPAddress  pgIPAddress;
typedef struct pgSocket		pgSocket;

typedef struct pgPacket
{
	pgByte* data;
	pgUint32 size;
	pgUint32 capacity;
	pgIPAddress* address;
	pgUint16 port;
} pgPacket;

typedef enum pgReceiveStatus
{
	PG_RECEIVE_ERROR = 0,
	PG_RECEIVE_DATA_AVAILABLE = 1,
	PG_RECEIVE_NO_DATA = 2
} pgReceiveStatus;

typedef enum pgAddressFamily
{
	PG_IPV6 = 1,
	PG_IPV4 = 2
} pgAddressFamily;

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgIPAddress* pgCreateIPAddress(pgString address);
PG_API_EXPORT pgVoid pgDestroyIPAddress(pgIPAddress* address);

PG_API_EXPORT pgString pgIPAddressToString(pgIPAddress* address);
PG_API_EXPORT pgBool pgIpAddressesAreEqual(pgIPAddress* address1, pgIPAddress* address2);
PG_API_EXPORT pgAddressFamily pgGetAddressFamily(pgIPAddress* address);

//====================================================================================================================
// UDP socket functions
//====================================================================================================================

PG_API_EXPORT pgSocket* pgCreateUdpSocket();
PG_API_EXPORT pgBool pgDestroyUdpSocket(pgSocket* socket);

PG_API_EXPORT pgBool pgBindUdpSocket(pgSocket* socket, pgUint16 port);

PG_API_EXPORT pgReceiveStatus pgTryReceiveUdpPacket(pgSocket* socket, pgPacket* packet);
PG_API_EXPORT pgBool pgSendUdpPacket(pgSocket* socket, pgPacket* packet);

//====================================================================================================================
// Network functions
//====================================================================================================================

PG_API_EXPORT pgString pgGetLastNetworkError();

#endif