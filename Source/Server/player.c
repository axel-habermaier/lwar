#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>

#include "server.h"
#include "vector.h"
#include "log.h"

void player_init(Player *p, size_t id) {
    p->id.n   = id;
    p->ship   = 0;
    p->weapon = 0;
    player_input(p, 0,0,0,0,0,0,0);
    player_select(p, 0,0);
    INIT_LIST_HEAD(&p->inventory);
}

void player_clear(Player *p) {
    Item *j;
    entity_remove(p->ship);
    inventory_foreach(p, j)
        item_remove(j);
    /* assert(list_empty(&p->inventory)); */
}

void player_input(Player *p, int up, int down, int left, int right, int fire1, int fire2, Pos phi) {
    p->up    = up;
    p->down  = down;
    p->left  = left;
    p->right = right;
    p->fire1 = fire1;
    p->fire2 = fire2;
    p->phi   = phi;
}

void player_select(Player *p, size_t ship_type, size_t weapon_type) {
    p->ship_type   = entity_type_get(ship_type);
    p->weapon_type = item_type_get(weapon_type);
}

void player_pickup(Player *p, Item *j) {
    list_move_tail(&j->h, &p->inventory);
}

void player_spawn(Player *p, Vec x) {
    assert(!p->ship);
    assert(!p->weapon);

    if(p->ship_type) {
        p->ship = entity_create(p->ship_type, p, x, _0);

        if(p->weapon_type && !p->weapon) {
            p->weapon = item_create(p->weapon_type);
            entity_attach(p->ship, p->weapon);
        }
    }
}

void player_notify_state(Entity *e) {
    Player *p = e->player;
    assert(p);
    if(p->ship == e && e->dead) {
        p->ship   = 0;
        p->weapon = 0;
    }
}

void player_die(Player *p) {
    entity_remove(p->ship);
}

void player_rename(Player *p, Str name) {
    p->name = name;
}

static void player_action(Player *p) {
    Entity *e = p->ship;
    if(!e) return;

    Item   *j = p->weapon;
    if(!j) return;

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

    j->fire_mask = 0;
    if(p->fire1) j->fire_mask |= 0x1;
    if(p->fire2) j->fire_mask |= 0x2;

/*
    if(   j->type->fire
       && j->fire_mask
       && clock_periodic(&j->fire_periodic, j->type->fire_interval))
    {
        j->type->fire(e, j);
    }
*/

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

