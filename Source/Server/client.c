#include <stdint.h>
#include <stddef.h>

#include "server.h"

static Client      _clients[MAX_CLIENTS];

static void player_init(Player *p, size_t id) {
    p->id.n        = id;
    p->ship        = 0;
    player_input(p, 0,0,0,0,0);
    player_select(p, 0,0);
}

void player_input(Player *p, int up, int down, int left, int right, int shooting) {
    p->up       = up;
    p->down     = down;
    p->left     = left;
    p->right    = right;
    p->shooting = shooting;
}

void player_select(Player *p, size_t ship_type, size_t weapon_type) {
    p->ship_type   = ship_type;
    p->weapon_type = weapon_type;
}

void player_rename(Player *p, Str name) {
    p->name = name;
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
    c->next_out_seqno  = 1; /* important to start with one */
    c->last_in_ack     = 0;
    c->last_in_seqno   = 0;
    c->last_in_frameno = 0;
    c->last_activity   = 0;
    c->misbehavior     = 0;
    c->needsync        = 1;
    c->hasleft         = 0;
    c->dead            = 0;

    player_init(&c->player, i);

}

static void client_dtor(size_t i, void *p) {
    Client *c = (Client*)p;
    entity_remove(c->player.ship);
    c->player.id.gen ++;
}

static int client_check_obsolete(size_t i, void *p) {
    return (server->client_mask & (1 << i)) == 0;
}

Client *client_create(Address *adr) {
    Client *c = slab_new(&server->clients, Client);
    if(c) {
        c->adr = *adr;
        server->client_mask |= (1 << client_id(c).n);
    }
    return c;
}

void client_remove(Client *c) {
    c->dead = 1;
    server->client_mask &= ~(1 << client_id(c).n);
}

Id client_id(Client *c) {
    return c->player.id;
}

Client *client_lookup(Address *adr) {
    if(!adr) return 0;
    Client *c;
    clients_foreach(c)
        if(address_eq(&c->adr, adr))
            return c;
    return 0;
}

Client *client_get(Id player) {
    Client *c = slab_at(&server->clients, Client, player.n);
    if(!c) return 0;
    return 0;
}

void clients_init() {
    slab_static(&server->clients, _clients, client_ctor, client_dtor);
    slab_remove(&server->clients, 0); /* reserve id 0 for server */
}

void clients_cleanup() {
    slab_free_pred(&server->clients, client_check_obsolete);
}
