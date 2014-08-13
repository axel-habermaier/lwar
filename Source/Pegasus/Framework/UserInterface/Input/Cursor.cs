namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Assets;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents the mouse cursor.
	/// </summary>
	public class Cursor
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="texture">The texture that defines the cursors visual appearance.</param>
		/// <param name="hotSpot">
		///     The hot spot of the cursor, i.e., the relative offset to the texture's origin where the cursor's click
		///     location lies.
		/// </param>
		public Cursor(Texture2D texture, Vector2i hotSpot)
		{
			Assert.ArgumentNotNull(texture);
			Assert.ArgumentInRange(hotSpot.X, 0, texture.Width);
			Assert.ArgumentInRange(hotSpot.Y, 0, texture.Height);

			Texture = texture;
			HotSpot = hotSpot;
		}

		/// <summary>
		///     Gets the arrow cursor.
		/// </summary>
		public static Cursor Arrow { get; private set; }

		/// <summary>
		///     Gets the text insertion cursor.
		/// </summary>
		public static Cursor Text { get; private set; }

		/// <summary>
		///     Gets the texture that defines the cursors visual appearance.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		///     Gets the hot spot of the cursor, i.e., the relative offset to the texture's origin where the cursor's
		///     click location lies.
		/// </summary>
		public Vector2i HotSpot { get; private set; }

		/// <summary>
		///     Loads the default cursors.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the cursors.</param>
		internal static void LoadCursors(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);

			Arrow = new Cursor(assets.Load(Cursors.Pointer), Vector2i.Zero);
			Text = new Cursor(assets.Load(Cursors.Text), new Vector2i(7, 7));
		}
	}
}