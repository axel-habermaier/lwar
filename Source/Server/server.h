#include "list.h"
#include "slab.h"

enum {
    MAX_CLIENTS    = 8,
    MAX_ENTITIES   = 1024,
    MAX_TYPES      = 32,
    MAX_COLLISIONS = 32, /* should be n^2-1 for priority queue */
    MAX_QUEUE      = 1028,
};

typedef struct Vec Vec;
typedef float      Pos;

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

Clock clock_delta();
int   clock_periodic(Clock *t, Clock i);
Clock to_clock(Time d);
Time  to_time(Clock t);
int   time_cmp(Time d0, Time d1);
void  time_update(Clock t);

void player_input(Player *p, int up, int down, int left, int right, int shooting);
void player_actions();

void clients_init();
/* void client_update(Client *c); */
Client *client_create();
void client_remove(Client *c);
Client *client_get(Id player, Address *a);

/*
void mqueue_init(List *l);
void mqueue_ack(List *l, uint32_t ack);
void mqueue_destroy(List *l);
size_t packet_scan(char *p, size_t n, Address a);
size_t packet_fmt_queue(Client *c, char *p, size_t n, Address *a);
*/
void protocol_init();
void protocol_recv();
void protocol_send();

void entities_init();
void entity_actions();
Entity *entity_create(EntityType *t, Vec x, Vec v);
void entity_remove(Entity *e);
Pos entity_radius(Entity *e);
Pos entity_mass(Entity *e);
Vec entity_acc(Entity *e);

float rad(float a); /* radians of a */
void physics_update();
void physics_acc(Entity *e, Vec a);
void physics_acc_rel(Entity *e, Vec a);

void rules_init();
EntityType *entity_type_get(size_t id);
void entity_type_register(size_t id, EntityType *t);

struct Vec {
    Pos x,y;
};

static const Vec  _0  = {0,0};

struct Id {
    uint16_t gen;
    uint16_t n;
};

struct Address {
	uint32_t ip;
	uint16_t port;
};

struct Player {
    Entity *ship;
    int up,down,left,right;
    int shooting;
    Id id;
};

struct Entity {
    List l;

    Vec x,v,a;
    Pos rot;
    int health;

    Id id;
    EntityType *type;

    Time remaining;

    /* an active entity will fire its type's active function periodically */
    int active;
    Clock activation_periodic;
};

struct EntityType {
    Pos r; /* radius */
    Pos m; /* mass   */
    Vec a; /* acceleration */

    Clock activation_interval; /* for shooting, ... */
    void (*action)(Entity *e);
};

struct Client {
    List l;

    Player player;
    Address adr;

    size_t next_out_seqno;
    size_t last_in_ack;

    size_t last_in_seqno;
};

struct Server {
    Slab clients;
    Slab entities;
    Slab queue;
    EntityType *types[MAX_TYPES];

    int running;
    Clock cur_time;
    Clock prev_time;
    Clock update_periodic;
};

#define clients_foreach(c)       slab_foreach(&server->clients, c, Client)
#define clients_foreach_cont(c)  slab_foreach_cont(&server->clients, c, Client)

#define entities_foreach(e)      slab_foreach(&server->entities, e, Entity)
#define entities_foreach_cont(e) slab_foreach_cont(&server->entities, e, Entity)

#define queue_foreach(qm)        slab_foreach(&server->queue, qm, QueuedMessage)
#define queue_foreach_cont(qm)   slab_foreach_cont(&server->queue, qm, QueuedMessage)
