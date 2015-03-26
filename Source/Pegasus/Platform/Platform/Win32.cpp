#include "Prelude.hpp"

#ifdef PG_SYSTEM_WINDOWS

namespace Win32
{
	std::string GetError(const char* message)
	{
		return GetError(message, GetLastError());
	}

	std::string GetError(const char* message, DWORD error)
	{
		char buffer[2048];
		auto length = FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, nullptr, error,
									 MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), buffer, sizeof(buffer) / sizeof(char), nullptr);

		if (length > 0)
			return Format("%s %s", message, buffer);
		else
			return Format("%s (error code: %d)", message, error);
	}
}

#endif