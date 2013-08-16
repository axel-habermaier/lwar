#include <stddef.h>
#include <stdint.h>
#include <string.h>

#include "server_export.h"
#include "server.h"

#include "log.h"
#include "connection.h"
#include "rules.h"

static Server _server;

Server *server=&_server;

bool id_eq(Id id0, Id id1) {
    return    id0.n   == id1.n
           && id0.gen == id1.gen;
}

int server_init() {
    /* initialize static server struct */
    memset(server, 0, sizeof(Server));

	server->conn_clients = conn_init();
    if(!server->conn_clients) return 0;
    if(!conn_bind(server->conn_clients)) return 0;

	server->conn_discovery = conn_init();
    if(!server->conn_discovery) return 0;
	if(!conn_multicast(server->conn_discovery)) return 0;

    protocol_init();
    physics_init();

    entities_init();
    clients_init();

    /* important to initialize rules last */
    rules_init();

    server->running = 1;
	packet_send_discovery(0);

    log_info("Initialized\n");

    return 1;
}

int server_update(Clock time, int force) {
    /* static Clock debug_clock; */

    if(server->running) {
        time_update(time);

        /* skip first frame */
        if(!server->prev_clock)
            return 1;

        /* heartbeat
        if(clock_periodic(&debug_clock, 1000))
            log_debug("server time: %d", server->cur_clock);
        */

        protocol_recv();

        players_update();
        entities_update();
        physics_update();

		queue_stats();
        protocol_send(force);
		packet_send_discovery(time);
        
        /* remove obsolete messages, clients, and entities
         * order is important
         */
        protocol_cleanup();
        clients_cleanup();
        entities_cleanup();

        return 1;
    }
    return 0;
}

void server_shutdown() {
	conn_shutdown(server->conn_clients);
	conn_shutdown(server->conn_discovery);

    /* TODO: shutdown components */
    log_info("Terminated\n");
}
