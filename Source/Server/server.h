#include "list.h"
#include "pool.h"

enum {
    MAX_CLIENTS    = 8,
    MAX_ENTITIES   = 1024,
    MAX_TYPES      = 32,
    MAX_COLLISIONS = 32, /* should be n^2-1 for priority queue */
    MAX_QUEUE      = 1028,

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
typedef struct EntityType EntityType;
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
void player_input(Player *p, int up, int down, int left, int right, int shooting, Pos phi);
void player_select(Player *p, size_t ship_type, size_t weapon_type);
void player_rename(Player *p, Str name);
void player_spawn(Player *p, Vec x);
void player_notify_state(Entity *e);
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

/*
void mqueue_init(List *l);
void mqueue_ack(List *l, uint32_t ack);
void mqueue_destroy(List *l);
size_t packet_scan(char *p, size_t n, Address a);
size_t packet_fmt_queue(Client *c, char *p, size_t n, Address *a);
*/
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

Pos    rad(Pos a); /* radians of a */
size_t deg(Pos a); /* degrees of a */
Pos physics_impact(Vec v0, Vec v1);
void physics_update();

void rules_init();
EntityType *entity_type_get(size_t id);
void entity_type_register(size_t id, EntityType *t);

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
    Entity *ship;
    int up,down,left,right;
    int shooting;
    Pos phi; /* desired orientation */
    EntityType *ship_type;
    EntityType *weapon_type;
    Str name;
};

struct Entity {
    List l;

    Vec x,v,a;  /* position, velocity, acceleration */
    Pos phi,r;  /* orientation angle, rotation (= delta phi) */
    Pos health;
    int dead;

    Id id;
    Player *player;
    EntityType *type;

    Time remaining;

    /* an active entity will fire its type's active function periodically */
    int active;
    Clock activation_periodic;
};

struct EntityType {
    size_t id;
    Pos radius;
    Pos mass;
    Vec max_a;      /* max acceleration */
    Vec max_b;      /* max brake        */
    Pos max_r;      /* max rotation     */
    Pos max_health; /* max health       */

    /* for shooting, ... */
    Clock activation_interval;
    void (*action)(Entity *e);
    void (*collision)(Entity *self, Entity *other, Vec v0, Vec v1);
};

struct Client {
    List l;

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
    Pool queue;
    EntityType *types[MAX_TYPES];

    /* allocated clients */
    unsigned int client_mask;

    int running;
    Clock cur_time;
    Clock prev_time;
    Clock update_periodic;

    Client *self;
};

static const Vec  _0  = {0,0};

#define clients_foreach(c)       pool_foreach(&server->clients, c, Client)
#define clients_foreach_cont(c)  pool_foreach_cont(&server->clients, c, Client)

#define entities_foreach(e)      pool_foreach(&server->entities, e, Entity)
#define entities_foreach_cont(e) pool_foreach_cont(&server->entities, e, Entity)

#define queue_foreach(qm)        pool_foreach(&server->queue, qm, QueuedMessage)
#define queue_foreach_cont(qm)   pool_foreach_cont(&server->queue, qm, QueuedMessage)

#ifndef max
#define max(n,m) ((n) < (m) ? (m) : (n))
#endif

#ifndef sgn
#define sgn(n) ((n) == 0 ? 0 : ((n) < 0 ? -1 : 1))
#endif

#ifndef min
#define min(n,m) ((n) > (m) ? (m) : (n))
#endif
