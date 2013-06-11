#include <assert.h>
#include <math.h>
#include <stdarg.h>
#include <stddef.h>
#include <stdint.h>
#include <stdlib.h>

#include "server.h"

#include "log.h"

void player_init(Player *p, size_t id) {
    Slot *s;
    SlotType *st;

    p->id.n = id;
    p->kills  = 0;
    p->deaths = 0;

    p->ship.entity = 0;
    /* p->name = ""; */
    slots_foreach(p,s,st)
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
    SlotType *st;

    p->a.x   = forwards    - backwards;
    p->a.y   = strafe_left - strafe_right;

    p->rot   = turn_right  - turn_left;

    p->aim.x = aim_x;
    p->aim.y = aim_y;

    int _f[NUM_SLOTS] = { fire1, fire2, fire3, fire4 };
    int *f = _f;

    slots_foreach(p,s,st) {
        if(s->entity) 
            s->entity->active = *f++;
    }
}

void player_select(Player *p,
                   int ship_type_id,
                   int weapon_type_id1, int weapon_type_id2,
                   int weapon_type_id3, int weapon_type_id4)
{
    EntityType *ship_type = entity_type_get(ship_type_id);
    if(!ship_type) return;

    Slot *s;
    SlotType *st;

    s = &p->ship;
    s->selected_type = ship_type;

    int _wt[NUM_SLOTS] = { weapon_type_id1, weapon_type_id2, weapon_type_id3, weapon_type_id4 };
    int *wt = _wt;

    slots_foreach(p,s,st) {
        s->selected_type = entity_type_get(*wt++);
    }
}

static void slot_spawn(Player *p, Slot *s, SlotType *st, Entity *parent, Vec x, Vec v) {
    assert(!s->entity);

    EntityType *t = s->selected_type;

    if(!t) return;
    if(parent) assert(st);

    // if(st && !set_contains(st->possible_types, t->id)) return;

    s->entity = entity_create(t, p, x, v);
    s->entity->slot = s;

    if(!parent) return;
    entity_attach(parent, s->entity, st->dx, st->dphi);
}

void player_spawn(Player *p, Vec x) {
    Slot *s;
    SlotType *st;
    slot_spawn(p, &p->ship, 0, 0, x, _0);
    Entity *ship = p->ship.entity;
    if(ship) {
        slots_foreach(p,s,st) {
            /* position and velocity will be overriden by physics,
             * since the entity will be attached to the ship */
            slot_spawn(p, s, st, ship, _0, _0);
        }
    }
}

void player_notify_entity(Entity *e) {
    if(!e->dead) return;
    if(!e->slot) return;

    e->slot->entity = 0;
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

