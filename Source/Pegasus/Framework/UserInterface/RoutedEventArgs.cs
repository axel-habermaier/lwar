namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Associates common data to a routed event that has been raised.
	/// </summary>
	public class RoutedEventArgs
	{
		/// <summary>
		///     The cached default instance of the routed event args.
		/// </summary>
		private static readonly RoutedEventArgs CachedInstance = new RoutedEventArgs();

		/// <summary>
		///     Gets the cached default instance of the routed event args.
		/// </summary>
		public static RoutedEventArgs Default
		{
			get
			{
				CachedInstance.Reset();
				return CachedInstance;
			}
		}

		/// <summary>
		///     Gets the UI element the event originated from.
		/// </summary>
		public UIElement Source { get; internal set; }

		/// <summary>
		///     Gets the routed event that has been raised.
		/// </summary>
		public RoutedEvent RoutedEvent { get; internal set; }

		/// <summary>
		///     Gets or sets a value indicating whether the routed event has already been handled.
		/// </summary>
		public bool Handled { get; set; }

		/// <summary>
		///     Resets all default properties of the routed event args.
		/// </summary>
		protected void Reset()
		{
			Source = null;
			RoutedEvent = null;
			Handled = false;
		}
	}
}