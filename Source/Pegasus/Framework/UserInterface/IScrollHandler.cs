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
		///     Gets the current scroll offset of the scroll handler.
		/// </summary>
		Vector2d ScrollOffset { get; }
	}
}