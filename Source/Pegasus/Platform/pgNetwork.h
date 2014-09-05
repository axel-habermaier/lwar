#ifndef pgNetwork_h__
#define pgNetwork_h__

#include "pg.h"

//====================================================================================================================
// Network types
//====================================================================================================================

typedef struct pgSocket	pgSocket;
typedef	struct pgIPAddress
{
	pgByte ip[16];
} pgIPAddress;

typedef struct
{
	pgByte* data;
	pgUint32 size;
	pgUint32 capacity;
	pgIPAddress* address;
	pgUint16 port;
} pgPacket;

typedef enum
{
	PG_RECEIVE_ERROR = 0,
	PG_RECEIVE_DATA_AVAILABLE = 1,
	PG_RECEIVE_NO_DATA = 2
} pgReceiveStatus;

//====================================================================================================================
// Address functions
//====================================================================================================================

PG_API_EXPORT pgBool pgTryParseIPAddress(pgString address, pgIPAddress* ipAddress);
PG_API_EXPORT pgString pgIPAddressToString(pgIPAddress* address);

//====================================================================================================================
// UDP socket functions
//====================================================================================================================

PG_API_EXPORT pgSocket* pgCreateUdpSocket();
PG_API_EXPORT pgBool pgDestroyUdpSocket(pgSocket* socket);

PG_API_EXPORT pgBool pgBindUdpSocket(pgSocket* socket, pgUint16 port);
PG_API_EXPORT pgBool pgBindUdpSocketMulticast(pgSocket* socket, pgInt32 timeToLive, pgIPAddress* ipAddress, pgUint16 port);

PG_API_EXPORT pgReceiveStatus pgTryReceiveUdpPacket(pgSocket* socket, pgPacket* packet);
PG_API_EXPORT pgBool pgSendUdpPacket(pgSocket* socket, pgPacket* packet);

//====================================================================================================================
// Network functions
//====================================================================================================================

PG_API_EXPORT pgString pgGetLastNetworkError();

#endif