#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgGraphicsApi pgGetGraphicsApi()
{
#ifdef OPENGL3
	return PG_API_OPENGL_3;
#elif defined(DIRECT3D11)
	return PG_API_DIRECT3D_11;
#endif
}