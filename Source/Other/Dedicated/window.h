enum {
    WINDOW_CLOSED   = 0x01,
    WINDOW_EXPOSURE = 0x02,
    WINDOW_RESIZED  = 0x04,
};

/* update opengl perspective */
void window_resized(int width, int height,
                    float fov, float near, float far);

/* return 1 on success */
int  window_open(const char *title, int width, int height);

/* return window flags */
unsigned int window_events(int *width, int *height);

void window_swap();
void window_close();

int key_down(unsigned char key);
int button_down(unsigned char button);
void mouse_pos(int *x, int *y);
