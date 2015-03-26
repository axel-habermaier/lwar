#include "Prelude.hpp"

NetworkException::NetworkException(const char* message)
{
	PG_WINDOWS_ONLY(_message = Win32::GetError(message, GetLastError()));
	PG_LINUX_ONLY(_message = Format("%s %s", message, strerror(errno)));
}

std::string NetworkException::Message() const
{
	return _message;
}