#pragma once

namespace Graphics
{
	static const int32 ConstantBufferSlotCount = 14;
	static const int32 TextureSlotCount = 16;
	static const int32 MaxMipmaps = 16;
	static const int32 MaxTextureSize = 8192;
	static const int32 MaxSurfaceCount = MaxMipmaps * 6 * 3;
	static const int32 MaxColorBuffers = 4;
	static const int32 MaxVertexBindings = 8;

	extern bool VsyncEnabled;

	// Updates the given state, if necessary, returning true to indicate that the state has indeed been changed.
	template <typename T>
	bool ChangeState(T* stateValue, const T value);

	bool IsCompressedFormat(SurfaceFormat format);
	bool IsFloatingPointFormat(SurfaceFormat format);
	bool IsDepthStencilFormat(SurfaceFormat format);
}

#include "Graphics.inl"