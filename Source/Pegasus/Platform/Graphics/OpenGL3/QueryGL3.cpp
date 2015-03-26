#include "Prelude.hpp"

using namespace OpenGL3;

Query* GraphicsDevice::InitializeQuery(QueryType queryType)
{
	auto query = PG_NEW(Query);
	query->Type = queryType;

	switch (query->Type)
	{
	case QueryType::Timestamp:
		query->Obj = Allocate(&GraphicsDevice::glGenQueries, "TimestampQuery");
		break;
	case QueryType::TimestampDisjoint:
	case QueryType::Occlusion:
	case QueryType::Synced:
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	return query;
}

void GraphicsDevice::FreeQuery(Query* query)
{
	if (query == nullptr)
		return;

	switch (query->Type)
	{
	case QueryType::Occlusion:
		break;
	case QueryType::Synced:
		glDeleteSync(query->Sync);
		break;
	case QueryType::TimestampDisjoint:
		break;
	case QueryType::Timestamp:
		glDeleteQueries(1, &query->Obj);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_DELETE(query);
}

void GraphicsDevice::BeginQuery(Query* query)
{
	switch (query->Type)
	{
	case QueryType::TimestampDisjoint:
		break;
	case QueryType::Timestamp:
	case QueryType::Synced:
	case QueryType::Occlusion:
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

void GraphicsDevice::EndQuery(Query* query)
{
	switch (query->Type)
	{
	case QueryType::Occlusion:
		break;
	case QueryType::Synced:
		glDeleteSync(query->Sync);
		query->Sync = glFenceSync(GL_SYNC_GPU_COMMANDS_COMPLETE, 0);
		break;
	case QueryType::TimestampDisjoint:
		break;
	case QueryType::Timestamp:
		glQueryCounter(query->Obj, GL_TIMESTAMP);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

bool GraphicsDevice::IsQueryCompleted(Query* query)
{
	switch (query->Type)
	{
	case QueryType::Occlusion:
		return false;
	case QueryType::TimestampDisjoint:
		return true;
	case QueryType::Synced:
	{
		PG_ASSERT(query->Sync != nullptr, "Sync failed or no sync point has been marked.");

		auto result = glClientWaitSync(query->Sync, GL_SYNC_FLUSH_COMMANDS_BIT, 0);
		return result == GL_CONDITION_SATISFIED || result == GL_ALREADY_SIGNALED;
	}
	case QueryType::Timestamp:
	{
		int32 available;
		glGetQueryObjectiv(query->Obj, GL_QUERY_RESULT_AVAILABLE, &available);

		return available == GL_TRUE;
	}
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

void GraphicsDevice::GetQueryData(Query* query, void* data)
{
	switch (query->Type)
	{
	case QueryType::Occlusion:
		break;
	case QueryType::TimestampDisjoint:
	{
		auto typedData = static_cast<TimestampDisjointQueryResult*>(data);
		typedData->Frequency = 1000000000;
		typedData->Disjoint = false;
		break;
	}
	case QueryType::Timestamp:
		glGetQueryObjectui64v(query->Obj, GL_QUERY_RESULT, static_cast<uint64*>(data));
		break;
	case QueryType::Synced:
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

void GraphicsDevice::SetQueryName(Query* query, const char* name)
{
	PG_UNUSED(query);
	PG_UNUSED(name);
}