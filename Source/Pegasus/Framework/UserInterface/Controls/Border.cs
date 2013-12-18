namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Draws a border, a background, or both around another element.
	/// </summary>
	public class Border : Decorator
	{
		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			spriteBatch.Draw(new Rectangle(x, y, width, height), Texture2D.White, Background);
		}
	}
}