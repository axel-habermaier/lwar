#include "Prelude.hpp"

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Compile-time size checks of standard data types
//-------------------------------------------------------------------------------------------------------------------------------------------------------
static_assert(sizeof(bool) == 1, "Unexpected size.");
static_assert(sizeof(char) == 1, "Unexpected size.");
static_assert(sizeof(byte) == 1, "Unexpected size.");
static_assert(sizeof(sbyte) == 1, "Unexpected size.");
static_assert(sizeof(uchar) == 1, "Unexpected size.");
static_assert(sizeof(int16) == 2, "Unexpected size.");
static_assert(sizeof(uint16) == 2, "Unexpected size.");
static_assert(sizeof(int32) == 4, "Unexpected size.");
static_assert(sizeof(uint32) == 4, "Unexpected size.");
static_assert(sizeof(int64) == 8, "Unexpected size.");
static_assert(sizeof(uint64) == 8, "Unexpected size.");
static_assert(sizeof(float32) == 4, "Unexpected size.");
static_assert(sizeof(float64) == 8, "Unexpected size.");

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Runtime size checks of interop types
//-------------------------------------------------------------------------------------------------------------------------------------------------------
PG_DECLARE_STRUCTCHECK_FUNC
PG_DECLARE_INTERFACECHECK_FUNC