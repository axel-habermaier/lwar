#ifndef _MSC_VER
#include <stdbool.h>
#endif

#include "bitset.h"
#include "list.h"
#include "array.h"
#include "pq.h"
#include "pool.h"

enum {
    MAX_CLIENTS         =    8,
    MAX_ENTITIES        = 4096,
    MAX_ENTITY_TYPES    =   32,
    MAX_COLLISIONS      =   32, /* should be n^2-1 for priority queue */
    MAX_QUEUE           = 128,
    MAX_STRINGS         =  128,

    NUM_SLOTS           =    4,

    TIMEOUT_INTERVAL    =   15 * 1000 /*ms*/,
    MISBEHAVIOR_LIMIT   =   10,

    MAX_PLANETS         = 10,
    MIN_PLANET_DIST     = 1500,
    MAX_PLANET_DIST     = 2500,
};

typedef size_t (Pack)(char *, void *);
typedef size_t (Unpack)(const char *, void *);

typedef struct Vec Vec;
typedef float      Real;

typedef struct Str Str;
typedef struct Id Id;
typedef struct Address Address;

/* clock_t on unix */
/* measures discrete time steps in milliseconds */
typedef unsigned long long Clock;

/* measures time intervals,
 * used for example by the physics engine
 */
typedef Real Time;

typedef struct Entity Entity;
typedef struct Slot Slot;

typedef struct EntityType EntityType;
typedef struct SlotType SlotType;

typedef struct Format Format;

typedef struct Collision Collision;

typedef struct Player Player;
typedef struct Client Client;
typedef struct Server Server;

extern Server *server;

bool id_eq(Id id0, Id id1);
bool address_eq(Address *adr0, Address *adr1);

Clock clock_delta();
bool  clock_periodic(Clock *t, Clock i);
bool  clock_periodic_active(Clock *t, Clock i, bool active);
Clock to_clock(Time t);
Time  to_time(Clock t);
Time  time_delta();
int   time_cmp(Time t0, Time t1);
void  time_update(Clock t);

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

void    clients_init();
void    clients_cleanup();
Client *client_create(Address *adr);
Client *client_create_local();
void    client_remove(Client *c);
Client *client_get(Id player);
Client *client_lookup(Address *adr);

void queue_timeout(Client *c);
void queue_gamestate_for(Client *cn);
void queue_join(Client *c);
void queue_leave(Client *c);
void queue_collision(Collision *c);
void queue_add(Entity *e);
void queue_remove(Entity *e);
void queue_kill(Player *k, Player *v);
void queue_stats();

void protocol_init();
void protocol_recv();
void protocol_send(bool force);
void protocol_notify_entity(Entity *e);
void protocol_notify_collision(Collision *c);
void protocol_notify_kill(Player *k, Player *v);
void protocol_cleanup();

void    entities_init();
void    entities_cleanup();
void    entities_update();
Entity *entity_create(EntityType *t, Player *p, Vec x, Vec v);
void    entity_remove(Entity *e);
void    entities_notify_collision(Collision *c);

void entity_push(Entity *e, Vec a);
void entity_accelerate(Entity *e, Vec a);
void entity_accelerate_to(Entity *e, Vec v);
void entity_rotate(Entity *e, Real r);
void entity_hit(Entity *e, Real damage, Player *p);
void entity_attach(Entity *e, Entity *c, Vec dx, Real dphi);

Real   rad(Real a); /* radians of a */
size_t deg(Real a); /* degrees of a */
size_t deg100(Real a);
void physics_init();
void physics_cleanup();
void physics_update();

void rules_init();

void format_register(Format *f);
void format_add_entity(Format *f, Entity *e);

EntityType *entity_type_get(size_t id);
void entity_type_register(const char *name, EntityType *t, Format *f);

struct Vec {
    Real x,y;
};

struct Str {
    unsigned char n;
    char *s;
};

struct Id {
    uint16_t gen;
    uint16_t n;
};

struct Address {
	uint8_t ip[16];
	uint16_t port;
	bool isIPv6;
};

