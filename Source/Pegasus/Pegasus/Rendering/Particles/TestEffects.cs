namespace Pegasus.Rendering.Particles
{
	using System;
	using Assets;
	using Emitteres;
	using Math;
	using Modifiers;
	using Platform.Graphics;

	/// <summary>
	///     A particle effect with a huge number of particles for testing.
	/// </summary>
	internal class HugeTestEffect : ParticleEffectTemplate
	{
		/// <summary>
		///     The texture that is used to draw the particles.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public HugeTestEffect()
			: base("Huge Test Effect")
		{
		}

		/// <summary>
		///     Loads the required assets of the particle effect template.
		/// </summary>
		protected override void Load()
		{
			BillboardRenderer.PreloadAssets(Assets);
			_texture = Assets.Load(Textures.ParticleTest);
		}

		/// <summary>
		///     Initializes the given particle effect with the template's parameters.
		/// </summary>
		/// <param name="particleEffect">The particle effect that should be initialized.</param>
		public override void Initialize(ParticleEffect particleEffect)
		{
			particleEffect.Emitters.Add(new PointEmitter
			{
				Capacity = 1000000,
				InitialColor = new Range<Color>(new Color(0f, 0, 0, 1), new Color(1f, 1, 1, 1)),
				InitialSpeed = new Range<float>(10, 400),
				InitialScale = new Range<float>(10, 20),
				InitialLifetime = new Range<float>(50),
				EmissionRate = 200000,
				Duration = 5,
				Renderer = new BillboardRenderer(GraphicsDevice, Assets)
				{
					BlendState = BlendState.Additive,
					Texture = _texture,
					SamplerState = SamplerState.BilinearClampNoMipmaps
				},
				Modifiers = new ModifierCollection
				{
					new FadeOutModifier()
				}
			});
		}
	}

	/// <summary>
	///     A particle effect with a small number of particles for testing.
	/// </summary>
	internal class SmallTestEffect : ParticleEffectTemplate
	{
		/// <summary>
		///     The texture that is used to draw the particles.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public SmallTestEffect()
			: base("Small Test Effect")
		{
		}

		/// <summary>
		///     Loads the required assets of the particle effect template.
		/// </summary>
		protected override void Load()
		{
			BillboardRenderer.PreloadAssets(Assets);
			_texture = Assets.Load(Textures.ParticleTest);
		}

		/// <summary>
		///     Initializes the given particle effect with the template's parameters.
		/// </summary>
		/// <param name="particleEffect">The particle effect that should be initialized.</param>
		public override void Initialize(ParticleEffect particleEffect)
		{
			particleEffect.Emitters.Add(new PointEmitter
			{
				Capacity = 4000,
				InitialColor = new Range<Color>(new Color(0f, 0, 0, 0), new Color(1f, 1, 1, 1)),
				InitialSpeed = new Range<float>(10, 200),
				InitialScale = new Range<float>(20, 100),
				InitialLifetime = new Range<float>(1, 5),
				EmissionRate = 2000,
				Duration = 5,
				Renderer = new BillboardRenderer(GraphicsDevice, Assets)
				{
					BlendState = BlendState.Additive,
					Texture = _texture,
					SamplerState = SamplerState.BilinearClampNoMipmaps
				},
				Modifiers = new ModifierCollection
				{
					new FadeOutModifier(),
					new ScaleModifier(-5)
				}
			});
		}
	}
}