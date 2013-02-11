#include "list.h"
#include "pool.h"

enum {
    MAX_CLIENTS    = 8,
    MAX_ITEMS      = 1024,
    MAX_ENTITIES   = 1024,
    MAX_ENTITY_TYPES = 32,
    MAX_ITEM_TYPES = 32,
    MAX_COLLISIONS = 32, /* should be n^2-1 for priority queue */
    MAX_QUEUE      = 1024,

    TIMEOUT_INTERVAL  = 15 * 1000 /*ms*/,
    MISBEHAVIOR_LIMIT = 10,
};

typedef struct Vec Vec;
typedef float      Pos;

typedef struct Str Str;
typedef struct Id Id;
typedef struct Address Address;

/* clock_t on unix */
typedef unsigned long long Clock;

/* in seconds, used to prevent implicit conversions from Clock */
typedef struct { Pos t; } Time;
int  time_cmp(Time d0, Time d1);
Time time_add(Time d0, Time d1);
Time time_sub(Time d0, Time d1);

typedef struct Entity Entity;
typedef struct Item Item;
typedef struct EntityType EntityType;
typedef struct ItemType ItemType;
typedef struct Player Player;

typedef struct Client Client;
typedef struct Server Server;

extern Server *server;

int id_eq(Id id0, Id id1);
int address_eq(Address *adr0, Address *adr1);

Clock clock_delta();
int   clock_periodic(Clock *t, Clock i);
Clock to_clock(Time d);
Time  to_time(Clock t);
int   time_cmp(Time d0, Time d1);
void  time_update(Clock t);

void player_init(Player *p, size_t id);
void player_clear(Player *p);
void player_input(Player *p, int up, int down, int left, int right, int fire1, int fire2, Pos phi);
void player_select(Player *p, size_t ship_type, size_t weapon_type);
void player_rename(Player *p, Str name);
void player_spawn(Player *p, Vec x);
void player_notify_state(Entity *e);
void player_pickup(Player *p, Item *i);
void player_die(Player *p);
void players_update();

void clients_init();
void clients_cleanup();
Client *bot_create();
int client_isbot(Client *c);
Client *client_create(Address *adr);
void client_remove(Client *c);
Client *client_get(Id player);
Client *client_lookup(Address *adr);

void protocol_init();
void protocol_recv();
void protocol_send(int force);
void protocol_notify_state(Entity *e);
void protocol_notify_collision(Entity *e0, Entity *e1, Vec v);
void protocol_cleanup();

void entities_init();
void entities_cleanup();
void entities_update();
Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v);
void entity_remove(Entity *e);
void entities_notify_collision(Entity *e0, Entity *e1, Vec v0, Vec v1);

void entity_push(Entity *e, Vec a);
void entity_accelerate(Entity *e, Vec a);
void entity_accelerate_to(Entity *e, Vec v);
void entity_rotate(Entity *e, Pos r);
void entity_attach(Entity *e, Item *j);

Item *item_create(ItemType *t);
void item_remove(Item *j);
void items_init();
void items_cleanup();
void items_update(Entity *e);

Pos    rad(Pos a); /* radians of a */
size_t deg(Pos a); /* degrees of a */
Pos physics_impact(Vec v0, Vec v1);
void physics_update();

void rules_init();
EntityType *entity_type_get(size_t id);
void entity_type_register(size_t id, EntityType *t);
ItemType *item_type_get(size_t id);
void item_type_register(size_t id, ItemType *t);

struct Vec {
    Pos x,y;
};

struct Str {
    unsigned char n;
    char  *s;
};

struct Id {
    uint16_t gen;
    uint16_t n;
};

struct Address {
	uint32_t ip;
	uint16_t port;
};

struct Player {
    Id id;
    Str name;

    /* gameplay */
    Entity *ship;
    Item   *weapon;
    EntityType *ship_type;   /* selection for next spawn */
    ItemType   *weapon_type;
    List inventory;          /* additional items, not activated */

    /* input state */
    int up,down,left,right;
    int fire1,fire2;
    Pos phi; /* desired orientation */
};

/* Entities have a physical appearance in the world */
struct Entity {
    List _l;
    EntityType *type;

    Id id;
    int dead;
    Clock age;

    /* gameplay */
    Player *player;
    List  attached; /* of items (weapons, also harvest) */

    /* physics */
    Vec x,v,a;  /* position, velocity, acceleration */
    Pos phi,r;  /* orientation angle, rotation (= delta phi) */
    Pos health;
    Time remaining;
};

/* Rays are essentially entities */
struct Ray {
    List _l;
    EntityType *type;

    Id id;
    int dead;
    Clock age;

    Entity *parent;

    Vec x;
    Pos phi;
    Pos len;
};

struct EntityType {
    size_t id;

    /* gameplay */
    void (*act)(Entity *e);
    void (*collide)(Entity *self, Entity *other, Vec v0, Vec v1);

    /* physics */
    Pos radius;
    Pos mass;
    Vec max_a;      /* max acceleration */
    Vec max_b;      /* max brake        */
    Pos max_r;      /* max rotation     */
    Pos max_health; /* max health       */
};

/* Items, such as weapons, may be attached to players/entities,
 * but have no manifestation in the world on their own
 */
struct Item {
    List _l;
    ItemType *type;

    Id id;
    int dead;
    List h;

    /* gameplay */
    unsigned int fire_mask;
    Clock fire_periodic;
    size_t ammunition;
};

struct ItemType {
    size_t id;

    /* gameplay */
    void (*fire)(Entity *e, Item *j);
    Clock fire_interval;
    size_t initial_ammunition;
};

struct Client {
    List _l;

    Player player;
    Address adr;

    int hasleft;  /* has actively disconnected */
    int dead;     /* memory will be released, don't use any more */

    size_t next_out_seqno;

    size_t last_in_ack;
    size_t last_in_seqno;
    size_t last_in_frameno;
    Clock  last_activity;

    /* count protocol violations */
    size_t misbehavior;
};

struct Server {
    Pool clients;
    Pool entities;
    Pool items;
    Pool queue;
    EntityType *entity_types[MAX_ENTITY_TYPES];
    ItemType   *item_types[MAX_ITEM_TYPES];

    /* set of allocated clients */
    unsigned int client_mask;

    int running;
    Clock cur_time;
    Clock prev_time;
    Clock update_periodic;

    Client *self;
};

static const Vec  _0  = {0,0};

#define clients_foreach(c)  pool_foreach(&server->clients, c, Client)
#define entities_foreach(e) pool_foreach(&server->entities, e, Entity)
#define items_foreach(j)    pool_foreach(&server->items, j, Item)
#define queue_foreach(qm)   pool_foreach(&server->queue, qm, QueuedMessage)
#define attached_foreach(e,j)  list_for_each_entry(j, Item, &e->attached,  h)
#define inventory_foreach(p,j) list_for_each_entry(j, Item, &p->inventory, h)

#ifndef max
#define max(n,m) ((n) < (m) ? (m) : (n))
#endif

#ifndef sgn
#define sgn(n) ((n) == 0 ? 0 : ((n) < 0 ? -1 : 1))
#endif

#ifndef min
#define min(n,m) ((n) > (m) ? (m) : (n))
#endif
