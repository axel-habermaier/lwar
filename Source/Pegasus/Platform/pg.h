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
// Standard types
//====================================================================================================================

typedef void				pgVoid;
typedef unsigned char		pgByte;
typedef char				pgChar;
typedef const char*			pgString;
typedef unsigned char		pgUInt8;
typedef signed char			pgInt8;
typedef unsigned short		pgUInt16;
typedef signed short		pgInt16;
typedef unsigned int		pgUInt32;
typedef signed int			pgInt32;
typedef unsigned long long	pgUInt64;
typedef signed long long	pgInt64;
typedef float				pgFloat32;
typedef double				pgFloat64;
typedef pgInt32				pgBool;

#define PG_TRUE				1
#define PG_FALSE			0

//====================================================================================================================
// Pegasus library types
//====================================================================================================================

typedef enum
{
	PG_LOGTYPE_FATAL	= 1,
	PG_LOGTYPE_ERROR	= 2,
	PG_LOGTYPE_WARNING	= 3,
	PG_LOGTYPE_INFO		= 4,
	PG_LOGTYPE_DEBUG	= 5
} pgLogType;

// The callback is expected to terminate the application if type == PG_LOGTYPE_FATAL. If it does not, the resulting 
// behavior of the Pegasus library is undefined.
typedef pgVoid (*pgLogCallback)(pgLogType type, pgString message);

//====================================================================================================================
// Pegasus library functions
//====================================================================================================================

PG_API_EXPORT pgVoid pgInitialize(pgLogCallback callback, pgString appName);
PG_API_EXPORT pgVoid pgShutdown();
PG_API_EXPORT pgFloat64 pgGetTime();

#endif