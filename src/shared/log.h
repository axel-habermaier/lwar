// Describes the type of the log entry
typedef enum 
{
	LT_FATAL,		
	LT_ERROR,		
	LT_WARNING,		
	LT_INFO,		
	LT_DEBUG		
} LogType; 

// Collects logging data
typedef struct
{
	const char*	msg;
	const char*	file;
	size_t line;
	LogType type;
} LogData;

// Log callback function
typedef void (*LogFunc)(const LogData* data);

// A logger that must be registered to be called when a log entry is created
typedef struct Logger Logger;
typedef struct Logger
{
	LogFunc callback;
	List list;
} Logger;

// Initializes the logging system
void log_init();

// Registers a logger
void log_register(Logger* logger);

// Writes a log entry
void log_write(const char* msg, LogType type, const char* file, size_t line, ...);

const char* logtype_to_string(LogType type);

#define LOGGER_INIT(callback) { callback, { NULL, NULL } }
