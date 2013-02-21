/* register in rules.c, implement in rules/$type.c */
extern EntityType type_ship;
extern EntityType type_bullet;
extern EntityType type_planet;
extern EntityType type_rocket;
extern EntityType type_ray;
extern EntityType type_shockwave;

extern EntityType type_gun;
extern EntityType type_phaser;

/* register in rules.c, implement in format.c, message.c, update.c */
// extern Format     format_message;
extern Format     format_ship;
extern Format     format_pos;
extern Format     format_ray;
extern Format     format_circle;

enum {
    ENTITY_TYPE_SHIP      =   1,
    ENTITY_TYPE_BULLET    =   2,
    ENTITY_TYPE_PLANET    =   3,
    ENTITY_TYPE_ROCKET    =   4,
    ENTITY_TYPE_RAY       =   5,
    ENTITY_TYPE_SHOCKWAVE =   6,

    ENTITY_TYPE_GUN       =   7,
    ENTITY_TYPE_PHASER    =   8,
};
