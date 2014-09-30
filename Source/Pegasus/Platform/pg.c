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

#ifdef DEBUG
	pgReportAllocatedMemory();
#endif
}

pgVoid pgMemCopy(pgVoid* destination, const pgVoid* source, pgInt32 byteCount)
{
	PG_ASSERT_NOT_NULL(destination);
	PG_ASSERT_NOT_NULL(source);
	PG_ASSERT(byteCount > 0, "At least 1 byte must be copied.");
	PG_ASSERT((source < destination && ((const pgByte*)source) + byteCount <= (pgByte*)destination) ||
			  (destination < source && ((pgByte*)destination) + byteCount <= (pgByte*)source),
			  "The memory regions overlap.");

	memcpy(destination, source, (size_t)byteCount);
}

//====================================================================================================================
// Internal functions
//====================================================================================================================

pgString pgGetOsErrorMessage()
{
	pgChar* osMessage;

#ifdef PG_SYSTEM_WINDOWS
	osMessage = pgGetWin32ErrorMessage(GetLastError());
#else
	static pgChar buffer[2048];
	strerror_r(errno, buffer, sizeof(buffer) / sizeof(pgChar));

	osMessage = buffer;
#endif

	return pgTrim(osMessage);
}

pgChar* pgTrim(pgChar* message)
{
	size_t length = strlen(message);

	for (size_t i = length; i > 0; --i)
	{
		if (message[i - 1] == '\r' || message[i - 1] == '\n')
			message[i - 1] = '\0';
		else
			break;
	}

	return message;
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

#ifndef PG_SYSTEM_WINDOWS
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

//====================================================================================================================
// Memory debugging
//====================================================================================================================

#ifdef DEBUG

typedef struct pgMemoryInfo
{
	pgVoid* ptr;
	pgString type;
	pgString file;
	pgInt32 line;
	struct pgMemoryInfo* next;
	struct pgMemoryInfo* prev;
} pgMemoryInfo;

static pgMemoryInfo* pgMemory = NULL;

static pgMemoryInfo* pgFindMemoryInfo(pgVoid* ptr)
{
	for (pgMemoryInfo* info = pgMemory; info != NULL; info = info->next)
		if (info->ptr == ptr)
			return info;

	return NULL;
}

pgVoid pgAllocated(pgVoid* ptr, pgString type, pgString file, pgInt32 line)
{
	pgMemoryInfo* info = pgFindMemoryInfo(ptr);
	if (info != NULL)
		PG_DIE("Memory is reallocated without being freed at %s:%d. The original allocation of type '%s' occurred at %s:%d.",
		file, line, info->type, info->file, info->line);

	info = (pgMemoryInfo*)malloc(sizeof(pgMemoryInfo));
	info->ptr = ptr;
	info->type = type;
	info->file = file;
	info->line = line;
	info->next = pgMemory;
	info->prev = NULL;

	if (pgMemory != NULL)
		pgMemory->prev = info;

	pgMemory = info;
}

pgVoid pgDeallocated(pgVoid* ptr)
{
	if (ptr == NULL)
		return;

	pgMemoryInfo* info = pgFindMemoryInfo(ptr);
	if (info != NULL)
	{
		if (info == pgMemory)
		{
			info->prev = NULL;
			pgMemory = info->next;
		}
		else
		{
			info->prev->next = info->next;

			if (info->next != NULL)
				info->next->prev = info->prev;
		}

		free(info);
	}
	else
		PG_DIE("Memory at location %p has already been freed.", ptr);
}

pgVoid pgReportAllocatedMemory()
{
	for (pgMemoryInfo* info = pgMemory; info != NULL; info = info->next)
		PG_ERROR("Leaked an instance of type '%s', allocated at %s:%d.", info->type, info->file, info->line);
}

#endif