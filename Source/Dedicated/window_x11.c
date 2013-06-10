#include <X11/Xlib.h>
#include <X11/Xatom.h>

#include <GL/gl.h>
#include <GL/glx.h>

#include <string.h>

#include "window.h"

static int attr_list[] = {
    GLX_RGBA, GLX_DOUBLEBUFFER,
    GLX_RED_SIZE,    4,
    GLX_GREEN_SIZE,  4,
    GLX_BLUE_SIZE,   4,
    GLX_DEPTH_SIZE, 16,
    None
};

static Display   *display;
static Window     window;
static GLXContext context;

/* mouse position, button and key status */
static int mx,my;
static int buttons[4];
static int keys[256];

/* TODO: more error handling */
int window_open(const char *title, int width, int height)
{
    if(display) return 0;

    display = XOpenDisplay(0);
    if(!display) return 0;

    int screen = XDefaultScreen(display);

    XVisualInfo *vi = glXChooseVisual(display, screen, attr_list);
    if(!vi) return 0;

    context = glXCreateContext(display, vi, 0, GL_TRUE);

    Window root_window = RootWindow(display, vi->screen);

    XSetWindowAttributes win_attr;
    win_attr.colormap  = XCreateColormap(display, root_window,
                                         vi->visual, AllocNone);
    win_attr.border_pixel = 0;
    win_attr.event_mask =   ExposureMask
                          | KeyPressMask    | KeyReleaseMask
                          | ButtonPressMask | ButtonReleaseMask
                          | PointerMotionMask
                          | StructureNotifyMask;

    window = XCreateWindow(display, root_window,
                           0, 0, width, height, 0, vi->depth,
                           InputOutput, vi->visual,
                           CWBorderPixel|CWEventMask|CWColormap,
                           &win_attr);
    Atom wm_delete = XInternAtom(display, "WM_DELETE_WINDOW", True); 
    XSetWMProtocols(display, window, &wm_delete, 1);
    XSetStandardProperties(display, window, title, title, None, 0,0,0);

    XMapRaised(display, window);

    glXMakeCurrent(display, window, context);
    return 1;
}

void window_close() {
    glXMakeCurrent(display, None, 0);
    glXDestroyContext(display, context);
    XCloseDisplay(display);
    display = 0;
}

void window_swap() {
    glXSwapBuffers(display, window);
}

unsigned int window_events(int *width, int *height) {
    XEvent event;
    unsigned int flags = 0;
    char kstr[1];
    int  kn;

    while(XPending(display)>0)
    {
        int pressed = 0;
        XNextEvent(display, &event);

        switch(event.type)
        {
        case Expose:
            flags |= WINDOW_EXPOSURE;
            break;

        case ConfigureNotify:
            if(width)  *width  = event.xconfigure.width;
            if(height) *height = event.xconfigure.height;
            flags |= WINDOW_RESIZED;
            break;

        case KeyPress:
            pressed = 1;
            /* fall through */
        case KeyRelease:
            kn = XLookupString(&event.xkey, kstr, 1, 0,0);
            if(kn == 1)
                keys[(int)kstr[0]] = pressed;
            break;

        case ButtonPress:
            pressed = 1;
            /* fall through */
        case ButtonRelease:
            if(event.xbutton.button < 4)
                buttons[event.xbutton.button] = pressed;
            break;

        case MotionNotify:
            mx = event.xmotion.x;
            my = event.xmotion.y;
            break;

        case ClientMessage:
            if(!strcmp(XGetAtomName(display, event.xclient.message_type), 
                       "WM_PROTOCOLS"))
                flags |= WINDOW_CLOSED;
        }
    }

    return flags;
}

int key_down(unsigned char key) {
    return keys[key];
}

int button_down(unsigned char button) {
    if(button < 4)
        return buttons[button];
    return 0;
}

void mouse_pos(int *x, int *y) {
    if(x) *x = mx;
    if(y) *y = my;
}
