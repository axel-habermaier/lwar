namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Indicates where an UI element should be displayed vertically relative to its parent element.
	/// </summary>
	public enum VerticalAlignment : byte
	{
		/// <summary>
		///     Indicates that the entire vertical space of the parent element is consumed.
		/// </summary>
		Stretch,

		/// <summary>
		///     Indicates that the element is aligned to the top of the parent element's space.
		/// </summary>
		Top,

		/// <summary>
		///     Indicates that the element is centered vertically in the parent element's space.
		/// </summary>
		Center,

		/// <summary>
		///     Indicates that the element is aligned to the bottom of the parent element's space.
		/// </summary>
		Bottom
	}
}