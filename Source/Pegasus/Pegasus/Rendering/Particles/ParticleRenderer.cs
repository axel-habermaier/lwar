﻿namespace Pegasus.Rendering.Particles
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     A base class for particle renderers.
	/// </summary>
	public abstract class ParticleRenderer : DisposableObject
	{
		/// <summary>
		///     The graphics device that is used to draw the particles.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///     The maximum number of particles drawn per frame.
		/// </summary>
		private int _capacity;

		/// <summary>
		///     The vertex buffer storing the particle colors.
		/// </summary>
		private DynamicVertexBuffer _colorsBuffer;

		/// <summary>
		///     The input layout used to draw the particles.
		/// </summary>
		private VertexInputLayout _inputLayout;

		/// <summary>
		///     The vertex buffer storing the particle positions.
		/// </summary>
		private DynamicVertexBuffer _positionsBuffer;
		/// <summary>
		/// The vertex buffer storing the particle scales.
		/// </summary>
		private DynamicVertexBuffer _scalesBuffer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the particles.</param>
		protected ParticleRenderer(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			_graphicsDevice = graphicsDevice;
			BlendState = BlendState.Additive;
		}

		/// <summary>
		///     Gets or sets the maximum number of particles drawn per frame.
		/// </summary>
		internal int Capacity
		{
			get { return _capacity; }
			set
			{
				if (_capacity == value)
					return;

				_capacity = value;

				InitializeVertexBuffers();
				CreateInputLayout();
			}
		}

		/// <summary>
		///     Gets or sets the blend state that is used to render the particles.
		/// </summary>
		public BlendState BlendState { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the renderer is currently being used by an emitter.
		/// </summary>
		internal bool IsUsed { get; set; }

		/// <summary>
		///     Changes the world matrix used to render the particles.
		/// </summary>
		internal abstract void ChangeWorldMatrix(ref Matrix matrix);

		/// <summary>
		///     Draws the particles.
		/// </summary>
		/// <param name="renderOutput">The render output the particles should be drawn to.</param>
		/// <param name="particles">The particles that should be drawn.</param>
		/// <param name="particleCount">The number of particles that should be drawn.</param>
		internal unsafe void Draw(RenderOutput renderOutput, ParticleCollection particles, int particleCount)
		{
			if (particleCount == 0)
				return;

			var size = particleCount * 3 * sizeof(float);
			using (var positions = _positionsBuffer.MapRange(0, size))
				positions.Write(particles.Positions, 0, size);

			size = particleCount * 4 * sizeof(byte);
			using (var colors = _colorsBuffer.MapRange(0, size))
				colors.Write(particles.Colors, 0, size);

			size = particleCount * sizeof(float);
			using (var scales = _scalesBuffer.MapRange(0, size))
				scales.Write(particles.Scales, 0, size);

			_inputLayout.Bind();
			BlendState.Bind();

			var instanceOffset = _positionsBuffer.GetInstanceOffset(Capacity);
			Assert.That(_colorsBuffer.GetInstanceOffset(Capacity) == instanceOffset, "Buffer update cycle mismatch.");
			Draw(renderOutput, particles, particleCount, instanceOffset);
		}

		/// <summary>
		///     Draws the particles.
		/// </summary>
		/// <param name="renderOutput">The render output the particles should be drawn to.</param>
		/// <param name="particles">The particles that should be drawn.</param>
		/// <param name="particleCount">The number of particles that should be drawn.</param>
		/// <param name="instanceOffset">The offset that should be applied to the instanced vertex buffer.</param>
		protected abstract void Draw(RenderOutput renderOutput, ParticleCollection particles, int particleCount, int instanceOffset);

		/// <summary>
		///     Gets the input bindings required to draw a single particle instance.
		/// </summary>
		protected abstract VertexInputBinding[] GetParticleInputBindings();

		/// <summary>
		///     Initializes the vertex buffers.
		/// </summary>
		private void InitializeVertexBuffers()
		{
			_positionsBuffer.SafeDispose();
			_colorsBuffer.SafeDispose();

			if (Capacity <= 0)
				return;

			_positionsBuffer = DynamicVertexBuffer.Create<float>(_graphicsDevice, Capacity * 3, GraphicsDevice.FrameLag);
			_colorsBuffer = DynamicVertexBuffer.Create<byte>(_graphicsDevice, Capacity * 4, GraphicsDevice.FrameLag);
			_scalesBuffer = DynamicVertexBuffer.Create<float>(_graphicsDevice, Capacity , GraphicsDevice.FrameLag);

			_positionsBuffer.SetName("Particle Positions Buffer");
			_colorsBuffer.SetName("Particle Colors Buffer");
			_scalesBuffer.SetName("Particle Scales Buffer");
		}

		/// <summary>
		///     Creates the vertex input layout used to draw the particles.
		/// </summary>
		private unsafe void CreateInputLayout()
		{
			_inputLayout.SafeDispose();

			var particleBindings = GetParticleInputBindings();

			Assert.NotNull(particleBindings);
			Assert.That(particleBindings.Length > 0, "Expected an input binding.");

			var bindings = new VertexInputBinding[particleBindings.Length + 3];
			for (var i = 0; i < particleBindings.Length; ++i)
				bindings[i] = particleBindings[i];

			bindings[particleBindings.Length] =
				new VertexInputBinding(_positionsBuffer.Buffer, VertexDataFormat.Vector3, DataSemantics.TexCoords0, sizeof(Vector3), 0, 1);

			bindings[particleBindings.Length + 1] =
				new VertexInputBinding(_colorsBuffer.Buffer, VertexDataFormat.Color, DataSemantics.Color0, sizeof(int), 0, 1);

			bindings[particleBindings.Length + 2] =
				new VertexInputBinding(_scalesBuffer.Buffer, VertexDataFormat.Float, DataSemantics.TexCoords1, sizeof(float), 0, 1);

			_inputLayout = new VertexInputLayout(_graphicsDevice, bindings);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputLayout.SafeDispose();
			_positionsBuffer.SafeDispose();
			_colorsBuffer.SafeDispose();
			_scalesBuffer.SafeDispose();
		}
	}
}