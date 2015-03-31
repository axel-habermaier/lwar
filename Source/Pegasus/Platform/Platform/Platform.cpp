#include "Prelude.hpp"

LogCallback OnLogged = nullptr;

PG_API_EXPORT void Initialize(LogCallback logCallback)
{
	OnLogged = logCallback;
	
#ifdef PG_SYSTEM_WINDOWS
	WSADATA wsaData;

	if (WSAStartup(MAKEWORD(1, 1), &wsaData) != 0)
		PG_DIE("%s", Win32::GetError("Winsock initialization failed."));
#endif

	if (SDL_Init(SDL_INIT_VIDEO) != 0)
		PG_DIE("SDL initialization failed: %s.", SDL_GetError());

#ifndef PG_SYSTEM_WINDOWS
	SDL_version version;
	SDL_GetVersion(&version);
	if (!SDL_VERSION_ATLEAST(2, 0, 2))
		PG_DIE("SDL2 is outdated: Version %d.%d.%d is installed but at version 2.0.2 is required.", version.major, version.minor, version.patch);
#endif

	SDL_StopTextInput();

	SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 8);
	SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 0);
	SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 0);
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

	PG_DEBUG_ONLY(SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG | SDL_GL_CONTEXT_DEBUG_FLAG));
	PG_RELEASE_ONLY(SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG));

	if (SDL_GL_LoadLibrary(nullptr) != 0)
		PG_DIE("Failed to load the OpenGL library: %s.", SDL_GetError());
}

PG_API_EXPORT void Shutdown()
{
#ifdef PG_SYSTEM_WINDOWS
	if (WSACleanup() != 0)
		PG_ERROR("%s", Win32::GetError("Winsock cleanup failed."));
#endif

	SDL_Quit();
	OnLogged = nullptr;
}

PG_API_EXPORT float64 GetTime()
{
	return SDL_GetPerformanceCounter() / static_cast<float64>(SDL_GetPerformanceFrequency());
}