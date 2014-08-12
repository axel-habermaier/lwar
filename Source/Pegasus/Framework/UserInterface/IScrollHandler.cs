namespace Pegasus.Framework.UserInterface
{
	using System;
	using Math;

	/// <summary>
	///     Represents an UI element that handles scrolling of child content.
	/// </summary>
	public interface IScrollHandler
	{
		/// <summary>
		///     Gets the area the scrolled content is presented in.
		/// </summary>
		RectangleD ScrollArea { get; }
	}
}