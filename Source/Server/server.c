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
    if(server->running) {
        time_update(time);

        /* skip first frame */
        if(!server->prev_time)
            return 1;
	
        protocol_recv();

        if(   force
           || clock_periodic(&server->update_periodic, UPDATE_INTERVAL))
        {
            protocol_send();
        }

        player_actions();
        entity_actions();

        physics_update();

        return 1;
    }
    return 0;
}

void server_shutdown() {
    conn_shutdown();
    log_info("Terminated\n");
}
