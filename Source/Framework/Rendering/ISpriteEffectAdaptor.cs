using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   An adaptor to an effect that can be used to draw 2D sprites.
	/// </summary>
	public interface ISpriteEffectAdaptor : IDisposable
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
	}
}