namespace Pegasus.Rendering.Particles
{
	using System;
	using Assets;
	using Assets.Effects;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Renders particles as 2D billboard sprites.
	/// </summary>
	public sealed class BillboardRenderer : ParticleRenderer
	{
		/// <summary>
		///     The vertex buffer containing the quad that is used to draw the particles.
		/// </summary>
		private readonly VertexBuffer _buffer;

		/// <summary>
		///     The effect that is used to draw the particles.
		/// </summary>
		private readonly BillboardParticleEffect _effect;

		/// <summary>
		///     The input binding required to draw a single particle instance.
		/// </summary>
		private readonly VertexInputBinding[] _particleBindings = new VertexInputBinding[1];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the particles.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public unsafe BillboardRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(assets);

			_buffer = VertexBuffer.Create(graphicsDevice, new[]
			{
				new Vector2(-0.5f, -0.5f),
				new Vector2(-0.5f, 0.5f),
				new Vector2(0.5f, -0.5f),
				new Vector2(0.5f, 0.5f)
			});

			_buffer.SetName("Particle Billboard Renderer");
			_effect = new BillboardParticleEffect(graphicsDevice, assets) { World = Matrix.Identity };
			_particleBindings[0] = new VertexInputBinding(_buffer, VertexDataFormat.Vector2, DataSemantics.Position, sizeof(Vector2), 0);
		}

		/// <summary>
		///     Gets or sets the texture that is used to draw the particles.
		/// </summary>
		public Texture2D Texture { get; set; }

		/// <summary>
		///     Gets or sets the sampler state that is used to draw the particles.
		/// </summary>
		public SamplerState SamplerState { get; set; }

		/// <summary>
		///     Preloads the assets required by the billboard renderer into the given assets manager.
		/// </summary>
		/// <param name="assets">The assets manager the assets should be preloaded into.</param>
		public static void PreloadAssets(AssetsManager assets)
		{
			BillboardParticleEffect.PreloadShaders(assets);
		}

		/// <summary>
		///     Draws the particles.
		/// </summary>
		/// <param name="renderOutput">The render output the particles should be drawn to.</param>
		/// <param name="particles">The particles that should be drawn.</param>
		/// <param name="particleCount">The number of particles that should be drawn.</param>
		/// <param name="instanceOffset">The offset that should be applied to the instanced vertex buffer.</param>
		protected override void Draw(RenderOutput renderOutput, ParticleCollection particles, int particleCount, int instanceOffset)
		{
			_effect.Texture = new Texture2DView(Texture, SamplerState);
			renderOutput.DrawInstanced(_effect.Default, particleCount, 2, PrimitiveType.TriangleStrip, instanceOffset: instanceOffset);
		}

		/// <summary>
		///     Changes the world matrix used to render the particles.
		/// </summary>
		internal override void ChangeWorldMatrix(ref Matrix matrix)
		{
			_effect.World = matrix;
		}

		/// <summary>
		///     Gets the input bindings required to draw a single particle instance.
		/// </summary>
		protected override VertexInputBinding[] GetParticleInputBindings()
		{
			return _particleBindings;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			base.OnDisposing();

			_effect.SafeDispose();
			_buffer.SafeDispose();
		}
	}
}