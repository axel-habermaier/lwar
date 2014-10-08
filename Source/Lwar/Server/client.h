#ifndef CLIENT_H
#define CLIENT_H

#include "address.h"
#include "clock.h"
#include "list.h"
#include "player.h"

struct Client {
    List _l;

    Player player;
    Address adr;
    size_t ping;   /* TODO: implement */

    bool remote;   /* adr is valid */
    // bool hasleft;  /* has actively disconnected */
    bool dead;     /* memory will be released, don't use any more */

    size_t next_out_reliable_seqno;
	size_t next_out_unreliable_seqno;

	size_t last_in_reliable_seqno;
	size_t last_in_unreliable_seqno;

    size_t last_in_ack;
    size_t last_in_frameno;
    Clock  last_activity;

    /* count protocol violations */
    size_t misbehavior;
};

void    clients_init();
void    clients_cleanup();
void    clients_shutdown();

Client *client_create(Address *adr);
Client *client_create_local();
void    client_remove(Client *c);
Client *client_get(Id player);
Client *client_lookup(Address *adr);


#endif
