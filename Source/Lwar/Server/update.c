#include <stdint.h>
#include <stddef.h>
#include <limits.h>

#include "server.h"
#include "message.h"
#include "uint.h"

size_t update_ship_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, deg100(e->phi));
    i += uint8_pack(s+i, 100 * e->health / e->type->init_health);
    return i;
}

size_t update_pos_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    return i;
}

size_t update_ray_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
	Id none = { 0, USHRT_MAX };
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, deg100(e->phi));
    i += uint16_pack(s+i, e->len);
	i += id_pack(s+i, !e->target ? none : e->target->id);
    return i;
}

size_t update_circle_pack(char *s, void *p) {
    Entity *e = (Entity*)p;
    size_t i=0;
    i += id_pack(s+i, e->id);
    i += int16_pack(s+i, e->x.x);
    i += int16_pack(s+i, e->x.y);
    i += uint16_pack(s+i, e->radius);
    return i;
}


