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
	public class DeviceState
	{
		/// <summary>
		///     The constant buffers that are currently bound.
		/// </summary>
		internal readonly ConstantBuffer[] ConstantBuffers;

		/// <summary>
		///     The sampler states that are currently bound.
		/// </summary>
		internal readonly SamplerState[] SamplerStates;

		/// <summary>
		///     The textures that are currently bound.
		/// </summary>
		internal readonly Texture[] Textures;

		/// <summary>
		///     The blend state that is currently bound.
		/// </summary>
		internal BlendState BlendState;

		/// <summary>
		///     Indicates whether the graphics device can currently be used for drawing.
		/// </summary>
		internal bool CanDraw;

		/// <summary>
		///     The depth stencil state that is currently bound.
		/// </summary>
		internal DepthStencilState DepthStencilState;

		/// <summary>
		///     The primitive type that is currently bound.
		/// </summary>
		internal PrimitiveType? PrimitiveType;

		/// <summary>
		///     The rasterizer state that is currently bound.
		/// </summary>
		internal RasterizerState RasterizerState;

		/// <summary>
		///     The render target that is currently bound.
		/// </summary>
		internal RenderTarget RenderTarget;

		/// <summary>
		///     The scissor area that is currently bound.
		/// </summary>
		internal Rectangle? ScissorArea;

		/// <summary>
		///     The shader program that is currently bound.
		/// </summary>
		internal ShaderProgram ShaderProgram;

		/// <summary>
		///     The vertex input layout that is currently bound.
		/// </summary>
		internal VertexLayout VertexLayout;

		/// <summary>
		///     The viewport that is currently bound.
		/// </summary>
		internal Rectangle? Viewport;

		/// <summary>
		///     Initializes the state.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the state belongs to.</param>
		internal DeviceState(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			ConstantBuffers = new ConstantBuffer[GraphicsDevice.ConstantBufferSlotCount];
			Textures = new Texture[GraphicsDevice.TextureSlotCount];
			SamplerStates = new SamplerState[GraphicsDevice.TextureSlotCount];
		}

		/// <summary>
		///     Changes the state, if the current state value and the new one differ. Returns false to indicate that a state change was
		///     not required.
		/// </summary>
		/// <typeparam name="T">The type of the state that should be changed.</typeparam>
		/// <param name="stateValue">The current state value that will be updated, if necessary.</param>
		/// <param name="value">The new state value.</param>
		internal static bool Change<T>(ref T stateValue, T value)
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
		internal static bool Change<T>(T[] stateValues, int index, T value)
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
		internal static void Unset<T>(ref T stateValue, T value)
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
		internal static void Unset<T>(T[] stateValues, T value)
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
		internal void Validate()
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
	}
}