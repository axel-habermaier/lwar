namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Assets;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Provides access to the default cursors.
	/// </summary>
	public static class Cursors
	{
		/// <summary>
		///     Gets the arrow cursor.
		/// </summary>
		public static Cursor Arrow { get; private set; }

		/// <summary>
		///     Gets the text insertion cursor.
		/// </summary>
		public static Cursor Text { get; private set; }

		/// <summary>
		///     Loads the default cursors.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the cursors.</param>
		internal static void Load(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);

			Arrow = new Cursor(assets.Load(Textures.PointerCursor), Vector2.Zero, Color.White);
			Text = new Cursor(assets.Load(Textures.TextCursor), new Vector2(7, 7), Color.White);
		}
	}
}