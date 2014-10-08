#ifndef STREAM_H
#define STREAM_H

#include "coroutine.h"
bool stream_recv(cr_t *state, Header *h, Message *m); /* TODO: needs to evaluate ack */
bool stream_send(cr_t *state, Header *h, Message *m); /* Note: keep h constant for a set of updates! */

bool stream_send_flush(Header *h, Message *m);
bool stream_send_discovery(Discovery *d);

#endif
