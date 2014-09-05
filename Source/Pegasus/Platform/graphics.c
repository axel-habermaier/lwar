#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgGraphicsApi pgGetGraphicsApi()
{
#ifdef PG_GRAPHICS_OPENGL3
	return PG_API_OPENGL_3;
#elif defined(PG_GRAPHICS_DIRECT3D11)
	return PG_API_DIRECT3D_11;
#endif
}