#include "prelude.h"

#if defined(WINDOWS) && defined(OPENGL3)

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgBool WglExtSupported(int extension, pgString extensionName);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateContext(pgContext* context)
{
	GLint attributes[] =
	{
		WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
		WGL_CONTEXT_MINOR_VERSION_ARB, 3,
		WGL_CONTEXT_PROFILE_MASK_ARB, WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
#if DEBUG
		WGL_CONTEXT_FLAGS_ARB, WGL_CONTEXT_DEBUG_BIT_ARB | WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
#else
		WGL_CONTEXT_FLAGS_ARB, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB, 
#endif
		0
	};

	PG_ASSERT_NOT_NULL(context->hdc);

	context->hrc = wglCreateContextAttribsARB(context->hdc, NULL, attributes);
	if (context->hrc == NULL)
		pgWin32Error("Failed to initialize the OpenGL context.");
}

pgVoid pgDestroyContext(pgContext* context)
{
	PG_ASSERT_NOT_NULL(context->hrc);

	if (!wglMakeCurrent(NULL, NULL))
		pgWin32Error("Unable to unset the current OpenGL context.");

	if (context->hrc != NULL && !wglDeleteContext(context->hrc))
		pgWin32Error("Unable to destroy the OpenGL context.");

	context->hrc = NULL;
}

pgVoid pgBindContext(pgContext* context, pgGraphicsDevice* device, pgWindow* window)
{
	context->hwnd = window->hwnd;
	context->hrc = device->context.hrc;

	context->hdc = GetDC(context->hwnd);
	if (context->hdc == NULL)
		pgWin32Error("Failed to get the device context of the swap chain window.");

	context->wndStyle = GetWindowLong(context->hwnd, GWL_STYLE);
	context->wndExStyle = GetWindowLong(context->hwnd, GWL_EXSTYLE);
}

pgVoid pgDestroyBoundContext(pgContext* context)
{
	if (context->hdc != NULL && !ReleaseDC(context->hwnd, context->hdc))
		pgWin32Error("Failed to release the device context of the swap chain window.");
}

pgVoid pgCreateContextWindow(pgContext* context)
{
	context->hwnd = CreateWindowEx(0, "Static", "", WS_POPUP | WS_DISABLED, 0, 0, 1, 1,
		NULL, NULL, GetModuleHandle(NULL), NULL);

	if (context->hwnd == NULL)
		pgWin32Error("Failed to initialize the OpenGL initialization window.");

	ShowWindow(context->hwnd, SW_HIDE);

	context->hdc = GetDC(context->hwnd);
	if (context->hdc == NULL)
		pgWin32Error("Failed to get the device context of the OpenGL initialization window.");
}

pgVoid pgDestroyContextWindow(pgContext* context)
{
	if (context->hdc != NULL && !ReleaseDC(context->hwnd, context->hdc))
		pgWin32Error("Failed to release the device context of the OpenGL initialization window.");

	if (context->hwnd != NULL && !DestroyWindow(context->hwnd))
		pgWin32Error("Failed to destroy the OpenGL initialization window.");

	context->hwnd = NULL;
}

pgVoid pgSetPixelFormat(pgContext* context)
{
	PIXELFORMATDESCRIPTOR descriptor;
	int bestMatch;
	BYTE colorBits = 32;
	BYTE alphaBits = 8;
	BYTE depthBits = 24;
	BYTE stencilBits = 8;

	PG_ASSERT_NOT_NULL(context);
	PG_ASSERT_NOT_NULL(context->hdc);

	// Describe the desired pixel format
	memset(&descriptor, 0, sizeof(PIXELFORMATDESCRIPTOR));
	descriptor.nSize = sizeof(PIXELFORMATDESCRIPTOR);
	descriptor.nVersion = 1;
	descriptor.iLayerType = PFD_MAIN_PLANE;
	descriptor.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
	descriptor.iPixelType = PFD_TYPE_RGBA;
	descriptor.cColorBits = colorBits;
	descriptor.cAlphaBits = alphaBits;
	descriptor.cDepthBits = depthBits;
	descriptor.cStencilBits = stencilBits;

	// Get the pixel format that best matches our requirements
	bestMatch = ChoosePixelFormat(context->hdc, &descriptor);
	if (bestMatch == 0)
		pgWin32Error("Failed to find a suitable pixel format.");

	// Check whether the chosen pixel format is acceptable
	if (DescribePixelFormat(context->hdc, bestMatch, sizeof(PIXELFORMATDESCRIPTOR), &descriptor) == 0)
		pgWin32Error("Failed to get pixel format description.");

	if (descriptor.cColorBits != colorBits)
		PG_DIE("Chosen pixel format has a %d bits per color; expected %d bits.", descriptor.cColorBits, colorBits);

	if (descriptor.cAlphaBits != alphaBits)
		PG_DIE("Chosen pixel format has a %d alpha bits; expected %d bits.", descriptor.cAlphaBits, alphaBits);

	if (descriptor.cDepthBits != depthBits)
		PG_DIE("Chosen pixel format has a %d bit depth buffer; expected %d bits.", descriptor.cDepthBits, depthBits);

	if (descriptor.cStencilBits != stencilBits)
		PG_DIE("Chosen pixel format has a %d bit stencil buffer; expected %d bits.", descriptor.cStencilBits, stencilBits);

	// Set the chosen pixel format
	if (!SetPixelFormat(context->hdc, bestMatch, &descriptor))
		pgWin32Error("Failed to set pixel format.");
}

