namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Indicates where an UI element should be displayed horizontally relative to its parent element.
	/// </summary>
	public enum HorizontalAlignment : byte
	{
		/// <summary>
		///   Indicates that the entire horizontal space of the parent element is consumed.
		/// </summary>
		Stretch,

		/// <summary>
		///   Indicates that the element is aligned to the left of the parent element's space.
		/// </summary>
		Left,

		/// <summary>
		///   Indicates that the element is centered horizontally in the parent element's space.
		/// </summary>
		Center,

		/// <summary>
		///   Indicates that the element is aligned to the right of the parent element's space.
		/// </summary>
		Right
	}
}