#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "vector.h"
#include "log.h"

void player_init(Player *p, size_t id) {
    p->id.n        = id;
    p->ship        = 0;
    player_input(p, 0,0,0,0,0,0);
    player_select(p, 0,0);
}

void player_input(Player *p, int up, int down, int left, int right, int shooting, Pos phi) {
    p->up       = up;
    p->down     = down;
    p->left     = left;
    p->right    = right;
    p->shooting = shooting;
    p->phi      = phi;
}

void player_select(Player *p, size_t ship_type, size_t weapon_type) {
    p->ship_type   = entity_type_get(ship_type);
    p->weapon_type = entity_type_get(weapon_type);
}

void player_spawn(Player *p, Vec x) {
    assert(!p->ship);
    if(p->ship_type) {
        p->ship = entity_create(p->ship_type, p, x, _0);
        assert(p->ship);
    }
}

void player_notify(Entity *e) {
    if(e->player->ship == e)
        e->player->ship = 0;
}

void player_die(Player *p) {
    entity_remove(p->ship);
    /* p->ship = 0; is notified */
}

void player_rename(Player *p, Str name) {
    p->name = name;
}

static void player_action(Player *p) {
    Entity *e = p->ship;
    if(!e) return;

    /*
    Vec a = { p->up    - p->down,
              p->right - p->left };
              */

    Vec a = { p->up - p->down,
              0 };

    entity_accelerate(e, a);
    entity_rotate(e, p->right - p->left);

    /* bypass max_r, a player may look wherever he wants to */
    /*
    e->phi = p->phi;
    e->r   = 0;
    */

    /* TODO: should probably activate weapon, not ship */
    e->active = p->shooting;
}

void players_update() {
    Client *c;
    Player *p;
    clients_foreach(c) {
        p = &c->player;

        if(!p->ship) {
            Vec x = { 100 + rand() % 600, 100 + rand() % 400 };
            player_spawn(p, x);
        }
        player_action(p);
    }
}

