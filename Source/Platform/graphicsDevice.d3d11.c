#include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Helper functions 
//====================================================================================================================

#define REQUIRED_FEATURE_LEVEL D3D_FEATURE_LEVEL_10_0

pgString FeatureLevelToString(D3D_FEATURE_LEVEL featureLevel)
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
	default:
		return "> 11.0";
	}
}

void* pgInitializeDebug();
void pgReportLiveObjects(IUnknown* debugObject);

//====================================================================================================================
// Core functions 
//====================================================================================================================

pgVoid pgCreateGraphicsDeviceCore(pgGraphicsDevice* device)
{
	D3D_FEATURE_LEVEL featureLevel;
	UINT flags;

	PG_D3DCALL(CreateDXGIFactory(&IID_IDXGIFactory, &device->factory), "Failed to create DXGI factory.");
	if (IDXGIFactory_EnumAdapters(device->factory, 0, &device->adapter) == DXGI_ERROR_NOT_FOUND)
		PG_DIE("Failed to get DXGI adapter.");
	
	flags = D3D11_CREATE_DEVICE_SINGLETHREADED;
#ifdef DEBUG
	flags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

	PG_D3DCALL(D3D11CreateDevice(device->adapter, D3D_DRIVER_TYPE_UNKNOWN, NULL, flags, NULL, 0, D3D11_SDK_VERSION, &device->ptr, &featureLevel, &device->context),
		"Failed to create Direct3D 11 device.");

#ifdef DEBUG
	typedef HRESULT(__stdcall *debugInterfacePtr)(REFIID, void**);
	HMODULE debugDll = GetModuleHandleW(L"dxgidebug.dll");
	PG_ASSERT(debugDll != NULL, "Failed to load dxgidebug.dll");

	debugInterfacePtr DXGIGetDebugInterface = (debugInterfacePtr)GetProcAddress(debugDll, "DXGIGetDebugInterface");
	PG_ASSERT(DXGIGetDebugInterface != NULL, "Failed to get DXGIGetDebugInterface().");

	PG_D3DCALL(DXGIGetDebugInterface(&IID_IDXGIDebug, (void**)(&device->debug)), "Failed to get the Direct3D 11 debug interface.");
#endif

	if (featureLevel < REQUIRED_FEATURE_LEVEL)
		PG_DIE("Incompatible graphics card: Only feature level %s is supported, but feature level %s is required.", 
			FeatureLevelToString(featureLevel), FeatureLevelToString(REQUIRED_FEATURE_LEVEL));
}

pgVoid pgDestroyGraphicsDeviceCore(pgGraphicsDevice* device)
{
	if (device->context != NULL)
	{
		ID3D11DeviceContext_ClearState(device->context);
		ID3D11DeviceContext_Flush(device->context);
		ID3D11DeviceContext_Release(device->context);
	}

	PG_SAFE_RELEASE(IDXGIAdapter, device->adapter);
	PG_SAFE_RELEASE(ID3D11Device, device->ptr);

#ifdef DEBUG
	if (device->debug != NULL)
		IDXGIDebug_ReportLiveObjects((IDXGIDebug*)device->debug, DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_ALL);

	PG_SAFE_RELEASE(IDXGIDebug, device->debug);
#endif

	PG_SAFE_RELEASE(IDXGIFactory, device->factory);
}

pgVoid pgSetViewportCore(pgGraphicsDevice* device, pgRectangle viewport)
{
	D3D11_VIEWPORT vp;
	vp.TopLeftX = (FLOAT)viewport.left;
	vp.TopLeftY = (FLOAT)viewport.top;
	vp.Width = (FLOAT)viewport.width;
	vp.Height = (FLOAT)viewport.height;
	vp.MaxDepth = 1;
	vp.MinDepth = 0;

	ID3D11DeviceContext_RSSetViewports(device->context, 1, &vp);
}

pgVoid pgSetScissorAreaCore(pgGraphicsDevice* device, pgRectangle scissorArea)
{
	D3D11_RECT rect;
	rect.left = scissorArea.left;
	rect.top = scissorArea.top;
	rect.right = scissorArea.left + scissorArea.width;
	rect.bottom = scissorArea.top + scissorArea.height;

	ID3D11DeviceContext_RSSetScissorRects(device->context, 1, &rect);
}

pgVoid pgSetPrimitiveTypeCore(pgGraphicsDevice* device, pgPrimitiveType primitiveType)
{
	ID3D11DeviceContext_IASetPrimitiveTopology(device->context, pgConvertPrimitiveType(primitiveType));
}

pgVoid pgDrawCore(pgGraphicsDevice* device, pgInt32 primitiveCount, pgInt32 offset)
{
	ID3D11DeviceContext_Draw(device->context, pgPrimitiveCountToVertexCount(device, primitiveCount), offset);
}

pgVoid pgDrawIndexedCore(pgGraphicsDevice* device, pgInt32 indexCount, pgInt32 indexOffset, pgInt32 vertexOffset)
{
	ID3D11DeviceContext_DrawIndexed(device->context, indexCount, indexOffset, vertexOffset);
}

pgVoid pgPrintDeviceInfoCore(pgGraphicsDevice* device)
{
	DXGI_ADAPTER_DESC adapterDesc;
	pgChar buffer[sizeof(adapterDesc.Description)];
	pgString featureLevel = FeatureLevelToString(ID3D11Device_GetFeatureLevel(device->ptr));

	IDXGIAdapter_GetDesc(device->adapter, &adapterDesc);
	wcstombs(buffer, adapterDesc.Description, sizeof(buffer));

	PG_INFO("Direct3D renderer: %s", buffer);
	PG_INFO("Direct3D feature level: %s", featureLevel);
}

#endif