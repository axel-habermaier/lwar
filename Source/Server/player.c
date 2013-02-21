#include <assert.h>
#include <math.h>
#include <stdarg.h>
#include <stddef.h>
#include <stdint.h>
#include <stdlib.h>

#include "server.h"
#include "vector.h"
#include "log.h"

void player_init(Player *p, size_t id) {
    Slot *s;

    p->id.n = id;
    p->ship.entity = 0;
    /* p->name = ""; */
    slots_foreach(p,s)
        s->entity = 0;
    player_input(p, 0,0,0,0,0,0,0,0,0,0,0,0);
    player_select(p, 0,0,0,0,0);
}

void player_clear(Player *p) {
    entity_remove(p->ship.entity);
}

/* too lazy to update signature each time something changes */
void player_input(Player *p,
                  int forwards,    int backwards,
                  int turn_left,   int turn_right,
                  int strafe_left, int strafe_right,
                  int fire1, int fire2, int fire3, int fire4,
                  int aim_x, int aim_y)
{
    Slot *s;

    p->a.x   = forwards    - backwards;
    p->a.y   = strafe_left - strafe_right;

    p->rot   = turn_right  - turn_left;

    p->aim.x = aim_x;
    p->aim.y = aim_y;

    s = p->weapons;
    if(s->entity) { s->entity->active = fire1; } s++;
    if(s->entity) { s->entity->active = fire2; } s++;
    if(s->entity) { s->entity->active = fire3; } s++;
    if(s->entity) { s->entity->active = fire4; } s++;
}

void player_select(Player *p,
                   int ship_type,
                   int weapon_type1, int weapon_type2,
                   int weapon_type3, int weapon_type4)
{
    Slot *s;

    s = &p->ship;
    s->selected_type = entity_type_get(ship_type);

    s = p->weapons;
    s->selected_type = entity_type_get(weapon_type1); s++;
    s->selected_type = entity_type_get(weapon_type2); s++;
    s->selected_type = entity_type_get(weapon_type3); s++;
    s->selected_type = entity_type_get(weapon_type4); s++;
}

static void slot_spawn(Player *p, Slot *s, Entity *parent, Vec x, Vec v) {
    assert(!s->entity);

    if(!s->selected_type) return;
    s->entity = entity_create(s->selected_type, p, x, v);
    
    if(!parent) return;
    entity_attach(parent, s->entity);
}

void player_spawn(Player *p, Vec x) {
    Slot *s;
    slot_spawn(p, &p->ship, 0, x, _0);
    Entity *ship = p->ship.entity;
    if(ship) {
        slots_foreach(p,s) {
            slot_spawn(p, s, ship, x, _0);
        }
    }
}

void player_notify_entity(Entity *e) {
    Player *p = e->player;
    assert(p);

    if(!e->dead) return;

/*
    if(p->ship == e)
        p->ship = 0;

    slots_foreach(i) {
        if(p->weapons[i] == e)
            p->weapons[i] = 0;
    }
*/
}

void player_die(Player *p) {
    entity_remove(p->ship.entity);
}

void player_rename(Player *p, Str name) {
    p->name = name;
}

static void player_action(Player *p) {
    Entity *ship = p->ship.entity;
    if(!ship) return;

    entity_accelerate(ship, p->a);
    entity_rotate(ship, p->rot);
}

void players_update() {
    Client *c;
    Player *p;
    clients_foreach(c) {
        p = &c->player;

        if(!p->ship.entity) {
            /* Vec x = { 100 + rand() % 600, 100 + rand() % 400 }; */
            Vec x = _0;
            player_spawn(p, x);
        }
        player_action(p);
    }
}

