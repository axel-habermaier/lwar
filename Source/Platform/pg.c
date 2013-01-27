#include "prelude.h"
#include <assert.h>
#include <stdarg.h>
#include <stdio.h>

LibraryState libraryState;

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgVoid pgInitialize(pgLogCallbacks* callbacks)
{
	// We have to use the default C assert() here, as we are about to initialize the infrastructure for the 
	// PG_ASSERT macros
	assert(callbacks != NULL);
	assert(callbacks->die != NULL);
	assert(callbacks->error != NULL);
	assert(callbacks->warning != NULL);
	assert(callbacks->info != NULL);
	assert(callbacks->debug != NULL);
	assert(libraryState.initialized == PG_FALSE);

	libraryState.initialized = PG_TRUE;
	libraryState.logCallbacks = *callbacks;
}

pgVoid pgShutdown()
{
	PG_ASSERT(libraryState.initialized == PG_TRUE, "Library is not initialized.");
	libraryState.initialized = PG_FALSE;
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

static pgString pgFormat(pgString message, va_list vl)
{
	static pgChar buffer[4096];
	PG_ASSERT_NOT_NULL(message);

	if (vsnprintf((char*)buffer, sizeof(buffer), (char*)message, vl) < 0)
		pgDie("Error while generating log message.");
	else
		return buffer;
}

PG_NORETURN pgVoid pgDie(pgString message, ...)
{
	va_list vl;
	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);
	libraryState.logCallbacks.die(pgFormat(message, vl));
	va_end(vl);
}

pgVoid pgError(pgString message, ...)
{
	va_list vl;
	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);
	libraryState.logCallbacks.error(pgFormat(message, vl));
	va_end(vl);
}

pgVoid pgWarn(pgString message, ...)
{
	va_list vl;
	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);
	libraryState.logCallbacks.warning(pgFormat(message, vl));
	va_end(vl);
}

pgVoid pgInfo(pgString message, ...)
{
	va_list vl;
	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);
	libraryState.logCallbacks.info(pgFormat(message, vl));
	va_end(vl);
}

pgVoid pgDebugInfo(pgString message, ...)
{
	va_list vl;
	PG_ASSERT_NOT_NULL(message);

	va_start(vl, message);
	libraryState.logCallbacks.debug(pgFormat(message, vl));
	va_end(vl);
}