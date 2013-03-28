#include "prelude.h"

#define PG_ASSERT_SIZE(type, size)	PG_COMPILE_TIME_ASSERT(type, sizeof(type) == size);

PG_ASSERT_SIZE(pgByte, 1);
PG_ASSERT_SIZE(pgChar, 1);
PG_ASSERT_SIZE(pgUint8,	1);
PG_ASSERT_SIZE(pgInt8, 1);
PG_ASSERT_SIZE(pgUint16, 2);
PG_ASSERT_SIZE(pgInt16,	2);
PG_ASSERT_SIZE(pgUint32, 4);
PG_ASSERT_SIZE(pgInt32,	4);
PG_ASSERT_SIZE(pgUint64, 8);
PG_ASSERT_SIZE(pgInt64, 8);
PG_ASSERT_SIZE(pgFloat32, 4);
PG_ASSERT_SIZE(pgFloat64, 8);
PG_ASSERT_SIZE(pgBool, 4);

// Enumeration types are expected to be 4 bytes long
PG_ASSERT_SIZE(pgKey, 4);
PG_ASSERT_SIZE(pgMouseButton, 4);
PG_ASSERT_SIZE(pgGraphicsApi, 4);
PG_ASSERT_SIZE(pgBlendOperation, 4);
PG_ASSERT_SIZE(pgBlendOption, 4);
PG_ASSERT_SIZE(pgColorWriteChannels, 4);
PG_ASSERT_SIZE(pgComparison, 4);
PG_ASSERT_SIZE(pgCullMode, 4);
PG_ASSERT_SIZE(pgFillMode, 4);
PG_ASSERT_SIZE(pgIndexSize, 4);
PG_ASSERT_SIZE(pgMapMode, 4);
PG_ASSERT_SIZE(pgPrimitiveType, 4);
PG_ASSERT_SIZE(pgResourceUsage, 4);
PG_ASSERT_SIZE(pgStencilOperation, 4);
PG_ASSERT_SIZE(pgSurfaceFormat, 4);
PG_ASSERT_SIZE(pgTextureAddressMode, 4);
PG_ASSERT_SIZE(pgTextureFilter, 4);
PG_ASSERT_SIZE(pgVertexDataFormat, 4);
PG_ASSERT_SIZE(pgDataSemantics, 4);
PG_ASSERT_SIZE(pgTextureType, 4);
PG_ASSERT_SIZE(pgQueryType, 4);
PG_ASSERT_SIZE(pgBufferType, 4);
PG_ASSERT_SIZE(pgShaderType, 4);
PG_ASSERT_SIZE(pgTextureFlags, 4);
PG_ASSERT_SIZE(pgAttachmentPoint, 4);
