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
		protected override void OnDraw(SpriteBatch batch)
		{
			var width = (int)System.Math.Round(RenderSize.Width);
			var height = (int)System.Math.Round(RenderSize.Height);
			var x = (int)System.Math.Round(VisualOffset.X);
			var y = (int)System.Math.Round(VisualOffset.Y);

			batch.Draw(new Rectangle(x,y,width,height), Texture2D.White, Foreground);
		}
	}
}