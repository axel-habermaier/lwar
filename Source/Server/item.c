#include <assert.h>
#include <math.h>
#include <stdint.h>
#include <stddef.h>

#include "server.h"
#include "vector.h"
#include "log.h"

static Item _items[MAX_ITEMS];

static void item_ctor(size_t i, void *p) {
    Item *j = (Item*)p;
    j->id.n = i;
    j->dead = 0;
    INIT_LIST_HEAD(&j->h);
}

static void item_dtor(size_t i, void *p) {
    Item *j = (Item*)p;
    list_del(&j->h);
    j->id.gen ++;
}

static int item_check_obsolete(size_t i, void *p) {
    Item *j = (Item*)p;
    return j->dead;
}

Item *item_create(ItemType *t) {
    assert(t);
    Item *j = pool_new(&server->items, Item);
    assert(j);
    j->type = t;
    j->fire_mask = 0;
    j->ammunition = t->initial_ammunition;
    log_debug("+ item %d", j->id.n);
    return j;
}

void item_remove(Item *j) {
    if(j) {
        j->dead = 1;
        log_debug("- item %d", j->id.n);
    }
}

static void fire(Entity *e, Item *j) {
    /* TODO: run timer one last time after fire has been released,
     *       then clear
     */
    if(   j->type->fire
       && j->fire_mask
       && clock_periodic(&j->fire_periodic, j->type->fire_interval))
    {
        j->type->fire(e, j);
    }
}

/* TODO: put in entity */
void items_update(Entity *e) {
    Item *j;
    attached_foreach(e, j)
        fire(e,j);
}

void items_init() {
    pool_static(&server->items, _items, item_ctor, item_dtor);
}

void items_cleanup() {
    pool_free_pred(&server->items, item_check_obsolete);
}
