namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents the mouse cursor.
	/// </summary>
	public class Cursor
	{
		/// <summary>
		///     The cursor that is displayed when the mouse hovers an UI element or any of its children.
		///     If null, the displayed cursor is determined by the hovered child element or the default cursor is displayed.
		/// </summary>
		public static readonly DependencyProperty<Cursor> CursorProperty = new DependencyProperty<Cursor>();

		/// <summary>
		///     The texture that defines the cursors visual appearance.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Cursor()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture that defines the cursors visual appearance.</param>
		/// <param name="hotSpot">
		///     The hot spot of the cursor, i.e., the relative offset to the texture's origin where the cursor's click
		///     location lies.
		/// </param>
		/// <param name="color">The color the cursor should be drawn in.</param>
		public Cursor(Texture2D texture, Vector2 hotSpot, Color color)
		{
			Assert.ArgumentNotNull(texture);

			Texture = texture;
			HotSpot = hotSpot;
			Color = color;
		}

		/// <summary>
		///     Gets or sets the color the cursor is drawn in.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		///     Gets or sets the hot spot of the cursor, i.e., the relative offset to the texture's origin where the cursor's
		///     click location lies.
		/// </summary>
		public Vector2 HotSpot { get; set; }

		/// <summary>
		///     Gets or sets the texture that defines the cursors visual appearance.
		/// </summary>
		public Texture2D Texture
		{
			get { return _texture; }
			set
			{
				Assert.ArgumentNotNull(value);
				_texture = value;
			}
		}

		/// <summary>
		///     Gets the cursor that is displayed when the mouse hovers the UI element or any of its children.
		/// </summary>
		/// <param name="element">The UI element the cursor should be returned for.</param>
		public static Cursor GetCursor(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(CursorProperty);
		}

		/// <summary>
		///     Sets the cursor that should be displayed when the mouse hovers the UI element or any of its children.
		/// </summary>
		/// <param name="element">The UI element the cursor should be set for.</param>
		/// <param name="cursor">The cursor that should be displayed when the mouse hovers the UI element or any of its children.</param>
		public static void SetCursor(UIElement element, Cursor cursor)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentNotNull(cursor);

			element.SetValue(CursorProperty, cursor);
		}

		/// <summary>
		///     Draws the cursor at the given position.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the cursor.</param>
		/// <param name="position">The position the cursor should be drawn at.</param>
		internal void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			Assert.ArgumentNotNull(spriteBatch);
			Assert.ArgumentInRange(HotSpot.X, 0, Texture.Width);
			Assert.ArgumentInRange(HotSpot.Y, 0, Texture.Height);

			position = position - HotSpot;
			spriteBatch.Layer = Int32.MaxValue;
			spriteBatch.Draw(Texture, new Vector2(position.X, position.Y), Color);
		}
	}
}