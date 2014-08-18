namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     An interface for controls that allow text input. While such a control has keyboard focus, all input triggers and
	///     command bindings are deactivated.
	/// </summary>
	public interface ITextInputControl
	{
		/// <summary>
		///     Gets or sets the text of the input control.
		/// </summary>
		string Text { get; set; }
	}
}