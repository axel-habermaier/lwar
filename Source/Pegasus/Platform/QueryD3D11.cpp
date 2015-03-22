#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Graphics/Query.hpp"

void GraphicsDevice::Initialize(Query* query)
{
	D3D11_QUERY_DESC desc;
	desc.MiscFlags = 0;

	switch (query->_type)
	{
	case QueryType::OcclusionQuery:
		desc.Query = D3D11_QUERY_OCCLUSION;
		break;
	case QueryType::SyncedQuery:
		desc.Query = D3D11_QUERY_EVENT;
		break;
	case QueryType::TimestampDisjointQuery:
		desc.Query = D3D11_QUERY_TIMESTAMP_DISJOINT;
		break;
	case QueryType::TimestampQuery:
		desc.Query = D3D11_QUERY_TIMESTAMP;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	Direct3D11::CheckResult(_device->CreateQuery(&desc, &query->_query), "Failed to create query.");
}

void GraphicsDevice::Free(Query* query)
{
	Direct3D11::Release(query->_query);
}

void GraphicsDevice::Begin(Query* query)
{
	_context->Begin(query->_query);
}

void GraphicsDevice::End(Query* query)
{
	_context->End(query->_query);
}

bool GraphicsDevice::IsCompleted(Query* query)
{
	HRESULT hr = _context->GetData(query->_query, nullptr, 0, 0);
	if (hr != S_OK && hr != S_FALSE)
		PG_DIE("{0}", Win32::GetError("Failed to check availability of query result.", static_cast<DWORD>(hr)));

	return hr == S_OK;
}

void GraphicsDevice::GetData(Query* query, void* data)
{
	HRESULT hr;

	switch (query->_type)
	{
	case QueryType::SyncedQuery:
		PG_DIE("Not supported.");
		break;
	case QueryType::TimestampQuery:
		hr = _context->GetData(query->_query, data, sizeof(uint64), 0);
		break;
	case QueryType::TimestampDisjointQuery:
	{
		D3D11_QUERY_DATA_TIMESTAMP_DISJOINT result;
		auto typedData = static_cast<TimestampDisjointQuery::Result*>(data);

		hr = _context->GetData(query->_query, &result, sizeof(D3D11_QUERY_DATA_TIMESTAMP_DISJOINT), 0);

		typedData->Frequency = result.Frequency;
		typedData->Disjoint = result.Disjoint == TRUE;
		break;
	}
	case QueryType::OcclusionQuery:
		PG_DIE("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_ASSERT(hr == S_OK, "Tried to get data from a query that has not yet completed.");
}

void GraphicsDevice::SetName(Query* query, const char* name)
{
	Direct3D11::SetName(query->_query, name);
}

#endif