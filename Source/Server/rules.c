#include <stdint.h>
#include <stddef.h>

#include "server.h"
#include "message.h"

#include "rules.h"

#define SELF_NAME "server"
static Str self_name = { sizeof(SELF_NAME)-1, SELF_NAME };

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
