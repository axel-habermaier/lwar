#pragma once

class NetworkException
{
public:
	NetworkException(const char* message);
	
	std::string Message() const;

private:
	std::string _message;
};
