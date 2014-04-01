#include "prelude.h"

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
	if (device == NULL)
		return;

	pgDestroyGraphicsDeviceCore(device);
	PG_FREE(device);
}

pgVoid pgSetViewport(pgGraphicsDevice* device, pgRectangle viewport)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(viewport.left, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(viewport.top, 0, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_IN_RANGE(viewport.width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(viewport.height, 0, PG_WINDOW_MAX_HEIGHT);

	if (pgRectangleEqual(&viewport, &device->viewport))
		return;

	device->viewport = viewport;
	pgSetViewportCore(device, viewport);
}

pgVoid pgSetScissorArea(pgGraphicsDevice* device, pgRectangle scissorArea)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(scissorArea.left, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(scissorArea.top, 0, PG_WINDOW_MAX_HEIGHT);
	PG_ASSERT_IN_RANGE(scissorArea.width, 0, PG_WINDOW_MAX_WIDTH);
	PG_ASSERT_IN_RANGE(scissorArea.height, 0, PG_WINDOW_MAX_HEIGHT);

	if (pgRectangleEqual(&scissorArea, &device->scissorArea))
		return;

	device->scissorArea = scissorArea;
	pgSetScissorAreaCore(device, scissorArea);
}

pgVoid pgSetPrimitiveType(pgGraphicsDevice* device, pgPrimitiveType primitiveType)
{
	PG_ASSERT_NOT_NULL(device);

	if (device->primitiveType == primitiveType)
		return;

	device->primitiveType = primitiveType;
	pgSetPrimitiveTypeCore(device, primitiveType);
}

pgVoid pgDraw(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(primitiveCount, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(offset, 0, INT32_MAX);

	pgValidateDeviceState(device);
	pgDrawCore(device, primitiveCount, offset);
	
	++device->statistics.drawCalls;
	device->statistics.vertexCount += pgPrimitiveCountToVertexCount(device, primitiveCount);
}

pgVoid pgDrawIndexed(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_IN_RANGE(indexCount, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(indexOffset, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(vertexOffset, 0, INT32_MAX);

	pgValidateDeviceState(device);
	pgDrawIndexedCore(device, indexCount, indexOffset, vertexOffset);

	++device->statistics.drawCalls;
	device->statistics.vertexCount += indexCount;
}

pgVoid pgGetStatistics(pgGraphicsDevice* device, pgStatistics* statistics)
{
	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(statistics);

	*statistics = device->statistics;
	memset(&device->statistics, 0, sizeof(pgStatistics));
}

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

pgVoid pgValidateDeviceState(pgGraphicsDevice* device)
{
	PG_ASSERT_NOT_NULL(device->program);
	PG_ASSERT_NOT_NULL(device->vertexShader);
	PG_ASSERT_NOT_NULL(device->fragmentShader);
	PG_ASSERT_NOT_NULL(device->depthStencilState);
	PG_ASSERT_NOT_NULL(device->blendState);
	PG_ASSERT_NOT_NULL(device->rasterizerState);
	PG_ASSERT_NOT_NULL(device->renderTarget);
	PG_ASSERT_NOT_NULL(device->inputLayout);
	PG_ASSERT(device->primitiveType != 0, "No primitive type has been set.");
	PG_ASSERT(device->viewport.width != 0 && device->viewport.height != 0, "Bound viewport has an area of 0.");
}