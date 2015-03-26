#pragma once

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Log Macros
//-------------------------------------------------------------------------------------------------------------------------------------------------------

// Logs a fatal error and terminates the application.
#define PG_DIE(fmt, ...)												\
	PG_MULTILINE_MACRO_BEGIN											\
		OnLogged(LogType::Fatal, Format(fmt, ##__VA_ARGS__).c_str());	\
		PG_DEBUG_BREAK;													\
		NoReturn();														\
	PG_MULTILINE_MACRO_END

// Logs an error message.
#define PG_ERROR(fmt, ...) \
	OnLogged(LogType::Error, Format(fmt, ##__VA_ARGS__).c_str())		

// Logs a warning.
#define PG_WARN(fmt, ...) \
	OnLogged(LogType::Warning, Format(fmt, ##__VA_ARGS__).c_str())	

// Logs an informational message.
#define PG_INFO(fmt, ...) \
	OnLogged(LogType::Info, Format(fmt, ##__VA_ARGS__).c_str())		

#ifdef DEBUG
	// Logs a message containing debugging information in debug builds.
	#define PG_DEBUG(fmt, ...) \
		OnLogged(LogType::Debug, Format(fmt, ##__VA_ARGS__).c_str())	

	#define PG_DEBUG_IF(condition, fmt, ...)								\
		PG_MULTILINE_MACRO_BEGIN											\
		if (condition)														\
			OnLogged(LogType::Debug, Format(fmt, ##__VA_ARGS__).c_str());	\
		PG_MULTILINE_MACRO_END
#else
	// Debugging log messages are stripped out of release builds.
	#define PG_DEBUG(fmt, ...) PG_UNUSED(fmt)
	#define PG_DEBUG_IF(condition, fmt, ...)	\
		PG_MULTILINE_MACRO_BEGIN				\
			PG_UNUSED(condition);				\
			PG_UNUSED(fmt);						\
		PG_MULTILINE_MACRO_END
#endif

extern LogCallback OnLogged;

// Type-safe non-positional format function, similar to sprintf. Does not support any formatting options and
// the type-specifier of a format hole is ignored.
template<typename... TArgs>
inline std::string Format(const char* fmt, TArgs... args);

// A dummy non-returning function that is needed by the PG_DIE macro.
PG_NORETURN inline void NoReturn();

#include "Log.inl"