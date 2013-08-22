using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Untyped base class for control templates that specify the visual appearance and structure of a control.
	/// </summary>
	public abstract class ControlTemplate
	{
		/// <summary>
		///   Instantiates the template and returns the the root of the instantiated visual tree.
		/// </summary>
		/// <param name="control">The control the template should be instantiated for.</param>
		internal abstract UIElement Instantiate(Control control);
	}
}