#include "Prelude.hpp"

namespace Memory
{
	void Copy(void* destination, const void* source, uint32 byteCount)
	{
		PG_ASSERT_NOT_NULL(source);
		PG_ASSERT_NOT_NULL(destination);
		PG_ASSERT((source < destination && static_cast<const byte*>(source)+byteCount <= destination) ||
				  (destination < source  && static_cast<byte*>(destination)+byteCount <= source),
				  "The memory regions overlap.");

		if (byteCount <= 0)
			return;

		memcpy(destination, source, byteCount);
	}

	void Move(void* destination, const void* source, uint32 byteCount)
	{
		PG_ASSERT_NOT_NULL(source);
		PG_ASSERT_NOT_NULL(destination);

		if (byteCount <= 0)
			return;

		memmove(destination, source, byteCount);
	}

	void Set(void* destination, uint32 byteCount, int value)
	{
		PG_ASSERT_NOT_NULL(destination);

		if (byteCount <= 0)
			return;

		memset(destination, value, byteCount);
	}

	bool Equals(const void* obj1, const void* obj2, uint32 byteCount)
	{
		PG_ASSERT_NOT_NULL(obj1);
		PG_ASSERT_NOT_NULL(obj2);

		if (obj1 == obj2)
			return true;

		return memcmp(obj1, obj2, byteCount) == 0;
	}
}