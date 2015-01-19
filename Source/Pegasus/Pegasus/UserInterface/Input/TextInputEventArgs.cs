namespace Pegasus.UserInterface.Input
{
	using System;

	/// <summary>
	///     Provides information about text input..
	/// </summary>
	public class TextInputEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly TextInputEventArgs CachedInstance = new TextInputEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private TextInputEventArgs()
		{
		}

		/// <summary>
		///     Gets the text that was entered.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		///     Initializes a cached instance.
		/// </summary>
		/// <param name="character">The character that was entered.</param>
		internal static TextInputEventArgs Create(string text)
		{
			CachedInstance.Reset();
			CachedInstance.Text = text;

			return CachedInstance;
		}
	}
}