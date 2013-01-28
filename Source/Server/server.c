#include <stdint.h>
#include <stddef.h>

#include "server_export.h"
#include "server.h"

#include "log.h"
#include "connection.h"
#include "rules.h"

static Server _server;
Server *server=&_server;

enum {
    UPDATE_INTERVAL = 30,
};

/* handle all pending incoming messages */
static void server_receive() {
    size_t n;
    Address a;
    char p[MAX_PACKET_LENGTH];
    for(;;) {
        int more = conn_receive(p, &n, &a);
        if(!more || !n) break;
        packet_scan(p, n, a);
    }
}

/* send queued reliable messages
 * TODO: put some timeouts in */
static void server_send() {
    size_t n;
    Client *c;
    Address a;
    char p[MAX_PACKET_LENGTH];

    for_each_connected_client(server, c) {
        for(;;) {
            n = packet_fmt_queue(c, p, MAX_PACKET_LENGTH, &a);
            if(!n) break;
            conn_send(p, n, &a);
        }
    }
}

int server_init() {
    INIT_LIST_HEAD(&server->allocated);
    INIT_LIST_HEAD(&server->created);
    INIT_LIST_HEAD(&server->free);

    clients_init();
    entities_init();
    // rules_init();

    if(!conn_init()) return 0;
    if(!conn_bind()) return 0;

    /* register some entity types */
    entity_type_register(ENTITY_TYPE_SHIP,   type_ship);
    entity_type_register(ENTITY_TYPE_BULLET, type_bullet);
    entity_type_register(ENTITY_TYPE_PLANET, type_planet);

    server->running = 1;

    log_info("Initialized\n");

    return 1;
}

int server_update(Clock time, int force) {
    if(server->running) {
        time_update(time);

        /* skip first frame */
        if(!server->prev_time)
            return 1;
	
        server_receive();

        if(   force
           || clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        {
            server_send();
        }

        player_actions();
        entity_actions();

        list_splice_init(&server->created, &server->allocated);

        physics_update();

        return 1;
    }
    return 0;
}

void server_shutdown() {
    conn_shutdown();
    log_info("Terminated\n");
}
