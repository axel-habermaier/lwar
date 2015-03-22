#ifndef POINTER_C_GENERATED_HEADER_GLXWIN_H
#define POINTER_C_GENERATED_HEADER_GLXWIN_H

#ifdef __glxext_h_
#error Attempt to include glx_exts after including glxext.h
#endif

#define __glxext_h_

#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <GL/glx.h>
#ifdef CODEGEN_FUNCPTR
#undef CODEGEN_FUNCPTR
#endif /*CODEGEN_FUNCPTR*/
#define CODEGEN_FUNCPTR


#ifndef GL_LOAD_GEN_BASIC_OPENGL_TYPEDEFS
#define GL_LOAD_GEN_BASIC_OPENGL_TYPEDEFS

	typedef unsigned int GLenum;
	typedef unsigned char GLboolean;
	typedef unsigned int GLbitfield;
	typedef signed char GLbyte;
	typedef short GLshort;
	typedef int GLint;
	typedef int GLsizei;
	typedef unsigned char GLubyte;
	typedef unsigned short GLushort;
	typedef unsigned int GLuint;
	typedef float GLfloat;
	typedef float GLclampf;
	typedef double GLdouble;
	typedef double GLclampd;
	#define GLvoid void

#endif /*GL_LOAD_GEN_BASIC_OPENGL_TYPEDEFS*/


#ifndef GLX_ARB_get_proc_address
	typedef void (*__GLXextFuncPtr)(void);
#endif
#ifndef GLX_SGIX_video_source
	typedef XID GLXVideoSourceSGIX;
#endif
#ifndef GLX_SGIX_fbconfig
	typedef XID GLXFBConfigIDSGIX;
	typedef struct __GLXFBConfigRec *GLXFBConfigSGIX;
#endif
#ifndef GLX_SGIX_pbuffer
	typedef XID GLXPbufferSGIX;
	typedef struct {
	    int type;
	    unsigned long serial;	  /* # of last request processed by server */
	    Bool send_event;		  /* true if this came for SendEvent request */
	    Display *display;		  /* display the event was read from */
	    GLXDrawable drawable;	  /* i.d. of Drawable */
	    int event_type;		  /* GLX_DAMAGED_SGIX or GLX_SAVED_SGIX */
	    int draw_type;		  /* GLX_WINDOW_SGIX or GLX_PBUFFER_SGIX */
	    unsigned int mask;	  /* mask indicating which buffers are affected*/
	    int x, y;
	    int width, height;
	    int count;		  /* if nonzero, at least this many more */
	} GLXBufferClobberEventSGIX;
#endif
#ifndef GLX_NV_video_output
	typedef unsigned int GLXVideoDeviceNV;
#endif
#ifndef GLX_NV_video_capture
	typedef XID GLXVideoCaptureDeviceNV;
#endif
#ifndef GLEXT_64_TYPES_DEFINED
	/* This code block is duplicated in glext.h, so must be protected */
	#define GLEXT_64_TYPES_DEFINED
	/* Define int32_t, int64_t, and uint64_t types for UST/MSC */
	/* (as used in the GLX_OML_sync_control extension). */
	#if defined(__STDC_VERSION__) && __STDC_VERSION__ >= 199901L
		#include <inttypes.h>
	#elif defined(__sun__) || defined(__digital__)
		#include <inttypes.h>
		#if defined(__STDC__)
			#if defined(__arch64__) || defined(_LP64)
				typedef long int int64_t;
				typedef unsigned long int uint64_t;
				#else
				typedef long long int int64_t;
				typedef unsigned long long int uint64_t;
			#endif /* __arch64__ */
		#endif /* __STDC__ */
	#elif defined( __VMS ) || defined(__sgi)
		#include <inttypes.h>
	#elif defined(__SCO__) || defined(__USLC__)
		#include <stdint.h>
	#elif defined(__UNIXOS2__) || defined(__SOL64__)
		typedef long int int32_t;
		typedef long long int int64_t;
		typedef unsigned long long int uint64_t;
	#elif defined(_WIN32) && defined(__GNUC__)
		#include <stdint.h>
	#elif defined(_WIN32)
		typedef __int32 int32_t;
		typedef __int64 int64_t;
		typedef unsigned __int64 uint64_t;
		#else
		#include <inttypes.h>     /* Fallback option */
	#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif /*__cplusplus*/

extern int glx_ext_ARB_create_context_profile;
extern int glx_ext_ARB_create_context;

#define GLX_CONTEXT_CORE_PROFILE_BIT_ARB 0x00000001
#define GLX_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB 0x00000002
#define GLX_CONTEXT_PROFILE_MASK_ARB 0x9126

#define GLX_CONTEXT_DEBUG_BIT_ARB 0x00000001
#define GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB 0x00000002
#define GLX_CONTEXT_MAJOR_VERSION_ARB 0x2091
#define GLX_CONTEXT_MINOR_VERSION_ARB 0x2092
#define GLX_CONTEXT_FLAGS_ARB 0x2094


#ifndef GLX_ARB_create_context
#define GLX_ARB_create_context 1
extern GLXContext (CODEGEN_FUNCPTR *_ptrc_glXCreateContextAttribsARB)(Display *, GLXFBConfig , GLXContext , Bool , const int *);
#define glXCreateContextAttribsARB _ptrc_glXCreateContextAttribsARB
#endif /*GLX_ARB_create_context*/ 

enum glx_LoadStatus
{
	glx_LOAD_FAILED = 0,
	glx_LOAD_SUCCEEDED = 1,
};

int glx_LoadFunctions(Display *display, int screen);


#ifdef __cplusplus
}
#endif /*__cplusplus*/

#endif //POINTER_C_GENERATED_HEADER_GLXWIN_H
