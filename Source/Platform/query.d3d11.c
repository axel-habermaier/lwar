#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateQueryCore(pgQuery* query)
{
	D3D11_QUERY_DESC desc;
	desc.MiscFlags = 0;
	desc.Query = pgConvertQueryType(query->type);

	PG_D3DCALL(ID3D11Device_CreateQuery(PG_DEVICE(query), &desc, &query->ptr), "Failed to create query.");
}

pgVoid pgDestroyQueryCore(pgQuery* query)
{
	PG_SAFE_RELEASE(ID3D11Query, query->ptr);
}

pgVoid pgBeginQueryCore(pgQuery* query)
{
	ID3D11DeviceContext_Begin(PG_CONTEXT(query), (ID3D11Asynchronous*)query->ptr);
}

pgVoid pgEndQueryCore(pgQuery* query)
{
	ID3D11DeviceContext_End(PG_CONTEXT(query), (ID3D11Asynchronous*)query->ptr);
}

pgVoid pgGetQueryDataCore(pgQuery* query, pgVoid* data, pgInt32 size)
{
	HRESULT hr;
	
	switch (query->type)
	{
	case PG_TIMESTAMP_QUERY:
		hr = ID3D11DeviceContext_GetData(PG_CONTEXT(query), (ID3D11Asynchronous*)query->ptr, data, size, 0);
		break;
	case PG_TIMESTAMP_DISJOINT_QUERY:
	{
		D3D11_QUERY_DATA_TIMESTAMP_DISJOINT result;
		pgTimestampDisjointQueryData* pgResult = (pgTimestampDisjointQueryData*)data;

		hr = ID3D11DeviceContext_GetData(PG_CONTEXT(query), (ID3D11Asynchronous*)query->ptr, &result, sizeof(D3D11_QUERY_DATA_TIMESTAMP_DISJOINT), 0);
		pgResult->frequency = result.Frequency;
		pgResult->valid = !result.Disjoint;
		break;
	}
	case PG_OCCLUSION_QUERY:
		pgDie("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
	
	PG_ASSERT(hr == S_OK, "Tried to get data from a query that has not yet completed.");
}

pgBool pgIsQueryDataAvailableCore(pgQuery* query)
{
	HRESULT hr = ID3D11DeviceContext_GetData(PG_CONTEXT(query), (ID3D11Asynchronous*)query->ptr, NULL, 0, 0);
	if (hr != S_OK && hr != S_FALSE)
		pgDieWin32Error("Failed to check availability of query result.", hr);

	return hr == S_OK;
}

#endif