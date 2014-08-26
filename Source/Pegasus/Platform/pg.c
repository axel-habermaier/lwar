#include "prelude.h"
#include <assert.h>
#include <stdarg.h>
#include <stdio.h>

pgLibraryState pgState;

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgVoid pgInitialize(pgLogCallback callback, pgString appName)
{
	size_t length = strlen(appName);

	// We have to use the default C assert() here, as we are about to initialize the infrastructure for the 
	// PG_ASSERT macros
	assert(callback != NULL);
	assert(appName != NULL);
	assert(pgState.initialized == PG_FALSE);

	pgState.initialized = PG_TRUE;
	pgState.logCallback = callback;

	PG_ALLOC_ARRAY(pgChar, length + 1, pgState.appName);
	memcpy(pgState.appName, appName, length + 1);

	pgInitializeCore();
}

pgVoid pgShutdown()
{
	PG_ASSERT(pgState.initialized == PG_TRUE, "Library is not initialized.");
	pgState.initialized = PG_FALSE;

	PG_FREE(pgState.appName);
	pgState.appName = NULL;

	pgShutdownCore();
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgString pgGetOsErrorMessage()
{
	size_t length, i;
	pgChar* osMessage;

#ifdef WINDOWS
	osMessage = pgGetWin32ErrorMessage(GetLastError());
#else
	static pgChar buffer[2048];
	strerror_r(errno, buffer, sizeof(buffer) / sizeof(pgChar));

	osMessage = buffer;
#endif

	length = strlen(osMessage);
	for (i = length; i > 0; --i)
	{
		if (osMessage[i - 1] == '\r' || osMessage[i - 1] == '\n')
			osMessage[i - 1] = '\0';
		else
			break;
	}

	return osMessage;
}

pgString pgFormat(pgString message, ...)
{
	static char buffer[4096];
	va_list vl;

	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);

	if (vsnprintf((char*)buffer, sizeof(buffer), (char*)message, vl) < 0)
		PG_DIE("Error while generating log message.");
	
	va_end(vl);
	return buffer;
}

pgVoid pgNoReturn()
{
	// A dummy non-returning function that is needed by the PG_DIE macro. Visual studio does not try to verify
	// whether the function actually returns, whereas GCC and clang do.

#ifndef WINDOWS
	for(;;) {}
#endif
}

pgBool pgRectangleEqual(pgRectangle* r1, pgRectangle* r2)
{
	return r1->left == r2->left && r1->top == r2->top && r1->width == r2->width && r1->height == r2->height;
}

pgInt32 pgClamp(pgInt32 value, pgInt32 min, pgInt32 max)
{
	if (max < min)
		return value;

	if (value < min)
		return min;

	if (value > max)
		return max;

	return value;
}