struct Slot {
    Entity   *entity;
    EntityType *selected_type;
};

struct SlotType {
    Vec  dx;
    Real dphi;
    BitSet possible_types;
};

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

/* Entities have a physical appearance in the world */
struct Entity {
    List _l;
    List _u;       /* the format list */

    EntityType *type;

    Id id;
    bool dead;
    Clock age;

    /* gameplay */
    Player *player;

    List children;  /* structured as a tree */
    List siblings;  /* with sibling lists   */
    Entity *parent; /* and parent pointers  */

    Slot   *slot;   /* if directly controlled by a player */

    bool active;
    Clock interval;
    Clock periodic;

    /* physics */
    Vec x,v,a;    /* world position, absolute velocity, acceleration */
    Real phi,rot; /* orientation angle, rotation (= delta phi) */

    Vec  dx;      /* position and angle relative to parent */
    Real dphi;

    Real energy;  /* ammunition, fuel, damage ... */
    Real health;
    Real shield;  /* damage multiplier */
    Real len;
    Real mass;
    Real radius;
    Time remaining;

    bool collides;
    bool bounces;
};

struct EntityType {
    size_t id;

    /* gameplay */
    void (*act)(Entity *self);
    void (*collide)(Entity *self, Entity *other, Real impact);

    Clock init_interval; /* for activation */

    /* physics */
    Real init_energy;
    Real init_health;
    Real init_shield;
    Real init_len;
    Real init_mass;
    Real init_radius;
    Vec  max_a;      /* acceleration */
    Vec  max_b;      /* brake        */
    Real max_rot;    /* rotation     */

    Format *format;

    const char *name;
    SlotType slots[NUM_SLOTS]; /* possible weapon attachments */
};

struct Format {
    List _l;
    size_t id;

    Pack *pack;
    Unpack *unpack;
    List  all;
    size_t len;
    size_t n;
};

struct Collision {
    Time t;
    Entity *e[2];
    Real    i[2];
    Vec x;
};

struct Client {
    List _l;

    Player player;
    Address adr;

    bool remote;   /* adr is valid */
    bool hasleft;  /* has actively disconnected */
    bool dead;     /* memory will be released, don't use any more */

    size_t next_out_seqno;

    size_t last_in_ack;
    size_t last_in_seqno;
    size_t last_in_frameno;
    Clock  last_activity;

    /* count protocol violations */
    size_t misbehavior;
};

struct Server {
    bool      running;

    Pool      clients;
    BitSet    connected;

    Pool      entities;
    Pool      queue;
    Array     types;
    List      formats;
    PrioQueue collisions;
    Pool      strings;

    Clock     cur_clock;
    Clock     prev_clock;
    Clock     update_periodic;

    Client   *self;
};

static const Vec _0 = {0,0};
static const Address address_none = {{}, 0,0};

#define clients_foreach(c)       pool_foreach(&server->clients, c, Client)
#define entities_foreach(e)      pool_foreach(&server->entities, e, Entity)
#define queue_foreach(qm)        pool_foreach(&server->queue, qm, QueuedMessage)
#define children_foreach(e0,e1)  list_for_each_entry(e1, Entity, &e0->children, siblings)
#define collisions_foreach(c)    pq_foreach(&server->collisions, c, Collision)
#define formats_foreach(f)       list_for_each_entry(f, Format, &server->formats, _l)
#define updates_foreach(t,e)     list_for_each_entry(e, Entity, &t->all, _u)
#define slots_foreach(p,s,st)    for(s = p->weapons, st = (p->ship.entity ? p->ship.entity->type->slots : 0); s<p->weapons+NUM_SLOTS; s++, st = (p->ship.entity ? st + 1 : 0))

#ifndef max
#define max(n,m) ((n) < (m) ? (m) : (n))
#endif

#ifndef sgn
#define sgn(n) ((n) == 0 ? 0 : ((n) < 0 ? -1 : 1))
#endif

#ifndef min
#define min(n,m) ((n) > (m) ? (m) : (n))
#endif

int roots(Real a, Real b, Real c, Real *x0, Real *x1);
