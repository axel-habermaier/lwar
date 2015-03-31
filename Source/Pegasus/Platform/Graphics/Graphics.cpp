#include "Prelude.hpp"

PG_API_EXPORT DeviceInterface* CreateDeviceInterface(GraphicsApi graphicsApi)
{
	DeviceInterface* deviceInterface = nullptr;

	switch (graphicsApi)
	{
	case GraphicsApi::Direct3D11:
		PG_WINDOWS_ONLY(deviceInterface = PG_NEW(DeviceInterface));
		PG_WINDOWS_ONLY(PG_NEW(Direct3D11::GraphicsDevice, deviceInterface););
		break;
	case GraphicsApi::OpenGL3:
		deviceInterface = PG_NEW(DeviceInterface);
		PG_NEW(OpenGL3::GraphicsDevice, deviceInterface);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	if (deviceInterface != nullptr)
		deviceInterface->GraphicsApi = graphicsApi;

	return deviceInterface;
}

PG_API_EXPORT void FreeDeviceInterface(DeviceInterface* deviceInterface)
{
	if (deviceInterface == nullptr)
		return;

	switch (deviceInterface->GraphicsApi)
	{
	case GraphicsApi::Direct3D11:
		PG_WINDOWS_ONLY(PG_DELETE(static_cast<Direct3D11::GraphicsDevice*>(deviceInterface->_this)););
		PG_WINDOWS_ONLY(PG_DELETE(deviceInterface));
		break;
	case GraphicsApi::OpenGL3:
		PG_DELETE(static_cast<OpenGL3::GraphicsDevice*>(deviceInterface->_this));
		PG_DELETE(deviceInterface);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

PG_API_EXPORT int32 GetConstantBufferSlotCount()
{
	return Graphics::ConstantBufferSlotCount;
}

PG_API_EXPORT int32 GetTextureSlotCount()
{
	return Graphics::TextureSlotCount;
}

PG_API_EXPORT int32 GetMaxColorBuffers()
{
	return Graphics::MaxColorBuffers;
}

PG_API_EXPORT int32 GetMaxMipmaps()
{
	return Graphics::MaxMipmaps;
}

PG_API_EXPORT int32 GetMaxTextureSize()
{
	return Graphics::MaxTextureSize;
}

PG_API_EXPORT int32 GetMaxSurfaceCount()
{
	return Graphics::MaxSurfaceCount;
}

PG_API_EXPORT int32 GetMaxVertexBindings()
{
	return Graphics::MaxVertexBindings;
}

bool Graphics::IsCompressedFormat(SurfaceFormat format)
{
	return format == SurfaceFormat::Bc1 || format == SurfaceFormat::Bc2 || format == SurfaceFormat::Bc3 ||
		format == SurfaceFormat::Bc4 || format == SurfaceFormat::Bc5;
}

bool Graphics::IsFloatingPointFormat(SurfaceFormat format)
{
	return format == SurfaceFormat::Rgba16F;
}

bool Graphics::IsDepthStencilFormat(SurfaceFormat format)
{
	return format == SurfaceFormat::Depth24Stencil8;
}