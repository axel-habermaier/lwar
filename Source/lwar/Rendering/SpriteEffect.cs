using System;

namespace Lwar.Rendering
{
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///   An adapter to to the 2D sprite effect used by lwar.
	/// </summary>
	internal class SpriteEffect : DisposableObject, ISpriteEffect
	{
		/// <summary>
		///   The sprite effect that is adapted.
		/// </summary>
		private Assets.Effects.SpriteEffect _effect;

		/// <summary>
		///   Sets the texture view that should be used to draw the sprites.
		/// </summary>
		public Texture2DView Sprite
		{
			set
			{
				Assert.NotNull(_effect, "The effect has not been properly initialized.");
				_effect.Sprite = value;
			}
		}

		/// <summary>
		///   Sets the world matrix that affects where the sprites are rendered.
		/// </summary>
		public Matrix World
		{
			set
			{
				Assert.NotNull(_effect, "The effect has not been properly initialized.");
				_effect.World = value;
			}
		}

		/// <summary>
		///   Gets the effect technique that should be is draw the sprites.
		/// </summary>
		public EffectTechnique Technique
		{
			get
			{
				Assert.NotNull(_effect, "The effect has not been properly initialized.");
				return _effect.Default;
			}
		}

		/// <summary>
		///   Initializes a the sprite effect.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			_effect = new Assets.Effects.SpriteEffect(graphicsDevice, assets);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
		}
	}
}