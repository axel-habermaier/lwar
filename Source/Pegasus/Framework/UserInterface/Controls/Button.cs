using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Platform.Graphics;
	using Rendering;
	using Rendering.UserInterface;

	/// <summary>
	///   Represents a button control.
	/// </summary>
	public class Button : ContentControl
	{
		public void Draw(SpriteBatch batch, Font font)
		{
			batch.Layer--;
			if (Content is Button)
				((Button)Content).Draw(batch, font);
			else
			{
				using (var text = Text.Create(Content.ToString()))
					TextRenderer.Draw(batch, font, text, Color.White, new Vector2i(100, 100));
			}

			batch.Draw(new Rectangle(200, 20, (int)Width, (int)Height), Texture2D.White, Foreground);
			batch.Layer++;
		}
	}
}