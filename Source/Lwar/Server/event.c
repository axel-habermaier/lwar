#include "types.h"

#include "event.h"

void event_kill(Player *k, Player *v) {
    protocol_notify_kill(k, v);
}

void event_entity(Entity *e) {
    player_notify_entity(e);
    protocol_notify_entity(e);
}

void event_collision(Collision *c) {
    entities_notify_collision(c);
    protocol_notify_collision(c);
}

