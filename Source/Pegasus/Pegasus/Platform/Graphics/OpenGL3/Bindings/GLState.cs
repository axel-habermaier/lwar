namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents parts of the state of the OpenGL state machine.
	/// </summary>
	internal struct GLState
	{
		public uint? ActiveTexture;
		public bool? AntialiasedLineEnabled;
		public bool? BlendEnabled;
		public uint? BlendOperation;
		public uint? BlendOperationAlpha;
		public Color? ClearColor;
		public bool? ColorMaskAlpha;
		public bool? ColorMaskBlue;
		public bool? ColorMaskGreen;
		public bool? ColorMaskRed;
		public uint? CullFace;
		public bool? CullFaceEnabled;
		public float? DepthBiasClamp;
		public bool? DepthClampEnabled;
		public float? DepthClear;
		public uint? DepthFunc;
		public bool? DepthTestEnabled;
		public bool? DepthWritesEnabled;
		public uint? DestinationBlend;
		public uint? DestinationBlendAlpha;
		public uint? FrontFace;
		public bool? MultisampleEnabled;
		public uint? PolygonMode;
		public uint PrimitiveType;
		public RenderTargetGL3 RenderTarget;
		public Rectangle ScissorArea;
		public Rectangle ScissorAreaGL;
		public bool? ScissorEnabled;
		public float? SlopeScaledDepthBias;
		public uint? SourceBlend;
		public uint? SourceBlendAlpha;
		public int? StencilClear;
		public bool? StencilTestEnabled;
		public uint Texture;
		public uint TextureType;
		public VertexLayoutGL3 VertexLayout;
		public Rectangle Viewport;
		public Rectangle ViewportGL;
	}
}