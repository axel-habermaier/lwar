namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Actors;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering.Particles;
	using Pegasus.Rendering.Particles.Modifiers;

	/// <summary>
	///     Renders explosion effects into a 3D scene.
	/// </summary>
	public class ExplosionRenderer : Renderer<Explosion>
	{
		/// <summary>
		///     The particle effect template used to initialize the explosion particle effects.
		/// </summary>
		private readonly ExplosionEffectTemplate _template = new ExplosionEffectTemplate();

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_template.Load(graphicsDevice, assets);
		}

		/// <summary>
		///     Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="element">The element that has been added.</param>
		protected override void OnAdded(Explosion element)
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
			///     The texture that is used to draw the particles.
			/// </summary>
			private Texture2D _texture;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			public ExplosionEffectTemplate()
				: base("Explosion")
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
				const float scale = 200f;
				particleEffect.Emitters.Add(new Emitter
				{
					Capacity = 500,
					InitialColor = new Range<Color>(new Color(255, 156, 43, 255), new Color(255, 202, 30, 255)),
					InitialPosition = new Range<Vector3>(Vector3.Zero),
					InitialVelocity = new Range<Vector3>(new Vector3(-scale, 0, -scale), new Vector3(scale, 0, scale)),
					EmissionRate = 3000,
					Lifetime = 0.4f,
					Duration = 0.2f,
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
}