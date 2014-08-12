namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Represents an UI element that is aware of scrolling, requiring its parent scroll handler to be set.
	/// </summary>
	public interface IScrollAware
	{
		/// <summary>
		///     Gets or sets the scroll handler that handles thi scrolling aware UI element.
		/// </summary>
		IScrollHandler ScrollHandler { get; set; }
	}
}