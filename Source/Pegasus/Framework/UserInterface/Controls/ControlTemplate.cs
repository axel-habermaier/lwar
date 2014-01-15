namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Specifies the visual appearance and structure of a control of the given type.
	/// </summary>
	/// <param name="control">The control the template should be instantiated for.</param>
	/// <returns>Returns the root element of the visual tree created by the template.</returns>
	public delegate UIElement ControlTemplate(Control control);
}