#ifndef QUEUE_H
#define QUEUE_H

void queue_init();
void queue_cleanup();
void queue_shutdown();

void queue_timeout(Client *c);
void queue_gamestate_for(Client *cn);
void queue_join(Client *c);
void queue_leave(Client *c);
void queue_collision(Collision *c);
void queue_add(Entity *e);
void queue_remove(Entity *e);
void queue_kill(Player *k, Player *v);
void queue_stats();

void queue_forward(Message *m);
Message *queue_next(cr_t *state, Client *c, size_t *tries);

#endif
