#include <math.h>
#include <stddef.h>
#include <stdint.h>


#include <GL/gl.h>

#include "server.h"
#include "rules.h"

#include "log.h"
#include "window.h"

static Vec  camera;
static Real camera_z;
static Real scale = 0.05;
static int width,height;

static void glRect(float w, float h, float phi) {
    glRotatef(phi,0,0,1);
    // glScalef(w,h,1);
    glScalef(w,h,1);
    glBegin(GL_POLYGON);
    glVertex3f( 1,-1, 0);
    glVertex3f( 0,-1, 0);
    glVertex3f( 0, 1, 0);
    glVertex3f( 1, 1, 0);
    glEnd();
}

static void glTriangle(float r, float phi) {
    glRotatef(phi,0,0,1);
    glScalef(r,r,1);
    glBegin(GL_POLYGON);
    glVertex3f(cos(rad(   0)), sin(rad(   0)), 0);
    glVertex3f(cos(rad( 130)), sin(rad( 130)), 0);
    glVertex3f(cos(rad(-130)), sin(rad(-130)), 0);
    glEnd();
}

static void glCircle(float r) {
    glScalef(r,r,1);
    int i;
    glBegin(GL_POLYGON);
    for(i=0; i<360; i+=10) {
        glVertex3f(cos(rad(i)),
                   sin(rad(i)),
                   0);
    }
    glEnd();
}

static void draw_entity(Entity *e) {
    glPushMatrix();
    glTranslatef(e->x.x, e->x.y, 0);
    float r   = e->radius;
    float phi = deg(e->phi);

    switch(e->type->id) {
    case ENTITY_TYPE_SHIP:
        glColor4f(0,1,0,0);
        glTriangle(r,phi);
        break;
    case ENTITY_TYPE_ROCKET:
        glColor4f(1,.5,0,0);
        glTriangle(r,phi);
        break;
    case ENTITY_TYPE_PLANET:
        glColor4f(0,.5,1,0);
        glCircle(r);
        break;
    case ENTITY_TYPE_BULLET:
        glColor4f(.5,.5,.5,0);
        glCircle(r);
        break;
    case ENTITY_TYPE_RAY:
        glColor4f(1,0,0,0);
        glRect(e->len,5,phi);
    }

    glPopMatrix();
}

static void draw() {
    glPushMatrix();
    glScalef(scale, scale, 1);
    glTranslatef(-camera.x, -camera.y, -camera_z);

    Entity *e;
    entities_foreach(e) {
        draw_entity(e);
    }
    glPopMatrix();
}

static void control() {
    if(!server->self) return;

    Player *p = &server->self->player;

    if(key_down('C'))
        server->running = 0;

    if(key_down('e')) {
        player_select(p, ENTITY_TYPE_SHIP, ENTITY_TYPE_GUN, ENTITY_TYPE_PHASER, 0, 0);
    } else if(key_down('E')) {
        player_select(p, 0, 0, 0, 0, 0);
        player_die(p);
    }

    if(key_down('+'))
        scale *= 1.03;

    if(key_down('-'))
        scale /= 1.03;

    int mx,my;
    int ax,ay;
    mouse_pos(&mx,&my);

    Entity *e = p->ship.entity;
    if(!e) return;

    ax = e->x.x + mx - width/2;
    ay = e->x.y - my + height/2;

    player_input(p, key_down('w'),
                    key_down('s'),
                    key_down('d'),
                    key_down('a'),
                    0,
                    0,
                    button_down(1),
                    button_down(3),
                    0,0,
                    ax,ay);

    camera = e->x;
    /* log_debug("camera (%.2f,%.2f)", camera.x, camera.y); */
}

int visualization_init() {
    if(!window_open("server viz", 640,480)) return 0;
    glClearColor(0,0,0,0);
    glColor4f(1,1,1,1);
    camera_z = 10;
    return 1;
}

int visualization_update() {
    int ev = window_events(&width, &height);

    if(ev & WINDOW_CLOSED)
        return 0;

    if(ev & (WINDOW_EXPOSURE | WINDOW_RESIZED)) {
        window_resized(width, height, 90, 0.5, 100);
    }

    glClear(GL_COLOR_BUFFER_BIT);
    control();
    draw();

    window_swap();

    return 1;
}

void visualization_shutdown() {
    window_close();
}
