#ifndef CONNECTION_H
#define CONNECTION_H

#include "address.h"

typedef struct Connection Connection;

bool conn_init(Connection* connection);
void conn_shutdown(Connection* connection);

bool conn_multicast(Connection* connection);
bool conn_bind(Connection* connection);

bool conn_recv(Connection* connection, char *buf, size_t* size, Address* adr);
bool conn_send(Connection* connection, const char *buf, size_t size, Address* adr);

#endif
