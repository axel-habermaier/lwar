//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Compiler specific macros and configuration
//-------------------------------------------------------------------------------------------------------------------------------------------------------

#if defined(_MSC_VER)

	#define PG_SYSTEM_WINDOWS
	#define PG_COMPILER_VISUAL_STUDIO

	// Allows emitting #pragma directives within macros
	#define PG_PRAGMA(pragma) __pragma(pragma)
	// Disables a specific warning
	#define PG_DISABLE_WARNING(number) PG_PRAGMA(warning(push)) PG_PRAGMA(warning(disable : number))
	// Enables a specific warning that has been disabled by PG_DISABLE_WARNING before
	#define PG_ENABLE_WARNING(number) PG_PRAGMA(warning(pop))

	// Disable warnings
	#pragma warning(disable : 4514)	// 'function' : unreferenced inline function has been removed
	#pragma warning(disable : 4820)	// 'n' bytes padding added after data member 'member'
	#pragma warning(disable : 4206) // nonstandard extension used: translation unit is empty
	#pragma warning(disable : 4710) // 'function' : function not inlined
	#pragma warning(disable : 4350) // behavior change: 'function' called instead of 'function'
	#pragma warning(disable : 4711) // function 'function' selected for automatic inline expansion
	#pragma warning(disable : 4625) // 'type': copy constructor was implicitly defined as deleted because of base
	#pragma warning(disable : 4626) // 'type': copy assignment operator was implicitly defined as deleted because of base
	#pragma warning(disable : 4571) // Informational: catch(...) semantics changed since Visual C++ 7.1
	#pragma warning(disable: 4371) // 'type' : layout of class may have changed from a previous version of the compiler
	#pragma warning(disable: 4355) // 'this' : used in base member initialize list
	#pragma warning(disable: 4505) // 'func' : unreferenced local function has been removed
	#pragma warning(disable: 4265) // 'class' : class has virtual functions, but destructor is not virtual
	#define _CRT_SECURE_NO_WARNINGS // No warnings about non-secure standard library functions
	#define _SCL_SECURE_NO_WARNINGS // No warnings about non-secure standard library functions

	#define PG_ALIGNOF(type) __alignof(type)
	#define PG_ALIGNAS(type) __declspec(align(__alignof(type)))
	#define PG_INLINE   __forceinline
	#define PG_NORETURN __declspec(noreturn)
	#define PG_API_EXPORT extern "C" __declspec(dllexport)
	#define PG_WINDOWS_ONLY(x) x
	#define PG_LINUX_ONLY(x)

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). Warning 4127 is disabled
	// to avoid the warning about the constant expression inside the while
	#define PG_MULTILINE_MACRO_BEGIN PG_DISABLE_WARNING(4127) do {
	#define PG_MULTILINE_MACRO_END } while (false) PG_ENABLE_WARNING(4127)

	#ifdef DEBUG	
		#define PG_NO_SWITCH_DEFAULT PG_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
	#else	
		#define PG_NO_SWITCH_DEFAULT __assume(0)
	#endif

	#if defined(DEBUG) && !defined(PG_NO_DEBUG_BREAK)
		#define PG_DEBUG_BREAK			\
			if (::IsDebuggerPresent())	\
				__debugbreak()
	#else	
		#define PG_DEBUG_BREAK 
	#endif

	// Common Win32 function forward declarations
	extern "C" __declspec(dllimport) void __stdcall OutputDebugStringA(const char*);
	extern "C" __declspec(dllimport) int __stdcall IsDebuggerPresent(void);

#elif defined(__GNUC__)

	#define PG_COMPILER_GCC
	#define PG_SYSTEM_LINUX
	
	#define PG_DISABLE_WARNING(number) 
	#define PG_ENABLE_WARNING(number)

	#define PG_ALIGNOF(type) alignof(type)
	#define PG_ALIGNAS(type) alignas(type)
	#define PG_INLINE inline
	#define PG_NORETURN __attribute__ ((noreturn))
	#define PG_DEBUG_BREAK 
	#define PG_API_EXPORT extern "C" __attribute__ ((__visibility__ ("default")))
	#define PG_WINDOWS_ONLY(x)
	#define PG_LINUX_ONLY(x) x

	#ifdef DEBUG	
		#define PG_NO_SWITCH_DEFAULT PG_ASSERT_NOT_REACHED("Default case of switch statement should never be executed.")
	#else	
		#define PG_NO_SWITCH_DEFAULT __builtin_unreachable()
	#endif

	// Wrapping a multi-line macro inside the multi-line begin/end macros ensures that the macro can be used like an 
	// ordinary function (i.e. it works correctly within single-line if statements, for instance). 
	#define PG_MULTILINE_MACRO_BEGIN do {
	#define PG_MULTILINE_MACRO_END } while (false)

#else
	#error Unsupported OS or compiler
#endif

#ifdef DEBUG
	#define PG_DEBUG_ONLY(x) x
	#define PG_RELEASE_ONLY(x) 
#else
	#define PG_DEBUG_ONLY(x) 
	#define PG_RELEASE_ONLY(x) x
#endif

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Standard types
//-------------------------------------------------------------------------------------------------------------------------------------------------------

