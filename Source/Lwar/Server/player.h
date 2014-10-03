#ifndef PLAYER_H
#define PLAYER_H

#include "bitset.h"
#include "config.h"
#include "entity.h"
#include "str.h"
#include "vector.h"

struct Player {
    Id id;
    Str name;

    /* gameplay */
    Slot ship;
    Slot weapons[NUM_SLOTS];
    size_t kills,deaths;

    /* input state */
    Vec a;
    Real rot;
    Vec aim;
};

void player_init(Player *p, size_t id);
void player_clear(Player *p);
void player_input(Player *p,
                  int forwards,    int backwards,
                  int turn_left,   int turn_right,
                  int strafe_left, int strafe_right,
                  int fire1, int fire2, int fire3, int fire4,
                  int aim_x, int aim_y);
void player_select(Player *p,
                   int ship_type_id,
                   int weapon_type_id1, int weapon_type_id2,
                   int weapon_type_id3, int weapon_type_id4);
void player_rename(Player *p, Str name);
void player_spawn(Player *p, Vec x);
void player_notify_entity(Entity *e);
void players_update();

#endif
