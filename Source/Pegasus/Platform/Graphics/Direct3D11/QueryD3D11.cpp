#include "Prelude.hpp"

using namespace Direct3D11;

Query* GraphicsDevice::InitializeQuery(QueryType queryType)
{
	D3D11_QUERY_DESC desc;
	desc.MiscFlags = 0;

	switch (queryType)
	{
	case QueryType::Occlusion:
		desc.Query = D3D11_QUERY_OCCLUSION;
		break;
	case QueryType::Synced:
		desc.Query = D3D11_QUERY_EVENT;
		break;
	case QueryType::TimestampDisjoint:
		desc.Query = D3D11_QUERY_TIMESTAMP_DISJOINT;
		break;
	case QueryType::Timestamp:
		desc.Query = D3D11_QUERY_TIMESTAMP;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	auto query = PG_NEW(Query);
	query->Type = queryType;

	CheckResult(_device->CreateQuery(&desc, &query->Obj), "Failed to create query.");
	return query;
}

void GraphicsDevice::FreeQuery(Query* query)
{
	PG_DELETE(query);
}

void GraphicsDevice::BeginQuery(Query* query)
{
	_context->Begin(query->Obj.Get());
}

void GraphicsDevice::EndQuery(Query* query)
{
	_context->End(query->Obj.Get());
}

bool GraphicsDevice::IsQueryCompleted(Query* query)
{
	HRESULT hr = _context->GetData(query->Obj.Get(), nullptr, 0, 0);
	if (hr != S_OK && hr != S_FALSE)
		PG_DIE("%s", Win32::GetError("Failed to check availability of query result.", static_cast<DWORD>(hr)));

	return hr == S_OK;
}

void GraphicsDevice::GetQueryData(Query* query, void* data)
{
	HRESULT hr;

	switch (query->Type)
	{
	case QueryType::Synced:
		PG_DIE("Not supported.");
		break;
	case QueryType::Timestamp:
		hr = _context->GetData(query->Obj.Get(), data, sizeof(uint64), 0);
		break;
	case QueryType::TimestampDisjoint:
	{
		D3D11_QUERY_DATA_TIMESTAMP_DISJOINT result;
		hr = _context->GetData(query->Obj.Get(), &result, sizeof(D3D11_QUERY_DATA_TIMESTAMP_DISJOINT), 0);

		auto typedData = static_cast<TimestampDisjointQueryResult*>(data);
		typedData->Frequency = result.Frequency;
		typedData->Disjoint = result.Disjoint == TRUE;
		break;
	}
	case QueryType::Occlusion:
		PG_DIE("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_ASSERT(hr == S_OK, "Tried to get data from a query that has not yet been completed.");
}

void GraphicsDevice::SetQueryName(Query* query, const char* name)
{
	SetName(query->Obj, name);
}
