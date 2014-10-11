#ifndef ENTITY_H
#define ENTITY_H

#include "bitset.h"
#include "clock.h"
#include "config.h"
#include "id.h"
#include "list.h"
#include "str.h"
#include "update.h"
#include "vector.h"

struct Slot {
    Entity *entity;
    EntityType *selected_type;
};

struct SlotType {
    Vec  dx;
    Real dphi;
    BitSet possible_types;
};

#define slots_foreach(p,s,st)    for(s = p->weapons, st = (p->ship.entity ? p->ship.entity->type->slots : 0); s<p->weapons+NUM_SLOTS; s++, st = (p->ship.entity ? st + 1 : 0))

/* Entities have a physical appearance in the world */
struct Entity {
    List _l;
    List _u;       /* the format list */

    EntityType *type;

    Id id;
    bool dead;
    Clock age;

    /* gameplay */
    Player *player;

    List children;  /* structured as a tree */
    List siblings;  /* with sibling lists   */
    Entity *parent; /* and parent pointers  */

    Slot *slot;     /* if directly controlled by a player */

	Entity *target;

    bool active;
    Clock interval;
    Clock periodic;

    /* physics */
    Vec x,v,a;    /* world position, absolute velocity, acceleration */
    Real phi,rot; /* orientation angle, rotation (= delta phi) */

    Vec  dx;      /* position and angle relative to parent */
    Real dphi;

    Real energy;  /* ammunition, fuel, damage ... */
    Real health;
    Real shield;  /* damage multiplier */
    Real len;
    Real mass;
    Real radius;
    Time remaining;

    bool collides;
    bool bounces;
};

struct EntityType {
    size_t id;

    /* gameplay */
    void (*act)(Entity *self);
    void (*collide)(Entity *self, Entity *other, Real impact);

    Clock init_interval; /* for activation */

    /* physics */
    Real init_energy;
    Real init_health;
    Real init_shield;
    Real init_len;
    Real init_mass;
    Real init_radius;
    Vec  max_a;      /* acceleration */
    Vec  max_b;      /* brake        */
    Real max_rot;    /* rotation     */

    Format *format;

    const char *name;
    SlotType slots[NUM_SLOTS]; /* possible weapon attachments */
};

void    entities_init();
void    entities_cleanup();
void    entities_update();
void    entities_shutdown();

Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v);
void    entity_remove(Entity *e);
void    entities_remove_for(Player *p);
void    entities_notify_collision(Collision *c);

void entity_push(Entity *e, Vec a);
void entity_accelerate(Entity *e, Vec a);
void entity_accelerate_to(Entity *e, Vec v);
void entity_rotate(Entity *e, Real r);
void entity_hit(Entity *e, Real damage, Player *p);
void entity_attach(Entity *e, Entity *c, Vec dx, Real dphi);

EntityType *entity_type_get(size_t id);
void entity_type_register(const char *name, EntityType *t, Format *f);

#endif
