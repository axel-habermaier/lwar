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
	#error Unsupported operation system or compiler
#endif

//====================================================================================================================
// Standard types
//====================================================================================================================

typedef void				pgVoid;
typedef unsigned char		pgByte;
typedef char				pgChar;
typedef const char*			pgString;
typedef unsigned char		pgUint8;
typedef signed char			pgInt8;
typedef unsigned short		pgUint16;
typedef signed short		pgInt16;
typedef unsigned int		pgUint32;
typedef signed int			pgInt32;
typedef unsigned long long	pgUint64;
typedef signed long long	pgInt64;
typedef float				pgFloat32;
typedef double				pgFloat64;
typedef pgInt32				pgBool;

#define PG_TRUE				1
#define PG_FALSE			0

//====================================================================================================================
// Pegasus library types
//====================================================================================================================

typedef pgVoid (*pgLogCallback)(pgString message);

typedef struct
{
	// The die callback is expected to terminate the application. If it does not, the resulting behavior of the
	// Pegasus library is undefined.
	pgLogCallback die;
	pgLogCallback error;
	pgLogCallback warning;
	pgLogCallback info;
	pgLogCallback debug;
} pgLogCallbacks;

//====================================================================================================================
// Pegasus library functions
//====================================================================================================================

PG_API_EXPORT pgVoid pgInitialize(pgLogCallbacks* callbacks);
PG_API_EXPORT pgVoid pgShutdown();
PG_API_EXPORT pgFloat64 pgGetTime();

#endif