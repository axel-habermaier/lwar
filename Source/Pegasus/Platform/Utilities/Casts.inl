namespace Internal
{
	// Source: http://blog.reverberate.org/2012/12/testing-for-integer-overflow-in-c-and-c.html

	template<bool isSigned, typename T>
	class IsNegativeFunctor;

	template<typename T>
	class IsNegativeFunctor < true, T >
	{
	public:
		bool operator()(T x)
		{
			return x < 0;
		}
	};

	template<typename T>
	class IsNegativeFunctor < false, T >
	{
	public:
		bool operator()(T x)
		{
			PG_UNUSED(x);

			// Unsigned type is never negative.
			return false;
		}
	};

	template<typename T>
	bool IsNegative(T x)
	{
		return IsNegativeFunctor<std::numeric_limits<T>::is_signed, T>()(x);
	}

	template<typename To, typename From>
	bool WillOverflow(From val)
	{
		static_assert(std::numeric_limits<From>::is_integer, "Expected integer type.");
		static_assert(std::numeric_limits<To>::is_integer, "Expected integer type.");

		auto signedOk = !std::numeric_limits<To>::is_signed ||
			((!std::numeric_limits<From>::is_signed &&
			(uintmax_t)val > (uintmax_t)INTMAX_MAX) ||
			(intmax_t)val < (intmax_t)std::numeric_limits<To>::min() ||
			(intmax_t)val > (intmax_t)std::numeric_limits<To>::max());

		auto unsignedOk = std::numeric_limits<To>::is_signed ||
			(IsNegative(val) ||
			(uintmax_t)val > (uintmax_t)std::numeric_limits<To>::max());

		return signedOk && unsignedOk;
	}
}

template <typename T, typename U>
inline T safe_static_cast(U value)
{
	PG_ASSERT(!Internal::WillOverflow<T>(value), "Value will overflow.");
	return static_cast<T>(value);
}