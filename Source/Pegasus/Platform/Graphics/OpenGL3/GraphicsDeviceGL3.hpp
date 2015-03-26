#pragma once

namespace OpenGL3
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
		BindingsGL3 _bindings;
		SDL_Window* _contextWindow = nullptr;
		SDL_GLContext _context = nullptr;

		int32 _boundDepthWritesEnabled = -1;
		int32 _boundScissorEnabled = -1;
		int32 _boundBlendEnabled = -1;
		int32 _boundDepthTestEnabled = -1;
		int32 _boundCullFaceEnabled = -1;
		int32 _boundDepthClampEnabled = -1;
		int32 _boundMultisampleEnabled = -1;
		int32 _boundStencilTestEnabled = -1;
		int32 _boundAntialiasedLineEnabled = -1;
		int32 _boundColorMaskRed = -1;
		int32 _boundColorMaskGreen = -1;
		int32 _boundColorMaskBlue = -1;
		int32 _boundColorMaskAlpha = -1;
		float32 _boundSlopeScaledDepthBias = -1;
		float32 _boundDepthBiasClamp = -1;
		float32 _boundDepthClear = -1;
		int32 _boundStencilClear = -1;
		uint32 _boundPrimitiveType = static_cast<uint32>(-1);
		uint32 _boundActiveTexture = static_cast<uint32>(-1);
		uint32 _boundCullFace = static_cast<uint32>(-1);
		uint32 _boundPolygonMode = static_cast<uint32>(-1);
		uint32 _boundFrontFace = static_cast<uint32>(-1);
		uint32 _boundDepthFunc = static_cast<uint32>(-1);
		uint32 _boundBlendOperation = static_cast<uint32>(-1);
		uint32 _boundBlendOperationAlpha = static_cast<uint32>(-1);
		uint32 _boundSourceBlend = static_cast<uint32>(-1);
		uint32 _boundDestinationBlend = static_cast<uint32>(-1);
		uint32 _boundSourceBlendAlpha = static_cast<uint32>(-1);
		uint32 _boundDestinationBlendAlpha = static_cast<uint32>(-1);
		byte _boundClearColorRed = 0;
		byte _boundClearColorGreen = 0;
		byte _boundClearColorBlue = 0;
		byte _boundClearColorAlpha = 0;
		int32 _boundViewportLeft = 0;
		int32 _boundViewportTop = 0;
		int32 _boundViewportTopGL = 0;
		int32 _boundViewportWidth = 0;
		int32 _boundViewportHeight = 0;
		int32 _boundScissorLeft = 0;
		int32 _boundScissorTop = 0;
		int32 _boundScissorTopGL = 0;
		int32 _boundScissorWidth = 0;
		int32 _boundScissorHeight = 0;
		uint32 _boundTexture = 0;
		uint32 _boundTextureType = 0;
		VertexLayout* _boundVertexLayout = nullptr;
		RenderTarget* _boundRenderTarget = nullptr;

		uint32 Allocate(void(GraphicsDevice::*genFunc)(int32, uint32*), const char* typeString);
		int32 FlipY(int32 top, int32 height);

		void CheckErrors();

		void ValidateFramebufferCompleteness();
		void RebindRenderTarget();
		void BindRenderTargetGL(RenderTarget* renderTarget);

		void UploadTexture(const Surface& surface, SurfaceFormat format, uint32 target, int32 level);
		void RebindTexture();

		void Enable(uint32 capability, int32* current, int32 enabled);
		void ChangeActiveTexture(int32 slot);
		void EnableScissor(int32 enabled);
		void EnableBlend(int32 enabled);
		void EnableDepthTest(int32 enabled);
		void EnableCullFace(int32 enabled);
		void EnableDepthClamp(int32 enabled);
		void EnableMultisample(int32 enabled);
		void EnableAntialiasedLine(int32 enabled);
		void EnableStencilTest(int32 enabled);
		void EnableDepthWrites(int32 enabled);
		void SetCullFace(uint32 cullFace);
		void SetFrontFace(uint32 frontFace);
		void SetPolygonMode(uint32 mode);
		void SetPolygonOffset(float32 slopeScaledDepthBias, float32 depthBiasClamp);
		void SetDepthFunc(uint32 func);
		void SetClearColor(Color color);
		void SetClearDepth(float32 depth);
		void SetClearStencil(int32 stencil);
		void SetBlendEquation(uint32 blendOperation, uint32 blendOperationAlpha);
		void SetBlendFuncs(uint32 sourceBlend, uint32 destinationBlend, uint32 sourceBlendAlpha, uint32 destinationBlendAlpha);
		void SetColorMask(int32 mask[4]);

		uint32 GetInternalFormat(SurfaceFormat format);
		int32 GetIndexSizeInBytes(IndexSize indexSize);
		uint32 GetVertexDataType(VertexDataFormat format);
		int32 GetVertexDataComponentCount(VertexDataFormat format);
		int32 GetMinFilter(TextureFilter filter);
		int32 GetMagFilter(TextureFilter filter);

		uint32 Map(BlendOperation blendOperation);
		uint32 Map(BlendOption blendOption);
		uint32 Map(Comparison comparison);
		uint32 Map(FillMode fillMode);
		uint32 Map(IndexSize indexSize);
		uint32 Map(MapMode mapMode);
		uint32 Map(PrimitiveType primitiveType);
		uint32 Map(ResourceUsage resourceUsage);
		uint32 Map(StencilOperation stencilOperation);
		uint32 Map(SurfaceFormat surfaceFormat);
		int32 Map(TextureAddressMode addressMode);

		void* ToPointer(uint32 value);
		void* ToPointer(int32 value);

		PG_DECLARE_OPENGL3_FUNCS
	};
}