using ulong = unsigned long;
using uchar = unsigned char;
using wchar = wchar_t;
using byte = unsigned char;
using sbyte = signed char;
using uint16 = unsigned short;
using int16 = signed short;
using uint32 = unsigned int;
using int32 = signed int;
using uint64 = unsigned long long;
using int64 = signed long long;
using float32 = float;
using float64 = double;

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Helper macros
//-------------------------------------------------------------------------------------------------------------------------------------------------------

// Can be used to silence warnings about unused parameters
#define PG_UNUSED(x) (void)(x)

// Assertions
#ifdef DEBUG

	#define PG_ASSERT(cond, fmt, ...)										\
		PG_MULTILINE_MACRO_BEGIN											\
		if (!(cond))														\
			PG_DIE("Assertion '" #cond "' failed. " fmt, ##__VA_ARGS__);	\
		PG_MULTILINE_MACRO_END

	#define PG_ASSERT_NOT_REACHED(fmt, ...) PG_DIE(fmt, ##__VA_ARGS__)
	#define PG_ASSERT_NOT_NULL(ptr) PG_ASSERT((ptr) != nullptr, "Pointer '" #ptr "' is null.")
	#define PG_ASSERT_NULL(ptr) PG_ASSERT((ptr) == nullptr, "Pointer '" #ptr "' must be null.")
	#define PG_ASSERT_GEQUAL(value, lowerBound) PG_ASSERT((value) >= (lowerBound), "'" #value "' must be greater than or equal to '" #lowerBound "'.")
	#define PG_ASSERT_LEQUAL(value, upperBound) PG_ASSERT((value) <= (upperBound), "'" #value "' must be less than or equal to '" #upperBound "'.")
	#define PG_ASSERT_GREATER(value, lowerBound) PG_ASSERT((value) > (lowerBound), "'" #value "' must be greater than '" #lowerBound "'.")
	#define PG_ASSERT_LESS(value, upperBound) PG_ASSERT((value) < (upperBound), "'" #value "' must be less than '" #upperBound "'.")
	#define PG_ASSERT_IN_RANGE(value, first, last) PG_ASSERT((value) >= (first) && (value) <= (last), "'" #value "' is out of range.")
	#define PG_ASSERT_VALID_INDEX(index, length) PG_ASSERT((index) >= 0 && (index) < length, "'" #index "' is out of range.")

#else

	#define PG_ASSERT(cond, fmt, ...)
	#define PG_ASSERT_NOT_REACHED(fmt, ...) 
	#define PG_ASSERT_NOT_NULL(ptr)
	#define PG_ASSERT_NULL(ptr)
	#define PG_ASSERT_GEQUAL(value, lowerBound)
	#define PG_ASSERT_LEQUAL(value, lowerBound)
	#define PG_ASSERT_GREATER(value, lowerBound)
	#define PG_ASSERT_LESS(value, upperBound)
	#define PG_ASSERT_IN_RANGE(value, first, last)
	#define PG_ASSERT_VALID_INDEX(index, length)				

#endif

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Default includes
//-------------------------------------------------------------------------------------------------------------------------------------------------------

// Required C library and STL includes
PG_DISABLE_WARNING(4548) // expression before comma has no effect
	#include <cstddef>
	#include <cstring>
	#include <cctype>
	#include <type_traits>
	#include <utility>
	#include <new>
	#include <memory>
	#include <limits>
	#include <vector>
	#include <map>
	#include <algorithm>
	#include <string>
	#include <sstream>
PG_ENABLE_WARNING(4548)

// Interop includes
#include "Interop/Enums.hpp"
#include "Interop/Callbacks.hpp"
#include "Interop/Structs.hpp"
#include "Interop/Interfaces.hpp"
#include "Interop/Funcs.hpp"

// Platform includes
#if defined(PG_SYSTEM_WINDOWS)
	
	#include "Platform/Win32.hpp"

#elif defined(PG_SYSTEM_LINUX)

	#include <arpa/inet.h>
	#include <fcntl.h>
	#include <netinet/in.h>
	#include <sys/socket.h>
	#include <sys/types.h>
	#include <unistd.h>

#endif

// SDL2 includes
PG_DISABLE_WARNING(4668) // 'macro' is not defined as a preprocessor macro
	#include "SDL2/SDL.h"
	#ifdef PG_SYSTEM_WINDOWS
		#include "SDL2/SDL_syswm.h"
	#endif
PG_ENABLE_WARNING(4668)

// Utility includes
#include "Utilities/Log.hpp"
#include "Utilities/Memory.hpp"
#include "Utilities/Casts.hpp"
#include "Utilities/Enumeration.hpp"

// Platform includes
#include "Platform/NativeWindow.hpp"

// Graphics includes
#include "Graphics/Graphics.hpp"
#include "Graphics/OpenGL3/BindingsGL3.hpp"
#include "Graphics/OpenGL3/OpenGL3.hpp"
#include "Graphics/OpenGL3/GraphicsDeviceGL3.hpp"
#include "Graphics/Direct3D11/Direct3D11.hpp"
#include "Graphics/Direct3D11/GraphicsDeviceD3D11.hpp"

// Network includes
#include "Network/IPAddress.hpp"
#include "Network/IPEndPoint.hpp"
#include "Network/NetworkException.hpp"
#include "Network/UdpSocket.hpp"