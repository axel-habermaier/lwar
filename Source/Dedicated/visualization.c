#include <math.h>
#include <stddef.h>
#include <stdint.h>

#include <GL/gl.h>

#include "server.h"
#include "rules.h"

#include "log.h"
#include "window.h"

static void glCircle() {
    int i;
    glBegin(GL_LINE_LOOP);
    for(i=0; i<360; i+=10) {
        glVertex3f(cos(rad(i)),
                   sin(rad(i)),
                   0);
    }
    glEnd();
}

static void draw_entity(Entity *e) {
    float r = entity_radius(e);
    glPushMatrix();
    glTranslatef(e->x.x, e->x.y, 0);
    glScalef(r,r,1);
    glCircle();
    glPopMatrix();
}

static Entity *launch(Pos x, Pos y, Pos vx, Pos vy, Pos ax, Pos ay, EntityType *t) {
    Vec _x = {x, y};
    Vec _v = {vx,vy};
    Vec _a = {ax,ay};
    Entity *e = entity_create(t,0,_x,_v);
    physics_acc(e,_a);
    return e;
}

int visualization_init() {
    if(!window_open("server viz", 640,480)) return 0;

    glTranslatef(0,0,-10);
    glClearColor(0,0,0,0);

    EntityType *ship   = entity_type_get(ENTITY_TYPE_SHIP);
    EntityType *planet = entity_type_get(ENTITY_TYPE_PLANET);

    Pos k = 0.5;
    Entity *enterprise = launch(-2, 6, k, 0, 0,0,  ship);
    Entity *borg       = launch( 2, 7,-k, 0, 0,0,  ship);
    Entity *pluto      = launch( 0,-2, 0, 0, 0,0,  planet);

    enterprise->active = 1;
    borg->active = 1;
    pluto->active = 1;

    return 1;
}

int visualization_update() {
    int width,height;
    int ev = window_events(&width, &height);

    if(ev & WINDOW_CLOSED)
        return 0;

    if(ev & (WINDOW_EXPOSURE | WINDOW_RESIZED)) {
        window_resized(width, height, 90, 0.5, 100);
    }

    glClear(GL_COLOR_BUFFER_BIT);

    Entity *e;
    entities_foreach(e) {
        draw_entity(e);
    }

    window_swap();

    return 1;
}

void visualization_shutdown() {
    window_close();
}
