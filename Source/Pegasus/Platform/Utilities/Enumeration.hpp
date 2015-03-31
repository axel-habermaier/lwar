#pragma once

template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
static T operator | (T left, T right);

template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
static T& operator |= (T& left, T right);

template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
static T operator & (T left, T right);

template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
static T& operator &= (T& left, T right);

template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
static T operator ~(T e);

namespace Enum
{
	template <typename T, typename = typename std::enable_if<std::is_enum<T>::value>::type>
	static bool HasFlag(T flags, T flag);

}

#include "Enumeration.inl"