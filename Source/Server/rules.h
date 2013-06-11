/* register in rules.c, implement in rules/$type.c */
extern EntityType type_ship;
extern EntityType type_bullet;
extern EntityType type_planet;
extern EntityType type_sun;
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

#include "entity.h"