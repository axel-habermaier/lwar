namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Client.Actors;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Rendering.Particles;
	using Pegasus.Rendering.Particles.Emitteres;
	using Pegasus.Rendering.Particles.Modifiers;

	/// <summary>
	///     Renders explosion effects into a 3D scene.
	/// </summary>
	internal class ExplosionRenderer : Renderer<ExplosionActor>
	{
		/// <summary>
		///     The particle effect template used to initialize the explosion particle effects.
		/// </summary>
		private ExplosionEffectTemplate _template;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_template = new ExplosionEffectTemplate(renderContext);
		}

		/// <summary>
		///     Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="element">The element that has been added.</param>
		protected override void OnAdded(ExplosionActor element)
		{
			element.ParticleEffect.Template = _template;
			element.ParticleEffect.Reset();
		}

		/// <summary>
		///     Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var explosion in Elements)
			{
				explosion.ParticleEffect.WorldMatrix = explosion.Transform.Matrix;
				explosion.ParticleEffect.Draw(output);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_template.SafeDispose();
		}

		/// <summary>
		///     A particle effect template for exploding ships.
		/// </summary>
		private class ExplosionEffectTemplate : ParticleEffectTemplate
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="renderContext">The render context the particle effect belongs to.</param>
			public ExplosionEffectTemplate(RenderContext renderContext)
				: base(renderContext, "Explosion")
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
					Capacity = 500,
					InitialColor = new Range<Color>(new Color(255, 156, 43, 255), new Color(255, 202, 30, 255)),
					InitialSpeed = new Range<float>(200),
					InitialScale = new Range<float>(100),
					InitialLifetime = new Range<float>(0.3f, 0.5f),
					EmissionRate = 3000,
					Duration = 0.2f,
					Renderer = new BillboardRenderer(RenderContext)
					{
						BlendState = RenderContext.BlendStates.Additive,
						Texture = RenderContext.GetAssetBundle<GameBundle>().BulletGlow,
						SamplerState = RenderContext.SamplerStates.BilinearClampNoMipmaps
					},
					Modifiers = new ModifierCollection
					{
						new FadeOutModifier()
					}
				});
			}
		}
	}
}