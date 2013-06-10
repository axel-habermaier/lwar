#include <math.h>

#include <GL/gl.h>

#include "window.h"


/* borrowed from gluPerspective */
static void window_perspective(GLfloat fovy, GLfloat aspect,
                               GLfloat zmin, GLfloat zmax)
{
    GLfloat xmin, xmax, ymin, ymax;
    ymax = zmin * tan(fovy * M_PI / 360.0);
    ymin = -ymax;
    xmin = ymin * aspect;
    xmax = ymax * aspect;
    glFrustum(xmin, xmax, ymin, ymax, zmin, zmax);
}

void window_resized(int width, int height,
                    float fov, float near, float far)
{
    /* prevent division by zero */
    if(height == 0) height = 1;

    glViewport(0,0,width,height);

    /* setup perspective view */
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    window_perspective(fov, ((GLfloat)width)/height, near, far);

    glMatrixMode(GL_MODELVIEW);
}
