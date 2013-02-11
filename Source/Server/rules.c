#include <stdint.h>
#include <stddef.h>

#include "server.h"

#include "rules.h"

#define SELF_NAME "server"
static Str self_name = { sizeof(SELF_NAME), SELF_NAME };

void rules_init() {
    server->self = bot_create();
    player_rename(&server->self->player, self_name);

    /* register some entity types */
    entity_type_register(ENTITY_TYPE_SHIP,   type_ship);
    entity_type_register(ENTITY_TYPE_BULLET, type_bullet);
    entity_type_register(ENTITY_TYPE_PLANET, type_planet);
    entity_type_register(ENTITY_TYPE_ROCKET, type_rocket);

    /* and some items */
    item_type_register(ITEM_TYPE_GUN, type_gun);

    //player_select(&server->self->player, ENTITY_TYPE_ROCKET,0);
    //server->self->player.shooting = 1;
    
    
    Vec x = { 500,500 };
    Entity *p0 = entity_create(type_planet, &server->self->player, x, _0);

	Vec x2 = { 800,500 };
    Entity *p1 = entity_create(type_planet, &server->self->player, x2, _0);
    

    /*
    Vec y = { 700, 500 };
    Entity *r0 = entity_create(type_rocket, &server->self->player, y, _0);
    r0->active = 1;
    r0->phi = -2.5;
    */
}

EntityType *entity_type_get(size_t id) {
    if(id < MAX_ENTITY_TYPES)
        return server->entity_types[id];
    else return 0;
}

/* entity type 0 is invalid, should map to NULL */
void entity_type_register(size_t id, EntityType *t) {
    if(0 < id && id < MAX_ENTITY_TYPES)
        server->entity_types[id] = t;
}

ItemType *item_type_get(size_t id) {
    if(id < MAX_ITEM_TYPES)
        return server->item_types[id];
    else return 0;
}

void item_type_register(size_t id, ItemType *t) {
    if(0 < id && id < MAX_ITEM_TYPES)
        server->item_types[id] = t;
}
