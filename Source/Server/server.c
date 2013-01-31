#include <stdint.h>
#include <stddef.h>

#include "server_export.h"
#include "server.h"

#include "log.h"
#include "connection.h"
#include "rules.h"

static Server _server;

Server *server=&_server;

int id_eq(Id id0, Id id1) {
    return    id0.n   == id1.n
           && id0.gen == id1.gen;
}

enum {
    UPDATE_INTERVAL = 30,
};

int server_init() {
    if(!conn_init()) return 0;
    if(!conn_bind()) return 0;

    clients_init();
    entities_init();
    protocol_init();

    /* register some entity types */
    entity_type_register(ENTITY_TYPE_SHIP,   type_ship);
    entity_type_register(ENTITY_TYPE_BULLET, type_bullet);
    entity_type_register(ENTITY_TYPE_PLANET, type_planet);

    server->running = 1;

    log_info("Initialized\n");

    return 1;
}

int server_update(Clock time, int force) {
    static Clock debug_clock;

    if(server->running) {
        time_update(time);

        /* skip first frame */
        if(!server->prev_time)
            return 1;

        if(clock_periodic(&debug_clock, 1000))
            log_debug("server time: %d", server->cur_time);
	
        protocol_recv();

        if(   force
           || clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        {
            protocol_send();
        }

        player_actions();
        entity_actions();

        physics_update();
        
        /* remove obsolete messages, clients, and entities */
        protocol_cleanup();
        clients_cleanup();
        entities_cleanup();

        return 1;
    }
    return 0;
}

void server_shutdown() {
    conn_shutdown();
    log_info("Terminated\n");
}
