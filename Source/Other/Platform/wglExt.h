#ifndef POINTER_C_GENERATED_HEADER_WINDOWSGL_H
#define POINTER_C_GENERATED_HEADER_WINDOWSGL_H

#ifdef __wglext_h_
#error Attempt to include auto-generated WGL header after wglext.h
#endif

#define __wglext_h_

#ifndef WIN32_LEAN_AND_MEAN
	#define WIN32_LEAN_AND_MEAN 1
#endif
#ifndef NOMINMAX
	#define NOMINMAX
#endif
#include <windows.h>

#ifdef CODEGEN_FUNCPTR
#undef CODEGEN_FUNCPTR
#endif /*CODEGEN_FUNCPTR*/
#define CODEGEN_FUNCPTR WINAPI


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


#ifndef WGL_ARB_pbuffer
	DECLARE_HANDLE(HPBUFFERARB);
#endif
#ifndef WGL_EXT_pbuffer
	DECLARE_HANDLE(HPBUFFEREXT);
#endif
#ifndef WGL_NV_present_video
	DECLARE_HANDLE(HVIDEOOUTPUTDEVICENV);
#endif
#ifndef WGL_NV_video_output
	DECLARE_HANDLE(HPVIDEODEV);
#endif
#ifndef WGL_NV_gpu_affinity
	DECLARE_HANDLE(HPGPUNV);
	DECLARE_HANDLE(HGPUNV);
	typedef struct _GPU_DEVICE {
	    DWORD  cb;
	    CHAR   DeviceName[32];
	    CHAR   DeviceString[128];
	    DWORD  Flags;
	    RECT   rcVirtualScreen;
	} GPU_DEVICE, *PGPU_DEVICE;
#endif
#ifndef WGL_NV_video_capture
	DECLARE_HANDLE(HVIDEOINPUTDEVICENV);
#endif

