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

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgBool pgRectangleEqual(pgRectangle* r1, pgRectangle* r2)
{
	return r1->left == r2->left && r1->top == r2->top && r1->width == r2->width && r1->height == r2->height;
}