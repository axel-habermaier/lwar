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

    //player_select(&server->self->player, ENTITY_TYPE_ROCKET,0);
    //server->self->player.shooting = 1;

    
    Vec x = { 500,500 };
    Entity *p0 = entity_create(type_planet, &server->self->player, x, _0);
    p0->active = 1;
    

    /*
    Vec y = { 700, 500 };
    Entity *r0 = entity_create(type_rocket, &server->self->player, y, _0);
    r0->active = 1;
    r0->phi = -2.5;
    */
}