pgBool pgContextFullscreen(pgContext* context, pgInt32 width, pgInt32 height)
{
	DEVMODE devMode;
	memset(&devMode, 0, sizeof(devMode));
	devMode.dmSize = sizeof(devMode);
	devMode.dmPelsWidth = width;
	devMode.dmPelsHeight = height;
	devMode.dmBitsPerPel = 32;
	devMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL;
	
	if (ChangeDisplaySettings(&devMode, CDS_FULLSCREEN) != DISP_CHANGE_SUCCESSFUL)
	{
		PG_ERROR("Failed to switch to fullscreen mode.");
		return PG_FALSE;
	}
	else
	{
		WINDOWPLACEMENT placement;
		memset(&placement, 0, sizeof(placement));
		placement.length = sizeof(placement);

		if (!GetWindowPlacement(context->hwnd, &placement))
			pgWin32Error("Failed to get window configuration.");

		// The context is moving the window, hence it has to store the original window position and size of the un-maximized window
		context->x = placement.rcNormalPosition.left;
		context->y = placement.rcNormalPosition.top;
		context->width = placement.rcNormalPosition.right - context->x;
		context->height = placement.rcNormalPosition.bottom - context->y;
		context->maximized = IsZoomed(context->hwnd);

		// Make the window compatible with fullscreen mode
		SetWindowLong(context->hwnd, GWL_STYLE, WS_POPUP | WS_CLIPCHILDREN | WS_CLIPSIBLINGS);
		SetWindowLong(context->hwnd, GWL_EXSTYLE, WS_EX_APPWINDOW);

		// Resize the window so that it fills the entire screen
		SetWindowPos(context->hwnd, HWND_TOP, 0, 0, width, height, SWP_FRAMECHANGED | SWP_NOCOPYBITS);
		ShowWindow(context->hwnd, SW_SHOW);

		return PG_TRUE;
	}
}

pgVoid pgContextWindowed(pgContext* context)
{
	// Return to the default mode
	ChangeDisplaySettings(NULL, 0);

	// Reset the original window flags and size
	SetWindowLong(context->hwnd, GWL_STYLE, context->wndStyle);
	SetWindowLong(context->hwnd, GWL_EXSTYLE, context->wndExStyle);
	
	// Restore the size
	SetWindowPos(context->hwnd, HWND_TOP, context->x, context->y, context->width, context->height, SWP_FRAMECHANGED | SWP_NOCOPYBITS);

	if (context->maximized)
		ShowWindow(context->hwnd, SW_MAXIMIZE);
	else
		ShowWindow(context->hwnd, SW_SHOW);
}

pgVoid pgInitializeContextExtensions(pgContext* context)
{
	PG_ASSERT_NULL(context->hrc);
	pgBool wglExtsSupported = PG_TRUE;

	// In order to initialize OpenGL and the extensions, we have to create a legacy OpenGL 1.1 context
	context->hrc = wglCreateContext(context->hdc);
	if (context->hrc == NULL)
		pgWin32Error("Failed to create the OpenGL legacy context.");

	if (!wglMakeCurrent(context->hdc, context->hrc))
		pgWin32Error("Failed to make the legacy OpenGL context current.");

	if (wgl_LoadFunctions(context->hdc) == wgl_LOAD_FAILED)
		PG_DIE("WGL initialization failed.");

	wglExtsSupported &= WglExtSupported(wgl_ext_ARB_extensions_string, "WGL_ARB_extensions_string");
	wglExtsSupported &= WglExtSupported(wgl_ext_ARB_create_context, "WGL_ARB_create_context");
	wglExtsSupported &= WglExtSupported(wgl_ext_ARB_create_context_profile, "WGL_ARB_create_context_profile");
	wglExtsSupported &= WglExtSupported(wgl_ext_ARB_pixel_format, "WGL_ARB_pixel_format");

	if (!wglExtsSupported)
		PG_DIE("Not all required WGL extensions are supported.");

	pgDestroyContext(context);
}

pgVoid pgMakeCurrent(pgContext* context)
{
	static pgContext* current = NULL;

	PG_ASSERT_NOT_NULL(context->hdc);
	PG_ASSERT_NOT_NULL(context->hrc);

	if (current == context)
		return;

	current = context;
	if (!wglMakeCurrent(context->hdc, context->hrc))
		pgWin32Error("Failed to make the OpenGL context current.");
}

pgVoid pgSwapBuffers(pgContext* context)
{
	PG_ASSERT_NOT_NULL(context->hdc);

	if (!SwapBuffers(context->hdc))
		pgWin32Error("Failed to swap buffers.");
}

//====================================================================================================================
// Helper functions 
//====================================================================================================================

static pgBool WglExtSupported(int extension, pgString extensionName)
{
	if (extension != wgl_LOAD_SUCCEEDED)
	{
		PG_ERROR("Extension '%s' is not supported.", extensionName);
		return PG_FALSE;
	}

	return PG_TRUE;
}

#endif