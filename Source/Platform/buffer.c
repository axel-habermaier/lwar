#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgBuffer* pgCreateBuffer(pgGraphicsDevice* device, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size)
{
	pgBuffer* buffer;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(size, 0, INT32_MAX);

	PG_ALLOC(pgBuffer, buffer);
	buffer->device = device;
	buffer->size = size;
	buffer->type = type;
	pgCreateBufferCore(buffer, usage, data);

	return buffer;
}

pgVoid pgDestroyBuffer(pgBuffer* buffer)
{
	if (buffer == NULL)
		return;

	if (buffer->type == PG_CONSTANT_BUFFER)
	{
		for (int i = 0; i < PG_CONSTANT_BUFFER_SLOT_COUNT; ++i)
		{
			if (buffer->device->constantBuffers[i] == buffer)
				buffer->device->constantBuffers[i] = NULL;
		}
	}

	pgDestroyBufferCore(buffer);
	PG_FREE(buffer);
}

pgVoid* pgMapBuffer(pgBuffer* buffer, pgMapMode mode)
{
	PG_ASSERT_NOT_NULL(buffer);
	return pgMapBufferCore(buffer, mode);
}

pgVoid* pgMapBufferRange(pgBuffer* buffer, pgMapMode mode, pgInt32 offset, pgInt32 byteCount)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_IN_RANGE(offset, 0, buffer->size);
	PG_ASSERT_IN_RANGE(byteCount, 1, buffer->size);
	PG_ASSERT(offset + byteCount <= buffer->size, "Buffer overflow");

	return pgMapBufferRangeCore(buffer, mode, offset, byteCount);
}

pgVoid pgUnmapBuffer(pgBuffer* buffer)
{
	PG_ASSERT_NOT_NULL(buffer);
	pgUnmapBufferCore(buffer);
}

pgVoid pgBindConstantBuffer(pgBuffer* buffer, pgInt32 slot)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_IN_RANGE(slot, 0, PG_CONSTANT_BUFFER_SLOT_COUNT);
	PG_ASSERT(buffer->type == PG_CONSTANT_BUFFER, "Buffer is not a constant buffer.");

	if (buffer->device->constantBuffers[slot] == buffer)
		return;

	buffer->device->constantBuffers[slot] = buffer;
	pgBindConstantBufferCore(buffer, slot);
}

pgVoid pgUpdateConstantBuffer(pgBuffer* buffer, pgVoid* data)
{
	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(data);
	PG_ASSERT(buffer->type == PG_CONSTANT_BUFFER, "Buffer is not a constant buffer.");

	pgUpdateConstantBufferCore(buffer, data);
}