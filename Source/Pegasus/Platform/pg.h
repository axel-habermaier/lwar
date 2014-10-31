#ifndef pg_h__
#define pg_h__

//====================================================================================================================
// Compiler-specific defines
//====================================================================================================================
#ifdef _MSC_VER
	#define PG_API_EXPORT __declspec(dllexport)
#elif defined(__unix__)
	#define PG_API_EXPORT __attribute__ ((__visibility__ ("default")))
#else
	#error Unsupported operating system or compiler
#endif

//====================================================================================================================
// Parameter annotations
//====================================================================================================================

/// <summary>
///     Indicates that the parameter is used to pass data into the function and to retrieve data from the function.
/// </summary>
#define ref

/// <summary>
///     Indicates that the parameter is only used to retrieve data from the function.
/// </summary>
#define out

//====================================================================================================================
// Standard types
//====================================================================================================================

/// <summary>
///     Represents the void type.
/// </summary>
typedef void pgVoid;

/// <summary>
///     Represents an unsigned byte.
/// </summary>
typedef unsigned char pgByte;

/// <summary>
///     Represents a signed byte.
/// </summary>
typedef unsigned char pgSByte;

/// <summary>
///     Represents a single character.
/// </summary>
typedef char pgChar;

/// <summary>
///     Represents a string constant.
/// </summary>
typedef const char* pgString;

/// <summary>
///     Represents a 16-bit unsigned integer.
/// </summary>
typedef unsigned short pgUInt16;

/// <summary>
///     Represents a 16-bit signed integer.
/// </summary>
typedef signed short pgInt16;

/// <summary>
///     Represents a 32-bit unsigned integer.
/// </summary>
typedef unsigned int pgUInt32;

/// <summary>
///     Represents a 32-bit signed integer.
/// </summary>
typedef signed int pgInt32;

/// <summary>
///     Represents a 64-bit unsigned integer.
/// </summary>
typedef unsigned long long pgUInt64;

/// <summary>
///     Represents a 64-bit unsigned integer.
/// </summary>
typedef signed long long pgInt64;

/// <summary>
///     Represents a 32-bit floating point number.
/// </summary>
typedef float pgFloat32;

/// <summary>
///     Represents a 64-bit floating point number.
/// </summary>
typedef double pgFloat64;

/// <summary>
///     Represents a Boolean value with a storage size of 4 bytes.
/// </summary>
typedef pgInt32 pgBool;

/// <summary>
///     Represents the Boolean value 'true'.
/// </summary>
#define PG_TRUE	1

/// <summary>
///     Represents the Boolean value 'false'.
/// </summary>
#define PG_FALSE 0

//====================================================================================================================
// Pegasus library types
//====================================================================================================================

/// <summary>
///    Describes the type of a log entry.
/// </summary>
typedef enum
{
	/// <summary>
	///    Indicates that the log entry represents a fatal error.
	/// </summary>
	PG_LOGTYPE_FATAL = 1,

	/// <summary>
	///    Indicates that the log entry represents an error.
	/// </summary>
	PG_LOGTYPE_ERROR = 2,

	/// <summary>
	///    Indicates that the log entry represents a warning.
	/// </summary>
	PG_LOGTYPE_WARNING = 3,

	/// <summary>
	///    Indicates that the log entry represents an informational message.
	/// </summary>
	PG_LOGTYPE_INFO = 4,

	/// <summary>
	///    Indicates that the log entry represents debugging information.
	/// </summary>
	PG_LOGTYPE_DEBUG = 5
} pgLogType;

/// <summary>
///     The type of the callback that is invoked when a log entry is generated.
/// </summary>
/// <param name="type">The log type that was generated.</param>
/// <param name="message">The log message that was generated.</param>
/// <remarks>
///		The callback is expected to terminate the application if type == PG_LOGTYPE_FATAL. If it does not, the resulting 
///		behavior of the Pegasus library is undefined.
/// </remarks>
typedef pgVoid (*pgLogCallback)(pgLogType type, pgString message);

//====================================================================================================================
// Pegasus library functions
//====================================================================================================================

/// <summary>
///    Initializes the library.
/// </summary>
/// <param name="callback">The callback that should be invoked when a log entry is generated.</param>
PG_API_EXPORT pgVoid pgInitialize(pgLogCallback callback);

/// <summary>
///    Shuts down the library, freeing all internal resources.
/// </summary>
PG_API_EXPORT pgVoid pgShutdown();

/// <summary>
///    Gets the system time in seconds.
/// </summary>
PG_API_EXPORT pgFloat64 pgGetTime();

/// <summary>
///     Copies the given number of bytes from the source to the destination.
/// </summary>
/// <param name="destination">A pointer to the memory the data should be copied to.</param>
/// <param name="source">A pointer to the memory the data should be copied from.</param>
/// <param name="byteCount">The number of bytes that should be copied.</param>
PG_API_EXPORT pgVoid pgMemCopy(pgVoid* destination, const pgVoid* source, pgInt32 byteCount);

#endif