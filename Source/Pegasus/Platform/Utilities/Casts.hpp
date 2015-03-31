#pragma once

// A safe version of static_cast that should be used when casting between differently sized or
// signed/unsigned integer types. In debug builds, checks that the cast actually succeeds. In
// Release builds, equivalent to static_cast.
// See also http://molecularmusings.wordpress.com/2011/08/12/a-safer-static_cast/
template <typename T, typename U>
inline T safe_static_cast(U value);

#include "Casts.inl"