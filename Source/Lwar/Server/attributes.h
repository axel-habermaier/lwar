#ifndef NORETURN

#ifdef _MSC_VER
	#define NORETURN __declspec(noreturn)
    #define FORMAT
#else
	#define NORETURN __attribute__ ((noreturn))
	#define FORMAT   __attribute__ ((format(printf,1,2)))
#endif

#endif
