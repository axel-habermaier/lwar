#include "types.h"

#include "server_export.h"
#include "server.h"

#include "debug.h"
#include "log.h"
#include "rules.h"
#include "physics.h"
#include "protocol.h"
#include "entity.h"
#include "client.h"
#include "queue.h"
#include "packet.h"

#include <stdint.h>
#include <string.h>

static Server _server;

Server *server=&_server;

int server_init(unsigned short port) {
    /* initialize static server struct */
    memset(server, 0, sizeof(Server));
    memset(assert_handler, 0, sizeof(jmp_buf));

    if(!conn_init(&server->conn_clients)) return 0;
    if(!conn_bind(&server->conn_clients, port)) return 0;

    queue_init();
    physics_init();

    entities_init();
    clients_init();

    /* important to initialize rules last */
    rules_init();

    server->running = 1;

    log_info("Initialized\n");

    return 1;
}

static void server_update_internal(Clock time, int force) {
    time_update(time);

    /* skip first frame */
    if(!server->prev_clock)
        return;

    /* heartbeat
    if(clock_periodic(&debug_clock, 1000))
        log_debug("server time: %d", server->cur_clock);
    */

    protocol_recv();

    players_update();
    entities_update();
    physics_update();

    protocol_send(force);
    
    /* remove obsolete messages, clients, and entities
     * order is important
     */
    queue_cleanup();
    clients_cleanup();
    entities_cleanup();
}

int server_update(Clock time, int force) {
    /* static Clock debug_clock; */

    if(!server->running)
        return 0;

    if(setjmp(assert_handler)) {
        log_die("assertion failed %s:%zu '%s'",
                failed_assertion.file, failed_assertion.line,
                failed_assertion.what);
        return -1;
    } else {
        server_update_internal(time, force);
        return 1;
    }
}

void server_shutdown() {
	conn_shutdown(&server->conn_clients);

    rules_shutdown();

    entities_shutdown();
    clients_shutdown();

    physics_shutdown();
    queue_shutdown();

    log_info("Terminated\n");
}
