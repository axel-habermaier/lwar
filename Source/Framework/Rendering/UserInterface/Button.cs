using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a button control.
	/// </summary>
	public class Button : ContentControl
	{
		public void Draw(SpriteBatch batch, Font font)
		{
			using (var text = Text.Create(Content.ToString()))
				TextRenderer.Draw(batch, font, text, Color.White, new Vector2i(100, 100));

			batch.Draw(new Rectangle(200, 20, 100, 100), Texture2D.White, Foreground);
		}
	}
}