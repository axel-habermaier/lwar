#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"
#include "message.h"
#include "vector.h"

#include "rules.h"

#define SELF_NAME "server"
static Str self_name = { sizeof(SELF_NAME)-1, SELF_NAME };
static const Real gravity_factor = 10000; // 0.04;

EntityType *_types[MAX_ENTITY_TYPES];

void rules_init() {
    array_static(&server->types, _types);

    server->self = client_create_local();
    player_rename(&server->self->player, self_name);

    format_register(&format_ship);
    format_register(&format_pos);
    format_register(&format_ray);
    format_register(&format_circle);

    /* register some entity types */
    entity_type_register(ENTITY_TYPE_SHIP,      &type_ship,      &format_ship);
    entity_type_register(ENTITY_TYPE_BULLET,    &type_bullet,    &format_ship); /* TODO: pos */
    entity_type_register(ENTITY_TYPE_PLANET,    &type_planet,    &format_ship); /* TODO: pos */
    entity_type_register(ENTITY_TYPE_ROCKET,    &type_rocket,    &format_ship);
    entity_type_register(ENTITY_TYPE_RAY,       &type_ray,       &format_ray);
    // entity_type_register(ENTITY_TYPE_SHOCKWAVE, &type_shockwave, &format_circle);

    entity_type_register(ENTITY_TYPE_GUN,       &type_gun,       0); /* not shared with server */
    entity_type_register(ENTITY_TYPE_PHASER,    &type_phaser,    0);

    Vec x = { 500,500 };
    Entity *p0 = entity_create(&type_planet, &server->self->player, x, _0);
}

EntityType *entity_type_get(size_t id) {
    if(id < MAX_ENTITY_TYPES)
        return array_at(&server->types, EntityType*, id);
    else return 0;
}

/* entity type 0 is invalid, should map to NULL */
void entity_type_register(size_t id, EntityType *t, Format *f) {
    if(0 < id && id < MAX_ENTITY_TYPES) {
        array_at(&server->types, EntityType*, id) = t;
        t->format = f;
    }
}

void decay(Entity *e) {
	if(e->age > 5000)
        e->health = 0;
}

void gun_shoot(Entity *gun) {
	if(gun->energy <= 0)
        return;

    gun->energy --;

    Vec f = unit(gun->phi);
        Vec x = add(gun->x, scale(f, gun->radius + type_bullet.init_radius*2));
        Vec dir = normalize(sub((gun->player->aim), x));
        Vec v = scale(dir, type_bullet.max_a.y); /* initial speed */
    entity_create(&type_bullet,gun->player,x,v);
}

void phaser_shoot(Entity *phaser) {
	/*
    if(phaser->energy <= 0)
        return;

    phaser->energy --;
    */

    if(!list_empty(&phaser->children))
        return;

    Vec f = unit(phaser->phi);
    Vec x = add(phaser->x, scale(f, phaser->radius));
    Vec v = _0;

    Entity *ray = entity_create(&type_ray,phaser->player,x,v);
    entity_attach(phaser, ray);
    ray->active = 1;
}

void gravity(Entity *e0) {
	Entity *e1;
    Real m0 = e0->mass;

    entities_foreach(e1) {
        if(e0->type != e1->type) {
            Real m1 = e1->mass;
            if(m1 == 0) continue;

            Vec dx = sub(e0->x, e1->x);
            Real l  = len(dx);
            Vec r  = normalize(dx);

            /* force is quadratic wrt proximity,
             * and wrt to inverse of mass of e1
             */
            Vec a  = scale(r, gravity_factor * (m0 + m1) / m1 / (l*l)); 
            entity_push(e1, a);
        }
    }
}

void ray_act(Entity *ray) {
	Entity *phaser = ray->parent;
    assert(phaser);
    if(!phaser->active) {
        entity_remove(ray);
        return;
    }

    Vec u = unit(ray->phi);

    Real    best_t;
    Entity *best_e = 0;

    Entity *e;
    entities_foreach(e) {
        if(e == ray) continue;
        if(e == ray->parent) continue;
        if(ray->parent->parent && e == ray->parent->parent) continue;

        Real r = e->radius;
        Vec dx = sub(ray->x, e->x);

        Real t,t0,t1;

        Real a =   dot_sq(u);
        Real b = 2*dot(dx,u);
        Real c =   dot_sq(dx) - r*r;

        int  n =   roots(a,b,c, &t0,&t1);
        if(n) {
                 if(0 < t0 && (t0 < t1 || t1 < 0)) t = t0;
            else if(0 < t1 && (t1 < t0 || t0 < 0)) t = t1;
            else continue;

            if(t > ray->radius)
                continue;

            if(!best_e || t < best_t) {
                best_t = t;
                best_e = e;
            }
        }
    }

    if(best_e) {
        ray->len = best_t;
    } else {
        ray->len = ray->radius;
    }
}

void aim(Entity *rocket) {
	Entity *e;

    Vec     best_v;
    Entity *target = 0;

    entities_foreach(e) {
        if(rocket->player == e->player)
            continue;

        Vec dx = sub(e->x, rocket->x);

        /* desired direction of velocity */
        Vec v = normalize(rotate(dx, -rocket->phi));

        if(v.x < 0) continue; /* target is behind rocket */

        if(!target || fabs(v.y) < fabs(best_v.y)) {
            best_v = v;
            target = e;
        }
    }

    if(target) {
        Real acc   = 1 - fabs(best_v.y);
        Real speed = len(rocket->type->max_a) * acc * acc;
        Vec v = scale(best_v, speed);
        entity_accelerate_to(rocket, v);
        entity_rotate(rocket, best_v.y);
    } else {
        entity_accelerate_to(rocket, _0);
    }
}
