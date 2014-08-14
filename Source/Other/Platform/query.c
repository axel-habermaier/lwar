#include "prelude.h"

//====================================================================================================================
// Exported functions
//====================================================================================================================

pgQuery* pgCreateQuery(pgGraphicsDevice* device, pgQueryType type)
{
	pgQuery* query;
	PG_ASSERT_NOT_NULL(device);

	PG_ALLOC(pgQuery, query);
	query->device = device;
	query->type = type;
	query->isActive = PG_FALSE;
	pgCreateQueryCore(query);

	return query;
}

pgVoid pgDestroyQuery(pgQuery* query)
{
	if (query == NULL)
		return;

	pgDestroyQueryCore(query);
	PG_FREE(query);
}

pgVoid pgBeginQuery(pgQuery* query)
{
	PG_ASSERT_NOT_NULL(query);
	PG_ASSERT(!query->isActive, "Query is already active.");
	PG_ASSERT(query->type != PG_TIMESTAMP_QUERY, "pgBeginQuery is not supported for PG_TIMESTAMP_QUERY.");
	PG_ASSERT(query->type != PG_SYNCED_QUERY, "pgBeginQuery is not supported for PG_SYNCED_QUERY.");

	query->isActive = PG_TRUE;
	pgBeginQueryCore(query);
}

pgVoid pgEndQuery(pgQuery* query)
{
	PG_ASSERT_NOT_NULL(query);
	PG_ASSERT(query->type == PG_TIMESTAMP_QUERY || query->type == PG_SYNCED_QUERY || query->isActive, "Query is currently not active.");

	pgEndQueryCore(query);
	query->isActive = PG_FALSE;
}

pgVoid pgGetQueryData(pgQuery* query, pgVoid* data, pgInt32 size)
{
	PG_ASSERT_NOT_NULL(query);
	PG_ASSERT_NOT_NULL(data);
	PG_ASSERT_IN_RANGE(size, 0, INT32_MAX);
	PG_ASSERT(!query->isActive, "Cannot get the data of a currently inactive query.");
	PG_ASSERT(query->type != PG_TIMESTAMP_QUERY || size == sizeof(pgUint64), "Invalid data type.");
	PG_ASSERT(query->type != PG_TIMESTAMP_DISJOINT_QUERY || size == sizeof(pgTimestampDisjointQueryData), "Invalid data type.");
	PG_ASSERT(query->type != PG_SYNCED_QUERY, "PG_SYNCED_QUERY does not return any data.");

	pgGetQueryDataCore(query, data, size);
}

pgBool pgIsQueryDataAvailable(pgQuery* query)
{
	PG_ASSERT_NOT_NULL(query);
	PG_ASSERT(!query->isActive, "No data is available for a currently inactive query.");

	return pgIsQueryDataAvailableCore(query);
}
