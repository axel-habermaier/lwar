namespace Memory
{
	template <typename T>
	void Copy(T* destination, const T* source)
	{
		Copy(static_cast<void*>(destination), static_cast<const void*>(source), sizeof(T));
	}

	template <typename T>
	void CopyArray(T* destination, const T* source, uint32 elementCount)
	{
		Copy(static_cast<void*>(destination), static_cast<const void*>(source), elementCount * sizeof(T));
	}

	template <typename T>
	void Move(T* destination, const T* source)
	{
		Move(static_cast<void*>(destination), static_cast<const void*>(source), sizeof(T));
	}

	template <typename T>
	void MoveArray(T* destination, const T* source, uint32 elementCount)
	{
		Move(static_cast<void*>(destination), static_cast<const void*>(source), elementCount * sizeof(T));
	}

	template <typename T>
	void Set(T* destination, int32 value)
	{
		Set(static_cast<void*>(destination), sizeof(T), value);
	}

	template <typename T>
	bool Equals(const T* obj1, const T* obj2)
	{
		return Equals(static_cast<const void*>(obj1), static_cast<const void*>(obj2), sizeof(T));
	}

	PG_INLINE void* Allocate(void* ptr, const char* typeName, const char* file, int line)
	{
		if (ptr == nullptr)
			PG_DIE("Failed to allocate an instance of type '%s' at '%s:%d'.", typeName, file, line);

		return ptr;
	}

	PG_INLINE void* AllocateArray(void* ptr, uint32 elementCount, const char* typeName, const char* file, int line)
	{
		if (ptr == nullptr)
			PG_DIE("Failed to allocate an array of %d instances of type '%s' at '%s:%d'.", elementCount, typeName, file, line);

		return ptr;
	}
}
