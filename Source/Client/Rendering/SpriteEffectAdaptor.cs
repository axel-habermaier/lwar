using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   An adaptor to an effect that can be used to draw 2D sprites.
	/// </summary>
	public class SpriteEffectAdaptor : DisposableObject, ISpriteEffectAdaptor
	{
		/// <summary>
		///   The sprite effect that is adapted.
		/// </summary>
		private readonly SpriteEffect _effect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SpriteEffectAdaptor(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_effect = new SpriteEffect(graphicsDevice, assets);
		}

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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
		}
	}
}