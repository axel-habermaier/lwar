#ifndef PROTOCOL_H
#define PROTOCOL_H

void protocol_init();
void protocol_recv();
void protocol_send(bool force);
void protocol_notify_entity(Entity *e);
void protocol_notify_collision(Collision *c);
void protocol_notify_kill(Player *k, Player *v);
void protocol_cleanup();

#endif
