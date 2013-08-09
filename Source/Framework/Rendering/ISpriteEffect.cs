using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;

	/// <summary>
	///   An adapter to an effect that can be used to draw 2D sprites.
	/// </summary>
	public interface ISpriteEffect : IDisposable
	{
		/// <summary>
		///   Sets the texture view that should be used to draw the sprites.
		/// </summary>
		Texture2DView Sprite { set; }

		/// <summary>
		///   Sets the world matrix that affects where the sprites are rendered.
		/// </summary>
		Matrix World { set; }

		/// <summary>
		///   Gets the effect technique that should be is draw the sprites.
		/// </summary>
		EffectTechnique Technique { get; }

		/// <summary>
		///   Initializes the sprite effect adapter.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets);
	}
}