#include "types.h"

#include "client.h"

#include "log.h"
#include "state.h"

static Client _clients[MAX_CLIENTS];

static void client_ctor(size_t i, void *p) {
    Client *c = (Client*)p;
    c->next_out_reliable_seqno    = 1; /* important to start with one */
	c->next_out_unreliable_seqno  = 1; 
    c->last_in_ack                = 0;
    c->last_in_reliable_seqno     = 0;
	c->last_in_unreliable_seqno   = 0;
    c->last_in_frameno            = 0;
    c->last_activity              = 0;
    c->misbehavior                = 0;
    c->hasleft                    = 0;
    c->dead                       = 0;

    player_init(&c->player, i);
}

static void client_dtor(size_t i, void *p) {
    Client *c = (Client*)p;
    player_clear(&c->player);
    c->player.id.gen ++;
}

static bool client_check_obsolete(size_t i, void *p) {
    Client *c = (Client *)p;
    return c->dead; /* (server->client_mask & (1 << i)) == 0; */
}

Client *client_create(Address *adr) {
    Client *c = pool_new(&server->clients, Client);
    if(c) {
        c->adr = *adr;
        c->remote = true;
        set_insert(server->connected, c->player.id.n);
        log_debug("+ client %d", c->player.id.n);
    }
    return c;
}

Client *client_create_local() {
    Client *c = pool_new(&server->clients, Client);
    if(c) {
        c->adr = address_none;
        c->remote = false;
        log_debug("+ bot %d", c->player.id.n);
    }
    return c;
}

void client_remove(Client *c) {
    c->dead = true;
    set_remove(server->connected, c->player.id.n);
    log_debug("- client %d", c->player.id.n);
}

Client *client_lookup(Address *adr) {
    if(!adr) return 0;
    Client *c;
    clients_foreach(c) {
        if(c->remote && address_eq(&c->adr, adr))
            return c;
    }
    return 0;
}

Client *client_get(Id player) {
    Client *c = pool_at(&server->clients, Client, player.n);
    if(!c) return 0;
    return 0;
}

void clients_init() {
    pool_static(&server->clients, _clients, client_ctor, client_dtor);
    /* pool_dynamic(&server->clients, Client, MAX_CLIENTS, client_ctor, client_dtor); */
}

void clients_cleanup() {
    pool_free_pred(&server->clients, client_check_obsolete);
}
