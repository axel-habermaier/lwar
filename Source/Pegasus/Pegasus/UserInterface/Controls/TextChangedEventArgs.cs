namespace Pegasus.UserInterface.Controls
{
	using System;
	using Utilities;

	/// <summary>
	///     Provides information about text change events.
	/// </summary>
	public class TextChangedEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly TextChangedEventArgs CachedInstance = new TextChangedEventArgs();

		/// <summary>
		///     Gets the new text after the change has been made.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		///     Initializes a cached instance.
		/// </summary>
		/// <param name="text">The new text after the change has been made.</param>
		internal static TextChangedEventArgs Create(string text)
		{
			Assert.ArgumentNotNull(text);

			CachedInstance.Reset();
			CachedInstance.Text = text;

			return CachedInstance;
		}
	}
}