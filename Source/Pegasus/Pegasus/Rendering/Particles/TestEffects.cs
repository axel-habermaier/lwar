namespace Pegasus.Rendering.Particles
{
	using System;
	using Assets;
	using Emitteres;
	using Math;
	using Modifiers;
	using Platform.Graphics;
	using Utilities;

	/// <summary>
	///     A particle effect with a huge number of particles for testing.
	/// </summary>
	[UsedImplicitly]
	internal class HugeTestEffect : ParticleEffectTemplate
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context the particle effect belongs to.</param>
		public HugeTestEffect(RenderContext renderContext)
			: base(renderContext, "Huge Test Effect")
		{
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
				Renderer = new BillboardRenderer(RenderContext)
				{
					BlendState = RenderContext.BlendStates.Additive,
					Texture = RenderContext.GetAssetBundle<MainBundle>().ParticleTest,
					SamplerState = RenderContext.SamplerStates.BilinearClampNoMipmaps
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
	[UsedImplicitly]
	internal class SmallTestEffect : ParticleEffectTemplate
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context the particle effect belongs to.</param>
		public SmallTestEffect(RenderContext renderContext)
			: base(renderContext, "Small Test Effect")
		{
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
				Renderer = new BillboardRenderer(RenderContext)
				{
					BlendState = RenderContext.BlendStates.Additive,
					Texture = RenderContext.GetAssetBundle<MainBundle>().ParticleTest,
					SamplerState = RenderContext.SamplerStates.BilinearClampNoMipmaps
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