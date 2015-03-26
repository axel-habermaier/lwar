namespace Pegasus.Rendering
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;

	partial class RenderContext
	{
		/// <summary>
		///     Provides access to commonly used blend states.
		/// </summary>
		public struct CommonBlendStates
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="graphicsDevice">The graphics device that should be used to initialize the states.</param>
			internal CommonBlendStates(GraphicsDevice graphicsDevice)
				: this()
			{
				var description = BlendDescription.Default();

				Opaque = new BlendState(graphicsDevice, description);
				Opaque.SetName("Opaque");
				Opaque.Bind();

				description = BlendDescription.Default();
				description.BlendEnabled = true;
				description.SourceBlend = BlendOption.One;
				description.DestinationBlend = BlendOption.InverseSourceAlpha;
				description.SourceBlendAlpha = BlendOption.One;
				description.DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

				Premultiplied = new BlendState(graphicsDevice, description);
				Premultiplied.SetName("Premultiplied");

				description = BlendDescription.Default();
				description.BlendEnabled = true;
				description.SourceBlend = BlendOption.One;
				description.DestinationBlend = BlendOption.One;
				description.SourceBlendAlpha = BlendOption.One;
				description.DestinationBlendAlpha = BlendOption.One;

				Additive = new BlendState(graphicsDevice, description);
				Additive.SetName("Additive");

				description = BlendDescription.Default();
				description.BlendEnabled = true;
				description.SourceBlend = BlendOption.SourceAlpha;
				description.DestinationBlend = BlendOption.InverseSourceAlpha;
				description.SourceBlendAlpha = BlendOption.SourceAlpha;
				description.DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

				Alpha = new BlendState(graphicsDevice, description);
				Alpha.SetName("Alpha");
			}

			/// <summary>
			///     Gets a default blend state with blending disabled.
			/// </summary>
			public BlendState Opaque { get; private set; }

			/// <summary>
			///     Gets a default blend state for premultiplied alpha blending.
			/// </summary>
			public BlendState Premultiplied { get; private set; }

			/// <summary>
			///     Gets a default blend state for additive alpha blending.
			/// </summary>
			public BlendState Additive { get; private set; }

			/// <summary>
			///     Gets a default blend state for alpha blending.
			/// </summary>
			public BlendState Alpha { get; private set; }

			/// <summary>
			///     Disposes the object, releasing all managed and unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				Opaque.SafeDispose();
				Premultiplied.SafeDispose();
				Additive.SafeDispose();
				Alpha.SafeDispose();
			}
		}

		/// <summary>
		///     Provides access to commonly used depth stencil states.
		/// </summary>
		public struct CommonDepthStencilStates
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="graphicsDevice">The graphics device that should be used to initialize the states.</param>
			internal CommonDepthStencilStates(GraphicsDevice graphicsDevice)
				: this()
			{
				var description = DepthStencilDescription.Default();

				DepthEnabled = new DepthStencilState(graphicsDevice, description);
				DepthEnabled.SetName("DepthEnabled");

				description = DepthStencilDescription.Default();
				description.DepthEnabled = false;
				description.DepthWriteEnabled = false;

				DepthDisabled = new DepthStencilState(graphicsDevice, description);
				DepthDisabled.SetName("DepthDisabled");
				DepthDisabled.Bind();

				description = DepthStencilDescription.Default();
				description.DepthWriteEnabled = false;
				DepthRead = new DepthStencilState(graphicsDevice, description);
				DepthRead.SetName("DepthRead");
			}

			/// <summary>
			///     Gets a default depth stencil state with depth read and write enabled, less comparison operation
			///     and stencil operations disabled.
			/// </summary>
			public DepthStencilState DepthEnabled { get; private set; }

			/// <summary>
			///     Gets a default depth stencil state with all depth and stencil operations disabled.
			/// </summary>
			public DepthStencilState DepthDisabled { get; private set; }

			/// <summary>
			///     Gets a default depth stencil state with depth writes disabled and all stencil operations disabled.
			/// </summary>
			public DepthStencilState DepthRead { get; private set; }

			/// <summary>
			///     Disposes the object, releasing all managed and unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				DepthEnabled.SafeDispose();
				DepthDisabled.SafeDispose();
				DepthRead.SafeDispose();
			}
		}

		/// <summary>
		///     Provides access to commonly used rasterizer states.
		/// </summary>
		public struct CommonRasterizerStates
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="graphicsDevice">The graphics device that should be used to initialize the states.</param>
			internal CommonRasterizerStates(GraphicsDevice graphicsDevice)
				: this()
			{
				var description = RasterizerDescription.Default();
				description.CullMode = CullMode.None;

				CullNone = new RasterizerState(graphicsDevice, description);
				CullNone.SetName("CullNone");
				CullNone.Bind();

				description = RasterizerDescription.Default();
				description.CullMode = CullMode.Back;
				description.FrontIsCounterClockwise = true;

				CullClockwise = new RasterizerState(graphicsDevice, description);
				CullClockwise.SetName("CullClockwise");

				description = RasterizerDescription.Default();
				description.CullMode = CullMode.Back;
				description.FrontIsCounterClockwise = false;

				CullCounterClockwise = new RasterizerState(graphicsDevice, description);
				CullCounterClockwise.SetName("CullCounterClockwise");
			}

			/// <summary>
			///     Gets a default rasterizer state with culling disabled.
			/// </summary>
			public RasterizerState CullNone { get; private set; }

			/// <summary>
			///     Gets a default rasterizer state with clockwise culling enabled.
			/// </summary>
			public RasterizerState CullClockwise { get; private set; }

			/// <summary>
			///     Gets a default rasterizer state with counter-clockwise culling enabled.
			/// </summary>
			public RasterizerState CullCounterClockwise { get; private set; }

			/// <summary>
			///     Disposes the object, releasing all managed and unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				CullNone.SafeDispose();
				CullClockwise.SafeDispose();
				CullCounterClockwise.SafeDispose();
			}
		}

		/// <summary>
		///     Provides access to commonly used sampler states.
		/// </summary>
		public struct CommonSamplerStates
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="graphicsDevice">The graphics device that should be used to initialize the states.</param>
			internal CommonSamplerStates(GraphicsDevice graphicsDevice)
				: this()
			{
				var description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;
				description.Filter = TextureFilter.Nearest;

				PointWrap = new SamplerState(graphicsDevice, description);
				PointWrap.SetName("PointWrap");

				description = SamplerDescription.Default();
				description.Filter = TextureFilter.Nearest;

				PointClamp = new SamplerState(graphicsDevice, description);
				PointClamp.SetName("PointClamp");

				description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;
				description.Filter = TextureFilter.NearestNoMipmaps;

				PointWrapNoMipmaps = new SamplerState(graphicsDevice, description);
				PointWrapNoMipmaps.SetName("PointWrapNoMipmaps");

				description = SamplerDescription.Default();
				description.Filter = TextureFilter.NearestNoMipmaps;

				PointClampNoMipmaps = new SamplerState(graphicsDevice, description);
				PointClampNoMipmaps.SetName("PointClampNoMipmaps");

				description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;

				BilinearWrap = new SamplerState(graphicsDevice, description);
				BilinearWrap.SetName("BilinearWrap");

				description = SamplerDescription.Default();

				BilinearClamp = new SamplerState(graphicsDevice, description);
				BilinearClamp.SetName("BilinearClamp");

				description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;
				description.Filter = TextureFilter.BilinearNoMipmaps;

				BilinearWrapNoMipmaps = new SamplerState(graphicsDevice, description);
				BilinearWrapNoMipmaps.SetName("BilinearWrapNoMipmaps");

				description = SamplerDescription.Default();
				description.Filter = TextureFilter.BilinearNoMipmaps;

				BilinearClampNoMipmaps = new SamplerState(graphicsDevice, description);
				BilinearClampNoMipmaps.SetName("BilinearClampNoMipmaps");

				description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;
				description.Filter = TextureFilter.Trilinear;
				description.MaximumAnisotropy = 1;

				TrilinearWrap = new SamplerState(graphicsDevice, description);
				TrilinearWrap.SetName("TrilinearWrap");

				description = SamplerDescription.Default();
				description.Filter = TextureFilter.Trilinear;
				description.MaximumAnisotropy = 1;

				TrilinearClamp = new SamplerState(graphicsDevice, description);
				TrilinearClamp.SetName("TrilinearClamp");

				description = SamplerDescription.Default();
				description.AddressU = TextureAddressMode.Wrap;
				description.AddressV = TextureAddressMode.Wrap;
				description.AddressW = TextureAddressMode.Wrap;
				description.Filter = TextureFilter.Anisotropic;

				AnisotropicWrap = new SamplerState(graphicsDevice, description);
				AnisotropicWrap.SetName("AnisotropicWrap");

				description = SamplerDescription.Default();
				description.Filter = TextureFilter.Anisotropic;

				AnisotropicClamp = new SamplerState(graphicsDevice, description);
				AnisotropicClamp.SetName("AnisotropicClamp");
			}

			/// <summary>
			///     Gets a default sampler state with point-filtering and wrap address mode.
			/// </summary>
			public SamplerState PointWrap { get; private set; }

			/// <summary>
			///     Gets a default sampler state with point-filtering and clamp address mode.
			/// </summary>
			public SamplerState PointClamp { get; private set; }

			/// <summary>
			///     Gets a default sampler state with point-filtering and wrap address mode for textures without mipmaps.
			/// </summary>
			public SamplerState PointWrapNoMipmaps { get; private set; }

			/// <summary>
			///     Gets a default sampler state with point-filtering and clamp address mode for textures without mipmaps.
			/// </summary>
			public SamplerState PointClampNoMipmaps { get; private set; }

			/// <summary>
			///     Gets a default sampler state with bilinear filtering and wrap address mode.
			/// </summary>
			public SamplerState BilinearWrap { get; private set; }

			/// <summary>
			///     Gets a default sampler state with bilinear filtering and clamp address mode.
			/// </summary>
			public SamplerState BilinearClamp { get; private set; }

			/// <summary>
			///     Gets a default sampler state with bilinear filtering and wrap address mode for textures without mipmaps.
			/// </summary>
			public SamplerState BilinearWrapNoMipmaps { get; private set; }

			/// <summary>
			///     Gets a default sampler state with bilinear filtering and clamp address mode for textures without mipmaps.
			/// </summary>
			public SamplerState BilinearClampNoMipmaps { get; private set; }

			/// <summary>
			///     Gets a default sampler state with trilinea filtering and wrap address mode.
			/// </summary>
			public SamplerState TrilinearWrap { get; private set; }

			/// <summary>
			///     Gets a default sampler state with trilinear filtering and clamp address mode.
			/// </summary>
			public SamplerState TrilinearClamp { get; private set; }

			/// <summary>
			///     Gets a default sampler state with anisotropic filtering and wrap address mode.
			/// </summary>
			public SamplerState AnisotropicWrap { get; private set; }

			/// <summary>
			///     Gets a default sampler state with anisotropic filtering and clamp address mode.
			/// </summary>
			public SamplerState AnisotropicClamp { get; private set; }

			/// <summary>
			///     Disposes the object, releasing all managed and unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				PointWrap.SafeDispose();
				PointClamp.SafeDispose();
				PointWrapNoMipmaps.SafeDispose();
				PointClampNoMipmaps.SafeDispose();

				BilinearWrap.SafeDispose();
				BilinearClamp.SafeDispose();
				BilinearWrapNoMipmaps.SafeDispose();
				BilinearClampNoMipmaps.SafeDispose();

				TrilinearWrap.SafeDispose();
				TrilinearClamp.SafeDispose();

				AnisotropicWrap.SafeDispose();
				AnisotropicClamp.SafeDispose();
			}
		}
	}
}