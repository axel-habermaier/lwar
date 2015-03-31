#pragma once

namespace Memory
{
	// Copies the given element from source to destination.
	template <typename T>
	void Copy(T* destination, const T* source);

	// Copies the given number of elements from source to destination.
	template <typename T>
	void CopyArray(T* destination, const T* source, uint32 elementCount);

	// Copies the given element from source to destination. The memory regions may overlap.
	template <typename T>
	void Move(T* destination, const T* source);

	// Copies the given number of elements from source to destination. The memory regions may overlap.
	template <typename T>
	void MoveArray(T* destination, const T* source, uint32 elementCount);

	// Sets the memory of the destination to the given value.
	template <typename T>
	void Set(T* destination, int32 value);

	// Copies the given number of bytes from source to destination.
	void Copy(void* destination, const void* source, uint32 byteCount);

	// Copies the given number of bytes from source to destination. The memory regions may overlap.
	void Move(void* destination, const void* source, uint32 byteCount);

	// Sets the given number of bytes at the destination address to the given value.
	void Set(void* destination, uint32 byteCount, int32 value);
}

#ifdef PG_SYSTEM_WINDOWS
	#define PG_NEW(type, ...) \
		static_cast<type*>(Memory::Allocate(new (std::nothrow) type(##__VA_ARGS__), #type, __FILE__, __LINE__))
#else
	#define PG_NEW(type, ...) \
		static_cast<type*>(Memory::Allocate(new (std::nothrow) type(__VA_ARGS__), #type, __FILE__, __LINE__))
#endif

#define PG_NEW_ARRAY(type, elementCount) \
	static_cast<type*>(Memory::AllocateArray(new (std::nothrow) type[elementCount], elementCount, #type, __FILE__, __LINE__))

#define PG_DELETE(obj) delete obj

#define PG_DELETE_ARRAY(obj) delete[] obj

#include "Memory.inl"
