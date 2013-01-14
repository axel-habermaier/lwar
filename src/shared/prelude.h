// ---------------------------------------------------------------------------------------------------------------------
// Visual Studio specific macros
#ifdef _MSC_VER

	#define WINDOWS

	// Allows emitting #pragma directives within macros
	#define LWAR_PRAGMA(pragma)	__pragma(pragma)
	// Disables a specific warning
	#define LWAR_DISABLE_WARNING(number) LWAR_PRAGMA(warning(push)) LWAR_PRAGMA(warning(disable : number))
	// Enables a specific warning that has been disabled by LWAR_DISABLE_WARNING before
	#define LWAR_ENABLE_WARNING(number)	LWAR_PRAGMA(warning(pop))

	// Disable warnings
	#pragma warning(disable : 4514)	// 'function' : unreferenced inline function has been removed
	#pragma warning(disable : 4820)	// 'n' bytes padding added after data member 'member'
	#define _CRT_SECURE_NO_WARNINGS // No warnings about non-secure standard library functions

	#define LWAR_INLINE __forceinline
	#define LWAR_NORETURN __declspec(noreturn)

	#ifdef DEBUG	
		#define LWAR_NO_SWITCH_DEFAULT \
			LWAR_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
		#define LWAR_DEBUG_BREAK __debugbreak()
	#else	
		#define LWAR_NO_SWITCH_DEFAULT __assume(0)
		#define LWAR_DEBUG_BREAK 
	#endif

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). Warning 4127 is disabled
	// to avoid the warning about the constant expression inside the while
	#define LWAR_MULTILINE_MACRO_BEGIN LWAR_DISABLE_WARNING(4127) do {
	#define LWAR_MULTILINE_MACRO_END } while (false) LWAR_ENABLE_WARNING(4127)

#endif

// ---------------------------------------------------------------------------------------------------------------------
// Linux specific macros
#ifdef __unix__

	#define LINUX

	#define LWAR_INLINE inline
	#define LWAR_NORETURN 
	#define LWAR_DEBUG_BREAK 

	#ifdef DEBUG	
		#define LWAR_NO_SWITCH_DEFAULT \
			LWAR_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
	#else	
		#define LWAR_NO_SWITCH_DEFAULT 
	#endif

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). 
	#define LWAR_MULTILINE_MACRO_BEGIN do {
	#define LWAR_MULTILINE_MACRO_END } while (false)

#endif

// ---------------------------------------------------------------------------------------------------------------------
// Macros

// Can be used to silence warnings about unused parameters
#define LWAR_UNUSED(x) (void)(x)

// Fails compilation if the condition evaluates to false at compile-time
#define COMPILE_TIME_ASSERT(name, x) typedef int assert_##name[(x) * 2 - 1]

// ---------------------------------------------------------------------------------------------------------------------
// Log macros
#define LWAR_LOG(type, fmt, ...) \
	log_write(fmt, type, __FILE__, (size_t)(__LINE__), __VA_ARGS__)

#define LWAR_DIE(fmt, ...) \
	LWAR_MULTILINE_MACRO_BEGIN \
	LWAR_LOG(LT_FATAL, fmt, __VA_ARGS__); \
	LWAR_DEBUG_BREAK; \
	sys_abort(); \
	LWAR_MULTILINE_MACRO_END

#define LWAR_ERROR(fmt, ...) LWAR_LOG(LT_ERROR, fmt, __VA_ARGS__)
#define LWAR_WARN(fmt, ...) LWAR_LOG(LT_WARNING, fmt, __VA_ARGS__)
#define LWAR_INFO(fmt, ...) LWAR_LOG(LT_INFO, fmt, __VA_ARGS__)
#define LWAR_DEBUG(fmt, ...) LWAR_LOG(LT_DEBUG, fmt, __VA_ARGS__)

// ---------------------------------------------------------------------------------------------------------------------
// Assert macros
#ifdef DEBUG
	#define LWAR_ASSERT(cond, fmt, ...) \
		LWAR_MULTILINE_MACRO_BEGIN \
		if (!(cond)) \
			LWAR_DIE("Assertion '" #cond "' failed. " fmt, __VA_ARGS__); \
		LWAR_MULTILINE_MACRO_END

	#define LWAR_ASSERT_NOT_REACHED(fmt, ...) LWAR_DIE(fmt, __VA_ARGS__)
	#define LWAR_ASSERT_NOT_NULL(ptr) LWAR_ASSERT((ptr) != NULL, "Pointer '" #ptr "' is null.")
	#define LWAR_ASSERT_NULL(ptr) LWAR_ASSERT((ptr) == NULL, "Pointer '" #ptr "' must be null.")
#else
	#define LWAR_ASSERT(cond, fmt, ...) LWAR_UNUSED(cond)
	#define LWAR_ASSERT_NOT_REACHED(fmt, ...) 
	#define LWAR_ASSERT_NOT_NULL(ptr) LWAR_UNUSED(ptr)
	#define LWAR_ASSERT_NULL(ptr) LWAR_UNUSED(ptr)
#endif

// ---------------------------------------------------------------------------------------------------------------------
// Standard types
#include <stdint.h>
#include <stddef.h>

typedef float   float32_t;
typedef double  float64_t;

#ifdef LINUX
typedef enum { true = 1, false = 0 } bool;
#endif

typedef struct
{
	uint16_t n;
	uint16_t gen;
} Id;

// ---------------------------------------------------------------------------------------------------------------------
// Standard project includes
#include "list.h"
#include "log.h"
#include "sys.h"