using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a button control.
	/// </summary>
	public class Button : UIElement
	{
		public static readonly DependencyProperty<object> ContentProperty = new DependencyProperty<object>("");

		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public void Draw(SpriteBatch batch, Font font)
		{
			using (var text = Text.Create(Content.ToString()))
				TextRenderer.Draw(batch, font, text, Color.White, new Vector2i(100, 100));
		}
	}
}