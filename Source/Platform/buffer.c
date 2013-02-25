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
	pgCreateBufferCore(buffer, type, usage, data, size);

	return buffer;
}

pgVoid pgDestroyBuffer(pgBuffer* buffer)
{
	if (buffer == NULL)
		return;

	pgDestroyBufferCore(buffer);
	PG_FREE(buffer);
}

pgVoid* pgMapBuffer(pgBuffer* buffer, pgMapMode mode)
{
	PG_ASSERT_NOT_NULL(buffer);
	return pgMapBufferCore(buffer, mode);
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

	if (buffer->device->state.constantBuffers[slot] == buffer)
		return;

	buffer->device->state.constantBuffers[slot] = buffer;
	pgBindConstantBufferCore(buffer, slot);
}