using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   An adapter to to the 2D sprite effect used by lwar.
	/// </summary>
	internal class SpriteEffectAdapter : DisposableObject, ISpriteEffectAdapter
	{
		/// <summary>
		///   The sprite effect that is adapted.
		/// </summary>
		private SpriteEffect _effect;

		/// <summary>
		///   Sets the texture view that should be used to draw the sprites.
		/// </summary>
		public Texture2DView Sprite
		{
			set { _effect.Sprite = value; }
		}

		/// <summary>
		///   Sets the world matrix that affects where the sprites are rendered.
		/// </summary>
		public Matrix World
		{
			set { _effect.World = value; }
		}

		/// <summary>
		///   Gets the effect technique that should be is draw the sprites.
		/// </summary>
		public EffectTechnique Technique
		{
			get { return _effect.Default; }
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

			_effect = new SpriteEffect(graphicsDevice, assets);
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