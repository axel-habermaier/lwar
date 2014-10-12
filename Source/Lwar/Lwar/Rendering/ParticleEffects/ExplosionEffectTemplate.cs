namespace Lwar.Rendering.ParticleEffects
{
	using System;
	using Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering.Particles;
	using Pegasus.Rendering.Particles.Modifiers;

	

	internal class ExplosionEffect3 : ParticleEffectTemplate
	{
		/// <summary>
		///     The texture that is used to draw the particles.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ExplosionEffect3()
			: base("Explosion 3")
		{
		}

		/// <summary>
		///     Loads the required assets of the particle effect template.
		/// </summary>
		protected override void Load()
		{
			BillboardRenderer.PreloadAssets(Assets);
			_texture = Assets.Load(Textures.BulletGlow);
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
				InitialColor = new Range<Color>(new Color(0f, 0, 0, 1), new Color(1f, 1, 1, 1)),
				InitialPosition = new Range<Vector3>(new Vector3(0, 0, 0), new Vector3(100, 100, 0)),
				InitialVelocity = new Range<Vector3>(new Vector3(-100, -230, -10), new Vector3(100, 230, 10)),
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
					new FadeOutModifier()
				}
			});
		}
	}

	internal class ExplosionEffect2 : ParticleEffectTemplate
	{
		/// <summary>
		///     The texture that is used to draw the particles.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ExplosionEffect2()
			: base("Fast Explosion")
		{
		}

		/// <summary>
		///     Loads the required assets of the particle effect template.
		/// </summary>
		protected override void Load()
		{
			BillboardRenderer.PreloadAssets(Assets);
			_texture = Assets.Load(Textures.BulletGlow);
		}

		/// <summary>
		///     Initializes the given particle effect with the template's parameters.
		/// </summary>
		/// <param name="particleEffect">The particle effect that should be initialized.</param>
		public override void Initialize(ParticleEffect particleEffect)
		{
			particleEffect.Emitters.Add(new Emitter
			{
				Capacity = 200,
				InitialColor = new Range<Color>(new Color(1f, 0, 0, 1)),
				InitialPosition = new Range<Vector3>(new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
				InitialVelocity = new Range<Vector3>(new Vector3(100, -10, 0), new Vector3(100, 10, 0)),
				EmissionRate = 100,
				Lifetime = 2,
				Duration = 1,
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
}