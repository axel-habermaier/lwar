#include "Prelude.hpp"

using namespace Direct3D11;

#ifdef DEBUG
	#include <Initguid.h>
	#include <DXGIDebug.h>
#endif

PG_DECLARE_DEVICEINTERFACE_API(GraphicsDevice, D3D11)

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Helper function
//-------------------------------------------------------------------------------------------------------------------------------------------------------

static std::ostream& operator << (std::ostream& s, D3D_FEATURE_LEVEL featureLevel)
{
	switch (featureLevel)
	{
	case D3D_FEATURE_LEVEL_9_1:
		return s << "9.1";
	case D3D_FEATURE_LEVEL_9_2:
		return s << "9.2";
	case D3D_FEATURE_LEVEL_9_3:
		return s << "9.3";
	case D3D_FEATURE_LEVEL_10_0:
		return s << "10.0";
	case D3D_FEATURE_LEVEL_10_1:
		return s << "10.1";
	case D3D_FEATURE_LEVEL_11_0:
		return s << "11.0";
	case D3D_FEATURE_LEVEL_11_1:
		return s << "11.1";
	default:
		return s << "> 11.1";
	}
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// GraphicsDevice
//-------------------------------------------------------------------------------------------------------------------------------------------------------
GraphicsDevice::GraphicsDevice(DeviceInterface* deviceInterface)
{
	PG_ASSERT_NOT_NULL(deviceInterface);

	deviceInterface->_this = this;
	PG_INITIALIZE_DEVICEINTERFACE(GraphicsDevice, D3D11, deviceInterface);

	CheckResult(CreateDXGIFactory(__uuidof(IDXGIFactory), reinterpret_cast<void**>(_factory.GetAddressOf())), "Failed to create DXGI factory.");

	if (_factory->EnumAdapters(0, &_adapter) == DXGI_ERROR_NOT_FOUND)
		PG_DIE("Failed to get DXGI adapter.");

	auto flags = D3D11_CREATE_DEVICE_SINGLETHREADED;
	PG_DEBUG_ONLY(flags |= D3D11_CREATE_DEVICE_DEBUG);

	D3D_FEATURE_LEVEL featureLevel;
	CheckResult(
		D3D11CreateDevice(_adapter.Get(), D3D_DRIVER_TYPE_UNKNOWN, nullptr, flags, nullptr, 0, D3D11_SDK_VERSION, &_device, &featureLevel, &_context),
		"Failed to create the Direct3D 11 device.");

#ifdef DEBUG
	const auto debugDllName = "dxgidebug.dll";
	const auto funcName = "DXGIGetDebugInterface";
	using DXGIGetDebugInterfaceFunc = HRESULT(__stdcall *)(const IID&, void**);

	HMODULE debugDll = GetModuleHandle(debugDllName);
	PG_ASSERT(debugDll != nullptr, "Failed to load '%s'.", debugDllName);

	auto DXGIGetDebugInterface = reinterpret_cast<DXGIGetDebugInterfaceFunc>(static_cast<void*>(GetProcAddress(debugDll, funcName)));
	PG_ASSERT(DXGIGetDebugInterface != nullptr, "Failed to get %s().", funcName);

	CheckResult(DXGIGetDebugInterface(__uuidof(IDXGIDebug), reinterpret_cast<void**>(_debug.GetAddressOf())), "Failed to instantiate IDXGIDebug.");
#endif

	if (featureLevel < RequiredFeatureLevel)
		PG_DIE("Incompatible graphics card: Only feature level %s is supported, but feature level %s is required.", featureLevel, RequiredFeatureLevel);
}

GraphicsDevice::~GraphicsDevice()
{
	if (_context != nullptr)
	{
		_context->ClearState();
		_context->Flush();
		_context.Reset();
	}

	_device.Reset();

#ifdef DEBUG
	if (_debug != nullptr)
		_debug->ReportLiveObjects(DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_ALL);
#endif
}

void GraphicsDevice::PrintDeviceInfo()
{
	DXGI_ADAPTER_DESC _adapterDesc;
	char buffer[sizeof(_adapterDesc.Description)];

	_adapter->GetDesc(&_adapterDesc);
	wcstombs(buffer, _adapterDesc.Description, sizeof(buffer));

	PG_INFO("Direct3D renderer: %s", buffer);
	PG_INFO("Direct3D feature level: %s", _device->GetFeatureLevel());
}

void GraphicsDevice::SetPrimitiveType(PrimitiveType primitiveType)
{
	_context->IASetPrimitiveTopology(Map(primitiveType));
}

void GraphicsDevice::SetViewport(int32 left, int32 top, int32 width, int32 height)
{
	D3D11_VIEWPORT vp;
	vp.TopLeftX = static_cast<float32>(left);
	vp.TopLeftY = static_cast<float32>(top);
	vp.Width = static_cast<float32>(width);
	vp.Height = static_cast<float32>(height);
	vp.MaxDepth = 1.0f;
	vp.MinDepth = 0.0f;

	_context->RSSetViewports(1, &vp);
}

void GraphicsDevice::SetScissorArea(int32 left, int32 top, int32 width, int32 height)
{
	D3D11_RECT rect;
	rect.left = left;
	rect.top = top;
	rect.right = left + width;
	rect.bottom = top + height;

	_context->RSSetScissorRects(1, &rect);
}

void GraphicsDevice::Draw(int32 vertexCount, int32 vertexOffset)
{
	_context->Draw(safe_static_cast<UINT>(vertexCount), safe_static_cast<UINT>(vertexOffset));
}

void GraphicsDevice::DrawIndexed(int32 indexCount, int32 indexOffset, int32 vertexOffset)
{
	_context->DrawIndexed(safe_static_cast<UINT>(indexCount), safe_static_cast<UINT>(indexOffset), vertexOffset);
}

void GraphicsDevice::DrawInstanced(int32 instanceCount, int32 vertexCount, int32 vertexOffset, int32 instanceOffset)
{
	_context->DrawInstanced(
		safe_static_cast<UINT>(vertexCount),
		safe_static_cast<UINT>(instanceCount),
		safe_static_cast<UINT>(vertexOffset),
		safe_static_cast<UINT>(instanceOffset));
}

void GraphicsDevice::DrawIndexedInstanced(int32 instanceCount, int32 indexCount, int32 indexOffset, int32 vertexOffset, int32 instanceOffset)
{
	_context->DrawIndexedInstanced(
		safe_static_cast<UINT>(indexCount),
		safe_static_cast<UINT>(instanceCount),
		safe_static_cast<UINT>(indexOffset),
		vertexOffset,
		safe_static_cast<UINT>(instanceOffset));
}
