#ifdef __cplusplus
extern "C" {
#endif

#ifdef _MSC_VER
	#define EXPORT __declspec(dllexport)
#else
	#define EXPORT
#endif

	typedef void (*LogCallback)(const char* message);

	typedef struct
	{
		// The die callback is expected to terminate the application. If it does not, the resulting behavior of the
		// server is undefined.
		LogCallback die;
		LogCallback error;
		LogCallback warning;
		LogCallback info;
		LogCallback debug;
	} LogCallbacks;

    typedef void (*TimerCallback)(unsigned int timer);
    typedef void (*CounterCallback)(unsigned int counter, unsigned int value);

    typedef struct
    {
        TimerCallback   start;
        TimerCallback   stop;
        CounterCallback counted;
    } PerformanceCallbacks;

	EXPORT void server_log_callbacks(LogCallbacks callbacks);
	EXPORT void server_performance_callbacks(PerformanceCallbacks callbacks);

    /* initialize server data structures
     * and network connection,
     * return > 0 on success
     */
    EXPORT int  server_init();

    /* should be called periodically
     * clock is a monotonic counter in millisecs
     *       that MUST start with 0
     * force specifies to bypass the internal message delay
     *       and to send status updates immediatly
     * return > 0 on success
     * return = 0 if terminated normally
     * return < 0 if some internal error happened
     * the first call will have no effect but to set the current time
     */
    EXPORT int  server_update(unsigned long long clock, int force);

    /* free data structures
     * shutdown network connection
     */
    EXPORT void server_shutdown();
#ifdef __cplusplus
}
#endif
