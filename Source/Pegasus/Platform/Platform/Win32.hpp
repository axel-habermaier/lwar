#pragma once

#ifdef PG_SYSTEM_WINDOWS

#define WIN32_LEAN_AND_MEAN
#define WIN32_EXTRA_LEAN
#define VC_EXTRALEAN
#define NOGDICAPMASKS
#define NOMENUS
#define NORASTEROPS
#define NOATOM
#define NODRAWTEXT
#define NOKERNEL
#define NOMEMMGR
#define NOMETAFILE
#define NOOPENFILE
#define NOSERVICE
#define NOSOUND
#define NOWINDOWSTATION
#define NOCOMM
#define NOKANJI
#define NOHELP
#define NOPROFILER
#define NODEFERWINDOWPOS
#define NOMCX

#ifndef NOMINMAX
	#define NOMINMAX
#endif

#ifdef _WIN32_WINNT
	#undef _WIN32_WINNT
#endif

#ifdef NTDDI_VERSION
	#undef NTDDI_VERSION
#endif

#ifdef _WIN32_WINNT
	#undef _WIN32_WINNT
#endif

#ifdef WINVER
	#undef WINVER
#endif

#define NTDDI_VERSION NTDDI_WIN7
#define _WIN32_WINNT _WIN32_WINNT_WIN7
#define WINVER _WIN32_WINNT_WIN7
#define D3D11_NO_HELPERS

PG_DISABLE_WARNING(4668)
PG_DISABLE_WARNING(4365)
PG_DISABLE_WARNING(4574)
PG_DISABLE_WARNING(4917)
PG_DISABLE_WARNING(4365)
PG_DISABLE_WARNING(4061)
PG_DISABLE_WARNING(4625)
PG_DISABLE_WARNING(4626)
PG_DISABLE_WARNING(4668)
PG_DISABLE_WARNING(4986)

	#include <Windows.h>
	#include <winsock2.h>
	#include <ws2tcpip.h>
	#include <dxgi.h>
	#include <d3d11.h>
	#include <wrl.h>

PG_ENABLE_WARNING(4668)
PG_ENABLE_WARNING(4365)
PG_ENABLE_WARNING(4574)
PG_ENABLE_WARNING(4917)
PG_ENABLE_WARNING(4365)
PG_ENABLE_WARNING(4061)
PG_ENABLE_WARNING(4625)
PG_ENABLE_WARNING(4626)
PG_ENABLE_WARNING(4668)
PG_ENABLE_WARNING(4986)

struct IDXGIDebug;

template <typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;

namespace Win32
{
	// Gets a user-friendly error message for the last Windows error that occurred.
	std::string GetError(const char* message);

	// Gets a user-friendly error message for the given Windows error.
	std::string GetError(const char* message, DWORD error);
}

#endif