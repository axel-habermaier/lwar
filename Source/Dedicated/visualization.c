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
    float r = e->type->radius;
    glPushMatrix();
    glTranslatef(e->x.x, e->x.y, 0);
    glScalef(r,r,1);
    glCircle();
    glPopMatrix();
}

int visualization_init() {
    if(!window_open("server viz", 640,480)) return 0;
    glTranslatef(0,0,-10);
    glClearColor(0,0,0,0);
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
