#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Math/Math.hpp"

#ifdef DEBUG
	#include <Initguid.h>
	#include <DXGIDebug.h>
#endif

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// Helper function
//-------------------------------------------------------------------------------------------------------------------------------------------------------
static const D3D_FEATURE_LEVEL RequiredFeatureLevel = D3D_FEATURE_LEVEL_9_3;

static const char* FeatureLevelToString(D3D_FEATURE_LEVEL featureLevel)
{
	switch (featureLevel)
	{
	case D3D_FEATURE_LEVEL_9_1:
		return "9.1";
	case D3D_FEATURE_LEVEL_9_2:
		return "9.2";
	case D3D_FEATURE_LEVEL_9_3:
		return "9.3";
	case D3D_FEATURE_LEVEL_10_0:
		return "10.0";
	case D3D_FEATURE_LEVEL_10_1:
		return "10.1";
	case D3D_FEATURE_LEVEL_11_0:
		return "11.0";
	case D3D_FEATURE_LEVEL_11_1:
		return "11.1";
	default:
		return "> 11.1";
	}
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
// GraphicsDevice
//-------------------------------------------------------------------------------------------------------------------------------------------------------
void GraphicsDevice::Initialize()
{
	D3D_FEATURE_LEVEL featureLevel;
	UINT flags;

	Direct3D11::CheckResult(CreateDXGIFactory(__uuidof(IDXGIFactory), reinterpret_cast<void**>(&_factory)), "Failed to create DXGI factory.");

	if (_factory->EnumAdapters(0, &_adapter) == DXGI_ERROR_NOT_FOUND)
		PG_DIE("Failed to get DXGI adapter.");

	flags = D3D11_CREATE_DEVICE_SINGLETHREADED;
	PG_DEBUG_ONLY(flags |= D3D11_CREATE_DEVICE_DEBUG);

	Direct3D11::CheckResult(
		D3D11CreateDevice(_adapter, D3D_DRIVER_TYPE_UNKNOWN, nullptr, flags, nullptr, 0, D3D11_SDK_VERSION, &_device, &featureLevel, &_context),
		"Failed to create Direct3D 11 device.");

#ifdef DEBUG
	typedef HRESULT(__stdcall *ptr)(const IID&, void**);
	HMODULE debugDll = GetModuleHandleW(L"dxgidebug.dll");
	PG_ASSERT(debugDll != nullptr, "Failed to load dxgidebug.dll");

	auto DXGIGetDebugInterface = reinterpret_cast<ptr>(static_cast<void*>(GetProcAddress(debugDll, "DXGIGetDebugInterface")));
	PG_ASSERT(DXGIGetDebugInterface != nullptr, "Failed to get DXGIGetDebugInterface().");

	Direct3D11::CheckResult(DXGIGetDebugInterface(__uuidof(IDXGIDebug), reinterpret_cast<void**>(&_debug)),
							"Failed to get the Direct3D 11 debug interface.");
#endif

	if (featureLevel < RequiredFeatureLevel)
	{
		PG_DIE("Incompatible graphics card: Only feature level {0} is supported, but feature level {1} is required.",
			   FeatureLevelToString(featureLevel), FeatureLevelToString(RequiredFeatureLevel));
	}
}

void GraphicsDevice::Deinitialize()
{
	if (_context != nullptr)
	{
		_context->ClearState();
		_context->Flush();
		_context->Release();
	}

	Direct3D11::Release(_device);
	Direct3D11::Release(_adapter);

#ifdef DEBUG
	if (_debug != nullptr)
		_debug->ReportLiveObjects(DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_ALL);

	Direct3D11::Release(_debug);
#endif

	Direct3D11::Release(_factory);
}

void GraphicsDevice::PrintDeviceInfo()
{
	DXGI_ADAPTER_DESC _adapterDesc;
	char buffer[sizeof(_adapterDesc.Description)];
	auto featureLevel = FeatureLevelToString(_device->GetFeatureLevel());

	_adapter->GetDesc(&_adapterDesc);
	wcstombs(buffer, _adapterDesc.Description, sizeof(buffer));

	PG_INFO("Direct3D renderer: {0}", buffer);
	PG_INFO("Direct3D feature level: {0}", featureLevel);
}

void GraphicsDevice::SetPrimitiveType(PrimitiveType primitiveType)
{
	if (ChangeState(&_boundPrimitiveType, primitiveType))
		_context->IASetPrimitiveTopology(Direct3D11::Map(primitiveType));
}

void GraphicsDevice::SetViewport(const Rect& viewport)
{
	if (!ChangeState(&_boundViewport, viewport))
		return;

	D3D11_VIEWPORT vp;
	vp.TopLeftX = viewport.Left();
	vp.TopLeftY = viewport.Top();
	vp.Width = viewport.Width();
	vp.Height = viewport.Height();
	vp.MaxDepth = 1.0f;
	vp.MinDepth = 0.0f;

	_context->RSSetViewports(1, &vp);
}

void GraphicsDevice::SetScissorArea(const Rect& scissorArea)
{
	if (!ChangeState(&_boundScissorArea, scissorArea))
		return;

	D3D11_RECT rect;
	rect.left = Math::ToInt32(scissorArea.Left());
	rect.top = Math::ToInt32(scissorArea.Top());
	rect.right = Math::ToInt32(scissorArea.Left() + scissorArea.Width());
	rect.bottom = Math::ToInt32(scissorArea.Top() + scissorArea.Height());

	_context->RSSetScissorRects(1, &rect);
}

void GraphicsDevice::Draw(uint32 primitiveCount, uint32 vertexOffset)
{
	ValidateState();
	_context->Draw(GetVertexCount(primitiveCount), vertexOffset);
}

void GraphicsDevice::DrawIndexed(uint32 indexCount, uint32 indexOffset, int32 vertexOffset)
{
	ValidateState();
	_context->DrawIndexed(indexCount, indexOffset, vertexOffset);
}

void GraphicsDevice::DrawInstanced(uint32 instanceCount, uint32 primitiveCount, uint32 vertexOffset, uint32 instanceOffset)
{
	ValidateState();
	_context->DrawInstanced(GetVertexCount(primitiveCount), instanceCount, vertexOffset, instanceOffset);
}

void GraphicsDevice::DrawIndexedInstanced(uint32 instanceCount, uint32 indexCount, uint32 indexOffset, int32 vertexOffset, uint32 instanceOffset)
{
	ValidateState();
	_context->DrawIndexedInstanced(indexCount, instanceCount, indexOffset, vertexOffset, instanceOffset);
}

#endif