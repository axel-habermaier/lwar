#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>
#include <time.h>

#include "server.h"
#include "message.h"
#include "vector.h"

#include "rules.h"

#ifdef _MSC_VER // INFINITY is not defined
#include <limits>
#define INFINITY std::numeric_limits<float>::infinity()
#endif

#define SELF_NAME "server"
static Str self_name = { sizeof(SELF_NAME)-1, SELF_NAME };
static const Real gravity_factor = 10000; // 0.04;

static void level_init() {
    size_t i;

    srand(time(0));

    Entity *sun = entity_create(&type_sun, &server->self->player, _0, _0);
	sun->active=true;
	
    for(i=0; i<MAX_PLANETS; i++) {
        Real dist = (i+2) * MIN_PLANET_DIST; // + rand()%(MAX_PLANET_DIST - MIN_PLANET_DIST);

        Real phi = rad(rand()%360);
        Vec  u = unit(phi);
        Vec  x = scale(u, dist);
        Entity *p = entity_create(&type_planet, &server->self->player, x, _0);
        p->active = true;
        p->len    = dist;
        p->energy = rad(180 + rand()%360); /* speed of rotation around sun per second */
        p->radius += rand()%(unsigned)p->radius;
    }
}

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
    entity_type_register("ship",   &type_ship,      &format_ship);
    entity_type_register("bullet", &type_bullet,    &format_pos);
    entity_type_register("planet", &type_planet,    &format_pos);
    entity_type_register("sun",    &type_sun,       &format_pos);
    entity_type_register("rocket", &type_rocket,    &format_ship);
    entity_type_register("ray",    &type_ray,       &format_ray);
    // entity_type_register(ENTITY_TYPE_SHOCKWAVE, &type_shockwave, &format_circle);

    entity_type_register("gun",    &type_gun,       0); /* not shared with client */
    entity_type_register("phaser", &type_phaser,    0);

    level_init();
}

EntityType *entity_type_get(size_t id) {
    /* note: id 0 is not used as an enumeration value */
    if(id < MAX_ENTITY_TYPES)
        return array_at(&server->types, EntityType*, id);
    else return 0;
}

/* entity type 0 is invalid, should map to NULL */
void entity_type_register(const char *name, EntityType *t, Format *f) {
    size_t id = t->id;
    if(0 < id && id < MAX_ENTITY_TYPES) {
        array_at(&server->types, EntityType*, id) = t;
        t->name   = name;
        t->format = f;
    }
}

void bullet_hit(Entity *self, Entity *other, Real impact) {
    entity_hit(self,  impact, other->player);
    /* inflict some additional damage */
    entity_hit(other, self->energy, self->player);
}

void ship_hit(Entity *self, Entity *other, Real impact) {
    entity_hit(self, impact, other->player);
}

void planet_hit(Entity *self, Entity *other, Real impact) {
    /* immediately kill other entity */
    entity_hit(other, INFINITY, self->player);
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
    Vec u = normalize(sub((gun->player->aim), x));
    Vec v = add(gun->v, scale(u, type_bullet.max_a.y)); /* initial speed */
    Entity *bullet = entity_create(&type_bullet,gun->player,x,v);
    bullet->active = 1;
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

    /* creates an active ray (which removes itself when phaser becomes inactive) */
    Entity *ray = entity_create(&type_ray,phaser->player,x,v);
    entity_attach(phaser, ray, _0, 0);
    ray->active = 1;
}

void gravity(Entity *e0) {
	Entity *e1;
    Real m0 = e0->mass;

    entities_foreach(e1) {
        if(e1->type->id == ENTITY_TYPE_SUN)    continue;
        if(e1->type->id == ENTITY_TYPE_PLANET) continue;

        Real m1 = e1->mass;
        if(m1 == 0) continue;

        Vec dx = sub(e0->x, e1->x);
        Real l = len(dx);
        Vec r  = normalize(dx);

        /* force is quadratic wrt proximity,
         * and wrt to inverse of mass of e1
         */
        Vec a  = scale(r, gravity_factor * (m0 + m1) / m1 / (l*l)); 
        entity_push(e1, a);
    }

    /* planet's movement */
    Vec  old_x = e0->x;
    Real old_phi   = arctan(old_x);
    Real delta_phi = e0->energy * time_delta();
    Vec  new_x = scale(unit(old_phi + delta_phi), e0->len);
    e0->v = sub(new_x, old_x);
}

void ray_act(Entity *ray) {
	Entity *phaser = ray->parent;
    assert(phaser);

	Entity *ship   = phaser->parent;
    assert(ship);

    /* the ray is deleted as soon as the phaser is inactive */
    if(!phaser->active) {
        entity_remove(ray);
        return;
    }

    Vec u = normalize(sub((ray->player->aim), phaser->x));
    ray->dphi = arctan(u) - phaser->phi;

    Real    best_t;
    Entity *best_e = 0;

    Entity *e;
    entities_foreach(e) {
        if(e == ray)    continue;
        if(e == phaser) continue;
        if(e == ship)   continue;

        /* TODO: partially merge into physics code */
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
        /* damage is proportional to frame time */
        entity_hit(best_e, ray->energy * clock_delta(), ray->player);
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
