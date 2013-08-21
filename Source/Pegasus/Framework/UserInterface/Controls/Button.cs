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
		public override void Draw(SpriteBatch batch, Font font)
		{
			var width = (int)RenderSize.Width;
			var height = (int)RenderSize.Height;
			var x = (int)VisualOffset.X;
			var y = (int)VisualOffset.Y;

			batch.Layer--;
			var s = Content == null ? "" : Content.ToString();
			using (var text = Text.Create(s))
				TextRenderer.Draw(batch, font, text, Color.FromRgba(0,0,0,255), new Vector2i(x,y));

			batch.Layer--;
			batch.Draw(new Rectangle(x,y,width,height), Texture2D.White, Foreground);
			batch.Layer++;
			batch.Layer++;
		}
	}
}