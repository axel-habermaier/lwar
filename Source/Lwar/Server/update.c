#include "types.h"

#include "update.h"

#include "pack.h"
#include "entity.h"
#include "id.h"
#include "list.h"
#include "message.h"
#include "real.h"
#include "server.h"
#include "uint.h"

Format format_pos_rot = { {0,0}, MESSAGE_UPDATE,        update_pos_rotation_pack, 0 };
Format format_pos     = { {0,0}, MESSAGE_UPDATE_POS,    update_pos_pack,          0 };
Format format_ray     = { {0,0}, MESSAGE_UPDATE_RAY,    update_ray_pack,          0 };
Format format_circle  = { {0,0}, MESSAGE_UPDATE_CIRCLE, update_circle_pack,       0 };
Format format_ship    = { {0,0}, MESSAGE_UPDATE_SHIP,   update_ship_pack,         0 };

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
