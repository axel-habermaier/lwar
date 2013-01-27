#include "prelude.h"

//====================================================================================================================
// Helper functions
//====================================================================================================================

pgInt32 pgPrimitiveCountToVertexCount(pgGraphicsDevice* device, pgInt32 primitiveCount)
{
	switch (device->primitiveType)
	{
	case PG_PRIMITIVE_TRIANGLES:
		return primitiveCount * 3;
	case PG_PRIMITIVE_TRIANGLESTRIP:
		return primitiveCount + 2;
	case PG_PRIMITIVE_LINES:
		return primitiveCount * 2;
	case PG_PRIMITIVE_LINESTRIP:
		return primitiveCount + 1;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgGraphicsDevice* pgCreateGraphicsDevice()
{
	pgGraphicsDevice* device;
	PG_ALLOC(pgGraphicsDevice, device);

	pgCreateGraphicsDeviceCore(device);
	pgPrintDeviceInfoCore(device);
	return device;
}

pgVoid pgDestroyGraphicsDevice(pgGraphicsDevice* device)
{
	PG_ASSERT_NOT_NULL(device);

	pgDestroyGraphicsDeviceCore(device);
	PG_FREE(device);
}

pgVoid pgSetViewport(pgGraphicsDevice* device, pgInt32 left, pgInt32 top, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(left, 0, 4096);
	PG_ASSERT_IN_RANGE(top, 0, 4096);
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

	pgSetViewportCore(device, left, top, width, height);
}

pgVoid pgSetScissorRect(pgGraphicsDevice* device, pgInt32 left, pgInt32 top, pgInt32 width, pgInt32 height)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(left, 0, 4096);
	PG_ASSERT_IN_RANGE(top, 0, 4096);
	PG_ASSERT_IN_RANGE(width, 0, 4096);
	PG_ASSERT_IN_RANGE(height, 0, 4096);

	pgSetScissorRectCore(device, left, top, width, height);
}

pgVoid pgSetPrimitiveType(pgGraphicsDevice* device, pgPrimitiveType primitiveType)
{
	PG_ASSERT_NOT_NULL(device);

	device->primitiveType = primitiveType;
	pgSetPrimitiveTypeCore(device, primitiveType);
}

pgVoid pgDraw(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(primitiveCount, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(offset, 0, INT32_MAX);

	pgDrawCore(device, primitiveCount, offset);
}

pgVoid pgDrawIndexed(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(indexCount, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(indexOffset, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(vertexOffset, 0, INT32_MAX);

	pgDrawIndexedCore(device, indexCount, indexOffset, vertexOffset);
}