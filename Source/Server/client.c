#include <stdint.h>
#include <stddef.h>

#include "server.h"

static Client      _clients[MAX_CLIENTS];

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
    clients_foreach(c) {
        player_action(&c->player);
    }
}

static void client_ctor(size_t i, void *p) {
    Client *c = (Client*)p;
    c->player.id.n = i;
    c->next_out_seqno = 1; /* important to start with one */
    c->last_in_ack    = 0;
    c->last_in_seqno  = 0;
    player_init(&c->player);
    /* mqueue_init(&c->queue); */
}

static void client_dtor(size_t i, void *p) {
    Client *c = (Client*)p;
    c->player.id.gen ++;
}

Client *client_create() {
    return slab_new(&server->clients, Client);
}

void client_remove(Client *c) {
    slab_free(&server->clients, c);
}

Client *client_get(Id player, Address *a) {
    Client *c = slab_at(&server->clients, Client, player.n);
    if(!c) return 0;
    if(!a) return c;
    if(   c->adr.ip == a->ip
       && c->adr.port == a->port) return c;
    return 0;
}

void clients_init() {
    slab_static(&server->clients, _clients, client_ctor, client_dtor);
}
