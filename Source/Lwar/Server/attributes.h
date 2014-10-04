#ifndef NORETURN

#ifdef _MSC_VER
	#define NORETURN __declspec(noreturn)
#else
	#define NORETURN __attribute__ ((noreturn))
#endif

#endif
