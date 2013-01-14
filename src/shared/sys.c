#include "prelude.h"
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>
#include <stdio.h>

LWAR_NORETURN void sys_abort()
{
	exit(EXIT_FAILURE);
}

int32_t sys_printf(const char* const fmt, ...)
{
	LWAR_ASSERT_NOT_NULL(fmt);

	va_list vl;
	va_start(vl, fmt);

	int32_t ret = vprintf(fmt, vl);
	va_end(vl);
	return ret;
}

int32_t sys_vsprintf(char* const dest, size_t length, const char* const fmt, va_list vl)
{
	LWAR_ASSERT_NOT_NULL(fmt);
	LWAR_ASSERT_NOT_NULL(fmt);
	LWAR_ASSERT(length > 0, "Destination buffer must have a size greater than zero.");

	return vsnprintf(dest, length, fmt, vl);
}

int32_t sys_sprintf(char* const dest, size_t length, const char* const fmt, ...)
{
	va_list vl;
	va_start(vl, fmt);

	int32_t ret = sys_vsprintf(dest, length, fmt, vl);
	va_end(vl);
	return ret;
}

void mem_copy(void* const dest, const void* const src, size_t bytes)
{
	LWAR_ASSERT_NOT_NULL(dest);
	LWAR_ASSERT_NOT_NULL(src);
	LWAR_ASSERT(dest < src || dest > ((char*)src) + bytes, "Memory regions are overlapping");
	LWAR_ASSERT(((char*)dest) + bytes < src || ((char*)dest) + bytes > ((char*)src) + bytes, "Memory regions are overlapping");

	memcpy(dest, src, bytes); 
}

void mem_move(void* const dest, const void* const src, size_t bytes)
{
	LWAR_ASSERT_NOT_NULL(dest);
	LWAR_ASSERT_NOT_NULL(src);

	memmove(dest, src, bytes); 
}