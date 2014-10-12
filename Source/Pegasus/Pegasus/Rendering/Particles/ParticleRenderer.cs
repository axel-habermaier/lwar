namespace Pegasus.Rendering.Particles
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;

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
		private DynamicVertexBuffer _colorBuffer;

		/// <summary>
		///     The input layout used to draw the particles.
		/// </summary>
		private VertexInputLayout _inputLayout;

		/// <summary>
		///     The vertex buffer storing the particle positions.
		/// </summary>
		private DynamicVertexBuffer _positionBuffer;

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
			using (var positions = _positionBuffer.MapRange(0, size))
				positions.Write(particles.Positions, 0, size);

			size = particleCount * 4 * sizeof(byte);
			using (var colors = _colorBuffer.MapRange(0, size))
				colors.Write(particles.Colors, 0, size);

			_inputLayout.Bind();
			BlendState.Bind();

			var instanceOffset = _positionBuffer.GetInstanceOffset(Capacity);
			Assert.That(_colorBuffer.GetInstanceOffset(Capacity) == instanceOffset, "Buffer update cycle mismatch.");
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
			_positionBuffer.SafeDispose();
			_colorBuffer.SafeDispose();

			if (Capacity <= 0)
				return;

			_positionBuffer = DynamicVertexBuffer.Create<float>(_graphicsDevice, Capacity * 3, GraphicsDevice.FrameLag);
			_colorBuffer = DynamicVertexBuffer.Create<byte>(_graphicsDevice, Capacity * 4, GraphicsDevice.FrameLag);

			_positionBuffer.SetName("Particle Position Buffer");
			_colorBuffer.SetName("Particle Color Buffer");
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

			var bindings = new VertexInputBinding[particleBindings.Length + 2];
			for (var i = 0; i < particleBindings.Length; ++i)
				bindings[i] = particleBindings[i];

			bindings[particleBindings.Length] =
				new VertexInputBinding(_positionBuffer.Buffer, VertexDataFormat.Vector3, DataSemantics.TexCoords0, sizeof(Vector3), 0, 1);

			bindings[particleBindings.Length + 1] =
				new VertexInputBinding(_colorBuffer.Buffer, VertexDataFormat.Color, DataSemantics.Color0, sizeof(int), 0, 1);

			_inputLayout = new VertexInputLayout(_graphicsDevice, bindings);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputLayout.SafeDispose();
			_positionBuffer.SafeDispose();
			_colorBuffer.SafeDispose();
		}
	}
}