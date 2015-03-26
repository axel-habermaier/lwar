#pragma once

#ifdef PG_SYSTEM_WINDOWS

namespace Direct3D11
{
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	// GraphicsDevice
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	class GraphicsDevice
	{
	public:
		GraphicsDevice(DeviceInterface* deviceInterface);
		~GraphicsDevice();

		PG_DECLARE_DEVICEINTERFACE_METHODS

	private:
		ID3D11VertexShader* _boundVertexShader = nullptr;
		ID3D11PixelShader* _boundPixelShader = nullptr;
		ComPtr<IDXGIFactory> _factory;
		ComPtr<IDXGIAdapter> _adapter;
		ComPtr<ID3D11Device> _device;
		ComPtr<ID3D11DeviceContext> _context;
		PG_DEBUG_ONLY(ComPtr<IDXGIDebug> _debug);

		static const DXGI_FORMAT SwapChainFormat = DXGI_FORMAT_R8G8B8A8_UNORM;
		static const D3D_FEATURE_LEVEL RequiredFeatureLevel = D3D_FEATURE_LEVEL_9_3;

		bool ChangeToFullscreenCore(SwapChain* swapChain, int32 width, int32 height);
		void ChangeToWindowedCore(SwapChain* swapChain, int32 width, int32 height);
		void InitializeBackBuffer(SwapChain* swapChain);
		void ReleaseBackBuffer(SwapChain* swapChain);
		void CreateTexture1D(Texture* texture, const TextureDescription* desc, const Surface* surfaces);
		void CreateTexture2D(Texture* texture, const TextureDescription* desc, const Surface* surfaces);
		void CreateTexture3D(Texture* texture, const TextureDescription* desc, const Surface* surfaces);
		void CreateCubeMap(Texture* texture, const TextureDescription* desc, const Surface* surfaces);
		void InitializeDesc2D(const TextureDescription* desc, D3D11_TEXTURE2D_DESC& d3d11Desc);
		void InitializeResourceData(D3D11_SUBRESOURCE_DATA* resourceData, const TextureDescription* desc, const Surface* surfaces);

		UINT GetSemanticIndex(DataSemantics semantics);
		LPCSTR GetSemanticName(DataSemantics semantics);

		void CheckResult(HRESULT hr, const char* msg);
		void SetName(const ComPtr<ID3D11DeviceChild>& obj, const char* name);

		D3D11_BLEND_OP Map(BlendOperation blendOperation);
		D3D11_BLEND Map(BlendOption blendOption);
		D3D11_COMPARISON_FUNC Map(Comparison comparison);
		byte Map(ColorWriteChannels channels);
		D3D11_CULL_MODE Map(CullMode cullMode);
		D3D11_FILL_MODE Map(FillMode fillMode);
		DXGI_FORMAT Map(IndexSize indexSize);
		D3D11_MAP Map(MapMode mapMode);
		D3D11_PRIMITIVE_TOPOLOGY Map(PrimitiveType primitiveType);
		D3D11_USAGE Map(ResourceUsage resourceUsage);
		D3D11_STENCIL_OP Map(StencilOperation stencilOperation);
		DXGI_FORMAT Map(SurfaceFormat surfaceFormat);
		D3D11_TEXTURE_ADDRESS_MODE Map(TextureAddressMode addressMode);
		D3D11_FILTER Map(TextureFilter textureFilter);
		DXGI_FORMAT Map(VertexDataFormat format);
	};
}

#endif