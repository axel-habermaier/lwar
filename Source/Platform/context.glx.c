#include "prelude.h"

#if defined(LINUX)

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgBool ctxErrorOccurred;
static int ErrorHandler(Display* display, XErrorEvent* e);
static pgBool GlxExtSupported(int extension, pgString extensionName);
static pgVoid SwitchToWindowedMode(pgContext* context);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateContext(pgContext* context)
{
	GLint attributes[] =
	{
		GLX_CONTEXT_MAJOR_VERSION_ARB, 3,
		GLX_CONTEXT_MINOR_VERSION_ARB, 3,
		GLX_CONTEXT_PROFILE_MASK_ARB, GLX_CONTEXT_CORE_PROFILE_BIT_ARB,
#if DEBUG
		GLX_CONTEXT_FLAGS_ARB, GLX_CONTEXT_DEBUG_BIT_ARB | GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
#else
		GLX_CONTEXT_FLAGS_ARB, GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
#endif
		0
	};
	
	// Prevent X11 from exiting the application if the context creation fails
	ctxErrorOccurred = PG_FALSE;
	int (*oldHandler)(Display*, XErrorEvent*) = XSetErrorHandler(&ErrorHandler);

	context->ctx = glXCreateContextAttribsARB(x11State.display, *context->configs, NULL, PG_TRUE, attributes);
	
	XSync(x11State.display, PG_FALSE);
	if (ctxErrorOccurred || context->ctx == NULL)
		pgDie("Failed to initialize the OpenGL context.");

	XSetErrorHandler(oldHandler);
	
	if (!glXIsDirect(x11State.display, context->ctx))
		pgDie("No direct rendering context could be established.");
}

pgVoid pgDestroyContext(pgContext* context)
{
	XFree(context->visuals);
	XFree(context->configs);

	if (!glXMakeCurrent(x11State.display, 0, NULL))
		pgDie("Unable to unset the current OpenGL context.");
		
	if (context->ctx != NULL)
		glXDestroyContext(x11State.display, context->ctx);
}

pgVoid pgBindContext(pgContext* context, pgGraphicsDevice* device, pgWindow* window)
{
	context->window = window->handle;
	context->ctx = device->context.ctx;
}

pgVoid pgDestroyBoundContext(pgContext* context)
{
	XFree(context->visuals);
	XFree(context->configs);
}

pgVoid pgCreateContextWindow(pgContext* context)
{
	pgInitializeX11();

	context->window = XCreateWindow(x11State.display, RootWindow(x11State.display, x11State.screen), 0, 0, 100, 100, 0,
		DefaultDepth(x11State.display, x11State.screen), InputOutput, DefaultVisual(x11State.display, x11State.screen), 0, NULL);
	
	if (!context->window)
		pgDie("Failed to initialize the OpenGL initialization window.");

	XFlush(x11State.display);
}

pgVoid pgDestroyContextWindow(pgContext* context)
{
	if (context->window)
	{
		if (!XDestroyWindow(x11State.display, context->window))
			pgDie("Failed to destroy the OpenGL initialization window.");
			
		XFlush(x11State.display);
	}
	
	pgShutdownX11();
}

pgVoid pgSetPixelFormat(pgContext* context)
{
	XWindowAttributes windowAttributes;
	if (!XGetWindowAttributes(x11State.display, context->window, &windowAttributes))
		pgDie("Failed to get the window attributes.");

	XVisualInfo visualInfo;
	visualInfo.depth = windowAttributes.depth;
	visualInfo.visualid = XVisualIDFromVisual(windowAttributes.visual);
	visualInfo.screen = x11State.screen;

	int count = 0;
	XVisualInfo* visuals = XGetVisualInfo(x11State.display, VisualDepthMask | VisualIDMask | VisualScreenMask, &visualInfo, &count);
	if (visuals == NULL || count == 0)
	{
		XFree(visuals);
		pgDie("Failed to get any visuals for window.");
	}

	XVisualInfo* bestVisual = NULL;
	int i;
	for (i = 0; i < count; ++i)
	{
		int rgba, red, green, blue, alpha, stencil, depth, doubleBuffered;

		glXGetConfig(x11State.display, &visuals[i], GLX_RGBA, &rgba);
		glXGetConfig(x11State.display, &visuals[i], GLX_RED_SIZE, &red);
		glXGetConfig(x11State.display, &visuals[i], GLX_GREEN_SIZE, &green);
		glXGetConfig(x11State.display, &visuals[i], GLX_BLUE_SIZE, &blue);
		glXGetConfig(x11State.display, &visuals[i], GLX_ALPHA_SIZE, &alpha);
		glXGetConfig(x11State.display, &visuals[i], GLX_DEPTH_SIZE, &depth);
		glXGetConfig(x11State.display, &visuals[i], GLX_STENCIL_SIZE, &stencil);
		glXGetConfig(x11State.display, &visuals[i], GLX_DOUBLEBUFFER, &doubleBuffered);

		if (rgba == 0 || doubleBuffered == 0)
			continue;
		
		if (red == 8 && green == 8 && blue == 8 && /* alpha == 0 && */ depth == 24 && stencil == 8)
		{
			bestVisual = &visuals[i];
			break;
		}
	}

	if (bestVisual == NULL)
		pgDie("Unable to find a matching frame buffer configuration.");

	GLXFBConfig* configs = glXChooseFBConfig(x11State.display, x11State.screen, NULL, &count);
	if (configs == NULL || count == 0)
	{
		XFree(configs);
		pgDie("Failed to get frame buffer configuration.");
	}

	Window root = RootWindow(x11State.display, x11State.screen);
	Colormap colorMap = XCreateColormap(x11State.display, root, bestVisual->visual, AllocNone);
	XSetWindowColormap(x11State.display, context->window, colorMap);

	context->visuals = visuals;
	context->configs = configs;
}

