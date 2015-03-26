template <typename T, typename>
static T operator | (T left, T right)
{
	return static_cast<T>(
		static_cast<typename std::underlying_type<T>::type>(left) |
		static_cast<typename std::underlying_type<T>::type>(right));
}

template <typename T, typename>
static T& operator |= (T& left, T right)
{
	return left = left | right;
}

template <typename T, typename>
static T operator & (T left, T right)
{
	return static_cast<T>(
		static_cast<typename std::underlying_type<T>::type>(left)&
		static_cast<typename std::underlying_type<T>::type>(right));
}

template <typename T, typename>
static T& operator &= (T& left, T right)
{
	return left = left & right;
}

template <typename T, typename>
static T operator ~(T e)
{
	return static_cast<T>(~static_cast<typename std::underlying_type<T>::type>(e));
}

namespace Enum
{
	template <typename T, typename>
	static bool HasFlag(T flags, T flag)
	{
		return (flags & flag) == flag;
	}
}
