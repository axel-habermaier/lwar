#ifndef TYPES_H
#define TYPES_H

#include <stdbool.h>
#include <stddef.h>

#include "list.h"

typedef struct Client Client;
typedef struct Collision Collision;
typedef struct Connection Connection;
typedef struct Entity Entity;
typedef struct EntityType EntityType;
typedef struct Format Format;
typedef struct Player Player;
typedef struct Slot Slot;
typedef struct SlotType SlotType;

typedef size_t (Pack)(char *, void *);
typedef size_t (Unpack)(const char *, void *);

#endif
