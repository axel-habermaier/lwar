namespace Internal
{
	inline void Format(std::ostringstream& s, const char* fmt)
	{
		while (*fmt)
		{
			if (*fmt == '%')
			{
				if (*(fmt + 1) == '%')
					++fmt;
				else
					throw "Too few arguments for format string.";
			}
			s << *fmt++;
		}
	}

	template<typename T, typename... TArgs>
	inline void Format(std::ostringstream& s, const char* fmt, T value, TArgs... args)
	{
		while (*fmt != '\0')
		{
			if (*fmt == '%')
			{
				if (*(fmt + 1) == '%')
					++fmt;
				else
				{
					s << value;
					::Internal::Format(s, fmt + 2, args...);
					return;
				}
			}
			s << *fmt++;
		}

		throw "Too many arguments for format string.";
	}
}

template<typename... TArgs>
inline std::string Format(const char* fmt, TArgs... args)
{
	std::ostringstream s;
	Internal::Format(s, fmt, std::forward<TArgs>(args) ...);
	return s.str();
}

PG_NORETURN inline void NoReturn()
{
	throw "The log function should never return control after a fatal log message.";
}