pgBool pgUpdateContextState(pgContext* context, pgInt32 width, pgInt32 height, pgBool fullscreen)
{
	if (fullscreen)
	{
		// Check for the XRandR extension
		int version;
		if (!XQueryExtension(x11State.display, "RANDR", &version, &version, &version))
		{
			pgError("XRandR extension not found. Fullscreen mode is not supported on this system.");
			return PG_FALSE;
		}

		// Get the current screen configuration from XRandR
		XRRScreenConfiguration* config = XRRGetScreenInfo(x11State.display, RootWindow(x11State.display, x11State.screen));
		if (!config)
		{
			pgError("Failed to get the current screen configuration. Fullscreen mode is not supported on this system.");
			return PG_FALSE;
		}

		// Select a video mode
		Rotation currentRotation;
		pgInt32 numModes;
		pgBool modeFound = PG_FALSE;

		context->prevMode = XRRConfigCurrentConfiguration(config, &currentRotation);
		XRRScreenSize* modes = XRRConfigSizes(config, &numModes);

		if (modes)
		{
			// Check all modes until a matching one is found
			int i;
			for (i = 0; i < numModes; ++i)
			{
				if (modes[i].width == width && modes[i].height == height)
				{
					// Switch to fullscreen mode
					XRRSetScreenConfig(x11State.display, config, RootWindow(x11State.display, x11State.screen), i, currentRotation, CurrentTime);

					modeFound = PG_TRUE;
					break;
				}
			}
		}

		// Free the configuration instance
		XRRFreeScreenConfigInfo(config);

		if (!modeFound)
		{
			pgError("No fullscreen mode of the desired size has been found.");
			return PG_FALSE;
		}

		// Grab the mouse and the keyboard
		XGrabPointer(x11State.display, context->window, PG_TRUE, 0, GrabModeAsync, GrabModeAsync, context->window, None, CurrentTime);
		XGrabKeyboard(x11State.display, context->window, PG_TRUE, GrabModeAsync, GrabModeAsync, CurrentTime);

		context->fullscreen = PG_TRUE;
		return PG_TRUE;
	}

	SwitchToWindowedMode(context);
	return PG_TRUE;
}

pgVoid pgInitializeContextExtensions(pgContext* context)
{
	pgBool glxExtsSupported = PG_TRUE;
	int major, minor;
 
	if (!glXQueryVersion(x11State.display, &major, &minor))
		pgDie("Failed to retrieve the GLX version.");
	
	if ((major == 1 && minor < 3) ||  major < 1)
		pgDie("GLX version 1.3 is required but found version %d.%d only.", major, minor);

	if (glx_LoadFunctions(x11State.display, x11State.screen) == glx_LOAD_FAILED)
		pgDie("GLX initialization failed.");

	glxExtsSupported &= GlxExtSupported(glx_ext_ARB_create_context_profile, "GLX_ARB_create_context_profile");
	glxExtsSupported &= GlxExtSupported(glx_ext_ARB_create_context, "GLX_ARB_create_context");

	if (!glxExtsSupported)
		pgDie("Not all required GLX extensions are supported.");
}

pgVoid pgMakeCurrent(pgContext* context)
{
	static pgContext* current = NULL;
	if (current == context)
		return;

	current = context;
	if (!glXMakeCurrent(x11State.display, context->window, context->ctx))
		pgDie("Failed to make OpenGL context current.");
}

pgVoid pgSwapBuffers(pgContext* context)
{
	glXSwapBuffers(x11State.display, context->window);
}

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static int ErrorHandler(Display* display, XErrorEvent* e)
{
	PG_UNUSED(display);
	PG_UNUSED(e);

    ctxErrorOccurred = PG_TRUE;
    return 0;
}

static pgBool GlxExtSupported(int extension, pgString extensionName)
{
	if (extension != glx_LOAD_SUCCEEDED)
	{
		pgError("Extension '%s' is not supported.", extensionName);
		return PG_FALSE;
	}

	return PG_TRUE;
}

static pgVoid SwitchToWindowedMode(pgContext* context)
{
	if (!context->fullscreen)
		return;

	// Release the mouse and the keyboard
	XUngrabPointer(x11State.display, CurrentTime);
	XUngrabKeyboard(x11State.display, CurrentTime);

	// Restore the original video mode
	XRRScreenConfiguration* config = XRRGetScreenInfo(x11State.display, RootWindow(x11State.display, x11State.screen));
	if (config) 
	{
		Rotation currentRotation;
		XRRConfigCurrentConfiguration(config, &currentRotation);

		XRRSetScreenConfig(x11State.display, config, RootWindow(x11State.display, x11State.screen), context->prevMode, currentRotation, CurrentTime);
		XRRFreeScreenConfigInfo(config);
	} 

	context->fullscreen = PG_FALSE;
}

#endif
