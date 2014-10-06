#ifndef QUEUE_H
#define QUEUE_H

void queue_init();
void queue_cleanup();
void queue_shutdown();

void queue_broadcast(Message *m);
void queue_unicast(Client *c, Message *m);

#include "coroutine.h"
Message *queue_next(cr_t *state, Client *c, size_t *tries);

#endif
