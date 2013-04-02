using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents an effect that can be used to draw 2D sprites.
	/// </summary>
	public interface ISpriteEffect
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
		///   Binds the effect to the graphics device.
		/// </summary>
		void Bind();
	}
}