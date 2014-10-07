#ifndef STREAM_H
#define STREAM_H

#include "coroutine.h"
bool stream_recv(cr_t *state, Header *h, Message *m); /* TODO: needs to evaluate ack */
void stream_send(cr_t *state, Header *h, Message *m); /* Note: keep h constant for a set of updates! */

#endif