#ifdef __cplusplus
extern "C" {
#endif /*__cplusplus*/

extern int wgl_ext_ARB_extensions_string;
extern int wgl_ext_ARB_create_context_profile;
extern int wgl_ext_ARB_pixel_format;
extern int wgl_ext_ARB_create_context;

#define WGL_CONTEXT_PROFILE_MASK_ARB 0x9126
#define WGL_CONTEXT_CORE_PROFILE_BIT_ARB 0x00000001
#define WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB 0x00000002
#define WGL_ERROR_INVALID_PROFILE_ARB 0x2096

#define WGL_NUMBER_PIXEL_FORMATS_ARB 0x2000
#define WGL_DRAW_TO_WINDOW_ARB 0x2001
#define WGL_DRAW_TO_BITMAP_ARB 0x2002
#define WGL_ACCELERATION_ARB 0x2003
#define WGL_NEED_PALETTE_ARB 0x2004
#define WGL_NEED_SYSTEM_PALETTE_ARB 0x2005
#define WGL_SWAP_LAYER_BUFFERS_ARB 0x2006
#define WGL_SWAP_METHOD_ARB 0x2007
#define WGL_NUMBER_OVERLAYS_ARB 0x2008
#define WGL_NUMBER_UNDERLAYS_ARB 0x2009
#define WGL_TRANSPARENT_ARB 0x200A
#define WGL_TRANSPARENT_RED_VALUE_ARB 0x2037
#define WGL_TRANSPARENT_GREEN_VALUE_ARB 0x2038
#define WGL_TRANSPARENT_BLUE_VALUE_ARB 0x2039
#define WGL_TRANSPARENT_ALPHA_VALUE_ARB 0x203A
#define WGL_TRANSPARENT_INDEX_VALUE_ARB 0x203B
#define WGL_SHARE_DEPTH_ARB 0x200C
#define WGL_SHARE_STENCIL_ARB 0x200D
#define WGL_SHARE_ACCUM_ARB 0x200E
#define WGL_SUPPORT_GDI_ARB 0x200F
#define WGL_SUPPORT_OPENGL_ARB 0x2010
#define WGL_DOUBLE_BUFFER_ARB 0x2011
#define WGL_STEREO_ARB 0x2012
#define WGL_PIXEL_TYPE_ARB 0x2013
#define WGL_COLOR_BITS_ARB 0x2014
#define WGL_RED_BITS_ARB 0x2015
#define WGL_RED_SHIFT_ARB 0x2016
#define WGL_GREEN_BITS_ARB 0x2017
#define WGL_GREEN_SHIFT_ARB 0x2018
#define WGL_BLUE_BITS_ARB 0x2019
#define WGL_BLUE_SHIFT_ARB 0x201A
#define WGL_ALPHA_BITS_ARB 0x201B
#define WGL_ALPHA_SHIFT_ARB 0x201C
#define WGL_ACCUM_BITS_ARB 0x201D
#define WGL_ACCUM_RED_BITS_ARB 0x201E
#define WGL_ACCUM_GREEN_BITS_ARB 0x201F
#define WGL_ACCUM_BLUE_BITS_ARB 0x2020
#define WGL_ACCUM_ALPHA_BITS_ARB 0x2021
#define WGL_DEPTH_BITS_ARB 0x2022
#define WGL_STENCIL_BITS_ARB 0x2023
#define WGL_AUX_BUFFERS_ARB 0x2024
#define WGL_NO_ACCELERATION_ARB 0x2025
#define WGL_GENERIC_ACCELERATION_ARB 0x2026
#define WGL_FULL_ACCELERATION_ARB 0x2027
#define WGL_SWAP_EXCHANGE_ARB 0x2028
#define WGL_SWAP_COPY_ARB 0x2029
#define WGL_SWAP_UNDEFINED_ARB 0x202A
#define WGL_TYPE_RGBA_ARB 0x202B
#define WGL_TYPE_COLORINDEX_ARB 0x202C

#define WGL_CONTEXT_DEBUG_BIT_ARB 0x00000001
#define WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB 0x00000002
#define WGL_CONTEXT_MAJOR_VERSION_ARB 0x2091
#define WGL_CONTEXT_MINOR_VERSION_ARB 0x2092
#define WGL_CONTEXT_LAYER_PLANE_ARB 0x2093
#define WGL_CONTEXT_FLAGS_ARB 0x2094
#define WGL_ERROR_INVALID_VERSION_ARB 0x2095

#ifndef WGL_ARB_extensions_string
#define WGL_ARB_extensions_string 1
extern const char * (CODEGEN_FUNCPTR *_ptrc_wglGetExtensionsStringARB)(HDC );
#define wglGetExtensionsStringARB _ptrc_wglGetExtensionsStringARB
#endif /*WGL_ARB_extensions_string*/ 


#ifndef WGL_ARB_pixel_format
#define WGL_ARB_pixel_format 1
extern BOOL (CODEGEN_FUNCPTR *_ptrc_wglGetPixelFormatAttribivARB)(HDC , int , int , UINT , const int *, int *);
#define wglGetPixelFormatAttribivARB _ptrc_wglGetPixelFormatAttribivARB
extern BOOL (CODEGEN_FUNCPTR *_ptrc_wglGetPixelFormatAttribfvARB)(HDC , int , int , UINT , const int *, FLOAT *);
#define wglGetPixelFormatAttribfvARB _ptrc_wglGetPixelFormatAttribfvARB
extern BOOL (CODEGEN_FUNCPTR *_ptrc_wglChoosePixelFormatARB)(HDC , const int *, const FLOAT *, UINT , int *, UINT *);
#define wglChoosePixelFormatARB _ptrc_wglChoosePixelFormatARB
#endif /*WGL_ARB_pixel_format*/ 

#ifndef WGL_ARB_create_context
#define WGL_ARB_create_context 1
extern HGLRC (CODEGEN_FUNCPTR *_ptrc_wglCreateContextAttribsARB)(HDC , HGLRC , const int *);
#define wglCreateContextAttribsARB _ptrc_wglCreateContextAttribsARB
#endif /*WGL_ARB_create_context*/ 

enum wgl_LoadStatus
{
	wgl_LOAD_FAILED = 0,
	wgl_LOAD_SUCCEEDED = 1,
};

int wgl_LoadFunctions(HDC hdc);


#ifdef __cplusplus
}
#endif /*__cplusplus*/

#endif //POINTER_C_GENERATED_HEADER_WINDOWSGL_H