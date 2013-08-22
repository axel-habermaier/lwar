using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Platform.Graphics;
	using Rendering;
	using Math = System.Math;

	/// <summary>
	///   Represents a button control.
	/// </summary>
	public class Button : ContentControl
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Button()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected override void OnDraw(SpriteBatch batch)
		{
			var width = (int)Math.Round(RenderSize.Width);
			var height = (int)Math.Round(RenderSize.Height);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			batch.Draw(new Rectangle(x, y, width, height), Texture2D.White, Foreground);
		}
	}
}