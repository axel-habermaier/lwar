namespace Pegasus.UserInterface.Input
{
	using System;
	using Assets;
	using Utilities;

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
		///     Initializes the default cursors.
		/// </summary>
		/// <param name="bundle">The bundle that should be used to initialize the cursors.</param>
		internal static void Initialize(MainBundle bundle)
		{
			Assert.ArgumentNotNull(bundle);

			Arrow = bundle.PointerCursor;
			Text = bundle.TextCursor;
		}
	}
}