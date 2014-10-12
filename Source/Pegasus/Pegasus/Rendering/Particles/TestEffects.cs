namespace Pegasus.Rendering.Particles
{
	using System;
	using Assets;
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
			particleEffect.Emitters.Add(new Emitter
			{
				Capacity = 1000000,
				InitialColor = new Range<Color>(new Color(0f, 0, 0, 1), new Color(1f, 1, 1, 1)),
				InitialPosition = new Range<Vector3>(new Vector3(0, 0, 0), new Vector3(100, 100, 0)),
				InitialVelocity = new Range<Vector3>(new Vector3(-200, -300, -20), new Vector3(100, 230, 20)),
				InitialScale = new Range<float>(10, 20),
				EmissionRate = 200000,
				Lifetime = 50,
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
			particleEffect.Emitters.Add(new Emitter
			{
				Capacity = 10000,
				InitialColor = new Range<Color>(new Color(0f, 0, 0, 0), new Color(1f, 1, 1, 1)),
				InitialPosition = new Range<Vector3>(new Vector3(0, 0, 0), new Vector3(100, 100, 0)),
				InitialVelocity = new Range<Vector3>(new Vector3(-100, -230, -10), new Vector3(100, 230, 10)),
				InitialScale = new Range<float>(10, 100),
				EmissionRate = 2000,
				Lifetime = 5,
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
					new ScaleModifier{ Delta = -10 }
				}
			});
		}
	}
}