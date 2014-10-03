#include "types.h"

#include "update.h"

#include "entity.h"
#include "id.h"
#include "list.h"
#include "message.h"
#include "real.h"
#include "state.h"
#include "uint.h"

#include <limits.h>

Format format_ship    = { {0,0}, MESSAGE_UPDATE,        update_ship_pack,   0 };
Format format_pos     = { {0,0}, MESSAGE_UPDATE_POS,    update_pos_pack,    0 };
Format format_ray     = { {0,0}, MESSAGE_UPDATE_RAY,    update_ray_pack,    0 };
Format format_circle  = { {0,0}, MESSAGE_UPDATE_CIRCLE, update_circle_pack, 0 };

void format_register(Format *f) {
    INIT_LIST_HEAD(&f->all);

    f->len = 0;
    f->n   = 0;

    if(1) {
        char s[64];
        Entity e = {};
        EntityType t = {};
        e.type = &t;
        f->len = f->pack(s,&e);
    }

    list_add_tail(&f->_l, &server->formats);
}


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


