#include "prelude.h"

#ifdef OPENGL3

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateQueryCore(pgQuery* query)
{
	if (query->type == PG_TIMESTAMP_DISJOINT_QUERY)
		return;

	PG_GL_ALLOC("Query", glGenQueries, query->id);
}

pgVoid pgDestroyQueryCore(pgQuery* query)
{
	if (query->type == PG_TIMESTAMP_DISJOINT_QUERY)
		return;

	PG_GL_FREE(glDeleteQueries, query->id);
}

pgVoid pgBeginQueryCore(pgQuery* query)
{
	switch (query->type)
	{
	case PG_TIMESTAMP_QUERY:
		pgDie("Not supported.");
		break;
	case PG_TIMESTAMP_DISJOINT_QUERY:
		// Not required by OpenGL
		break;
	case PG_OCCLUSION_QUERY:
		pgDie("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_ASSERT_NO_GL_ERRORS();
}	

pgVoid pgEndQueryCore(pgQuery* query)
{
	switch (query->type)
	{
	case PG_TIMESTAMP_QUERY:
		glQueryCounter(query->id, GL_TIMESTAMP);
		break;
	case PG_TIMESTAMP_DISJOINT_QUERY:
		// Not required by OpenGL
		break;
	case PG_OCCLUSION_QUERY:
		pgDie("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_ASSERT_NO_GL_ERRORS();
}

pgVoid pgGetQueryDataCore(pgQuery* query, pgVoid* data, pgInt32 size)
{
	PG_UNUSED(size);

	switch (query->type)
	{
	case PG_TIMESTAMP_QUERY:
	{
		GLuint64* result = (GLuint64*)data;
		glGetQueryObjectui64v(query->id, GL_QUERY_RESULT, result);
		break;
	}
	case PG_TIMESTAMP_DISJOINT_QUERY:
	{
		pgTimestampDisjointQueryData* result = (pgTimestampDisjointQueryData*)data;
		result->frequency = 1000000000;
		result->valid = PG_TRUE;
		break;
	}
	case PG_OCCLUSION_QUERY:
		pgDie("Not implemented.");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	PG_ASSERT_NO_GL_ERRORS();
}

pgBool pgIsQueryDataAvailableCore(pgQuery* query)
{
	GLint available;

	if (query->type == PG_TIMESTAMP_DISJOINT_QUERY)
		return PG_TRUE;

	glGetQueryObjectiv(query->id, GL_QUERY_RESULT_AVAILABLE, &available);
	PG_ASSERT_NO_GL_ERRORS();

	return available == GL_TRUE;
}

#endif