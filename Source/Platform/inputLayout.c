#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgInputLayout* pgCreateInputLayout(pgGraphicsDevice* device, pgBuffer* indexBuffer, pgInt32 indexOffset, 
								   pgIndexSize indexSize, pgInputBinding* inputBindings, pgInt32 bindingsCount)
{
	pgInputLayout* layout;

	PG_ASSERT_NOT_NULL(device);
	PG_ASSERT_NOT_NULL(inputBindings);
	PG_ASSERT_IN_RANGE(indexOffset, 0, INT32_MAX);
	PG_ASSERT_IN_RANGE(bindingsCount, 0, PG_INPUT_BINDINGS_COUNT);

	PG_ALLOC(pgInputLayout, layout);
	layout->device = device;
	pgCreateInputLayoutCore(layout, indexBuffer, indexOffset, indexSize, inputBindings, bindingsCount);

	return layout;
}

pgVoid pgDestroyInputLayout(pgInputLayout* inputLayout)
{
	PG_ASSERT_NOT_NULL(inputLayout);

	pgDestroyInputLayoutCore(inputLayout);
	PG_FREE(inputLayout);
}

pgVoid pgBindInputLayout(pgInputLayout* inputLayout)
{
	PG_ASSERT_NOT_NULL(inputLayout);
	pgBindInputLayoutCore(inputLayout);
}