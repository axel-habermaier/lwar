namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Math;
	using Utilities;

	/// <summary>
	///     Represents the state of a graphics device.
	/// </summary>
	internal class DeviceState
	{
		/// <summary>
		///     Indicates whether the graphics device can currently be used for drawing.
		/// </summary>
		public bool CanDraw;

		/// <summary>
		///     The constant buffers that are currently bound.
		/// </summary>
		public readonly ConstantBuffer[] ConstantBuffers = new ConstantBuffer[GraphicsDevice.ConstantBufferSlotCount];

		/// <summary>
		///     The textures that are currently bound.
		/// </summary>
		public readonly Texture[] Textures = new Texture[GraphicsDevice.TextureSlotCount];

		/// <summary>
		///     The blend state that is currently bound.
		/// </summary>
		public BlendState BlendState;

		/// <summary>
		///     The depth stencil state that is currently bound.
		/// </summary>
		public DepthStencilState DepthStencilState;

		/// <summary>
		///     The rasterizer state that is currently bound.
		/// </summary>
		public RasterizerState RasterizerState;

		/// <summary>
		///     The sampler states that are currently bound.
		/// </summary>
		public readonly SamplerState[] SamplerStates = new SamplerState[GraphicsDevice.TextureSlotCount];

		/// <summary>
		///     The render target that is currently bound.
		/// </summary>
		public RenderTarget RenderTarget;

		/// <summary>
		///     The shader program that is currently bound.
		/// </summary>
		public ShaderProgram ShaderProgram;

		/// <summary>
		///     The vertex input layout that is currently bound.
		/// </summary>
		public VertexLayout VertexLayout;

		/// <summary>
		///     The primitive type that is currently bound.
		/// </summary>
		public PrimitiveType? PrimitiveType;

		/// <summary>
		///     The viewport that is currently bound.
		/// </summary>
		public Rectangle? Viewport;

		/// <summary>
		///     The scissor area that is currently bound.
		/// </summary>
		public Rectangle? ScissorArea;

		/// <summary>
		///     Changes the state, if the current state value and the new one differ. Returns false to indicate that a state change was
		///     not required.
		/// </summary>
		/// <typeparam name="T">The type of the state that should be changed.</typeparam>
		/// <param name="stateValue">The current state value that will be updated, if necessary.</param>
		/// <param name="value">The new state value.</param>
		public static bool Change<T>(ref T stateValue, T value)
		{
			if (EqualityComparer<T>.Default.Equals(stateValue, value))
				return false;

			stateValue = value;
			return true;
		}

		/// <summary>
		///     Changes the state, if the current state value and the new one differ. Returns false to indicate that a state change was
		///     not required.
		/// </summary>
		/// <typeparam name="T">The type of the state that should be changed.</typeparam>
		/// <param name="stateValues">The current state values that will be updated, if necessary.</param>
		/// <param name="index">The index of the state value that should be updated.</param>
		/// <param name="value">The new state value.</param>
		public static bool Change<T>(T[] stateValues, int index, T value)
			where T : class
		{
			Assert.ArgumentNotNull(stateValues);
			Assert.ArgumentInRange(index, stateValues);

			if (stateValues[index] == value)
				return false;

			stateValues[index] = value;
			return true;
		}

		/// <summary>
		///     Unsets the given state value if it matches the given value.
		/// </summary>
		/// <typeparam name="T">The type of the state that should be unset.</typeparam>
		/// <param name="stateValue">The current state value that will be unset, if necessary.</param>
		/// <param name="value">The state value that should be unset.</param>
		public static void Unset<T>(ref T stateValue, T value)
			where T : class
		{
			if (stateValue == value)
				stateValue = null;
		}

		/// <summary>
		///     Unsets the given state value if it matches the given value.
		/// </summary>
		/// <typeparam name="T">The type of the state that should be unset.</typeparam>
		/// <param name="stateValues">The current state values that will be unset, if necessary.</param>
		/// <param name="value">The state value that should be unset.</param>
		public static void Unset<T>(T[] stateValues, T value)
			where T : class
		{
			for (var i = 0; i < stateValues.Length; ++i)
			{
				if (stateValues[i] == value)
					stateValues[i] = null;
			}
		}

		/// <summary>
		///     In debug builds, validates the state of the graphics device before drawing.
		/// </summary>
		[Conditional("DEBUG")]
		public void Validate()
		{
			Assert.That(CanDraw, "Drawing commands can only be issued between a call to BeginFrame() and EndFrame().");
			Assert.NotNull(BlendState);
			Assert.NotNull(DepthStencilState);
			Assert.NotNull(RasterizerState);
			Assert.NotNull(RenderTarget);
			Assert.NotNull(ShaderProgram);
			Assert.NotNull(VertexLayout);
			Assert.NotNull(RenderTarget);
			Assert.NotNull(Viewport);
			Assert.NotNull(PrimitiveType);
			Assert.InRange(PrimitiveType.Value);
			Assert.That(Viewport.Value.Size != new Size(), "Viewport has an area of 0.");
		}

#if Direct3D11
		VertexShader _boundVertexShader ;
		FragmentShader _boundFragmentShader ;
#elif OpenGL3
		public bool? DepthWritesEnabled;
		public bool? ScissorEnabled;
		public bool? BlendEnabled;
		public bool? DepthTestEnabled;
		public bool? CullFaceEnabled;
		public bool? DepthClampEnabled;
		public bool? MultisampleEnabled;
		public bool? StencilTestEnabled;
		public bool? AntialiasedLineEnabled;
		public bool? ColorMaskRed;
		public bool? ColorMaskGreen;
		public bool? ColorMaskBlue;
		public bool? ColorMaskAlpha;
		public float? SlopeScaledDepthBias;
		public float? DepthBiasClamp;
		public float? DepthClear;
		public int? StencilClear;
		public uint? ActiveTexture;
		public uint? CullFace;
		public uint? PolygonMode;
		public uint? FrontFace;
		public uint? DepthFunc;
		public uint? BlendOperation;
		public uint? BlendOperationAlpha;
		public uint? SourceBlend;
		public uint? DestinationBlend;
		public uint? SourceBlendAlpha;
		public uint? DestinationBlendAlpha;
		public Color? ClearColor;
		public Rectangle? ViewportGL;
		public Rectangle? ScissorAreaGL;
		public uint PrimitiveTypeGL;
		public uint Texture;
		public uint TextureType;
#endif
	}
}