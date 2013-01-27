#ifdef __cplusplus
extern "C" {
#endif

#ifdef _MSC_VER
	#define EXPORT __declspec(dllexport)
#else
	#define SYM_EXPORT
#endif

    /* initialize server data structures
     * and network connection,
     * return > 0 on success
     */
    EXPORT int  server_init();

    /* should be called periodically
     * clock is a monotonic counter in millisecs
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
