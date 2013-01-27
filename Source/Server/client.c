#include <stdint.h>
#include <stddef.h>

#include "server.h"

static void player_init(Player *p) {
    p->ship = 0;
    p->shooting = 0;
}

void player_input(Player *p, int up, int down, int left, int right, int shooting) {
    p->up       = up;
    p->down     = down;
    p->left     = left;
    p->right    = right;
    p->shooting = shooting;
}

static void player_action(Player *p) {
    Entity *e = p->ship;
    Vec a = entity_acc(e);
    Vec b = { a.x * p->right - p->left,
              a.y * p->up    - p->down };
    physics_acc_rel(e, b);
    e->active = p->shooting;
}

void player_actions() {
    Client *c;
    for_each_connected_client(server, c) {
        player_action(&c->player);
    }
}

static void client_init(Client *c) {
    player_init(&c->player);
    c->next_seqno = 0;
    c->last_ack   = 0;
    mqueue_init(&c->queue);
}

Client *client_create() {
    Client *c;
    for_each_client(server, c) {
        if(!c->connected) {
            c->connected = 1;
            client_init(c);
            return c;
        }
    }
    return 0;
}

void client_remove(Client *c) {
    if(c) {
        c->player.id.gen ++;
        mqueue_destroy(&c->queue);
        c->connected = 0;
    }
}

Client *client_get(Id player, Address *a) {
    if(player.n < MAX_CLIENTS) {
        Client *c = &server->clients[player.n];
        if(c->connected)
            if(!a || (c->adr.ip == a->ip && c->adr.port == a->port))
                return c;
    }
    return 0;
}

void clients_init() {
    size_t i;
    for(i=0; i<MAX_CLIENTS;  i++) {
        Client *c = &server->clients[i];
        c->player.id.n = i;
    }
}
