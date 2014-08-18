namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Specifies the visiblity of an UI element.
	/// </summary>
	public enum Visibility
	{
		/// <summary>
		///     Indicates that the element is displayed.
		/// </summary>
		Visible,

		/// <summary>
		///     Indicates that the element is not displayed and the layouting mechanism does not reserve space for it.
		/// </summary>
		Collapsed,

		/// <summary>
		///     Indicates that the element is not displayed, but the layouting mechanism reserves space for it.
		/// </summary>
		Hidden,
	}
}