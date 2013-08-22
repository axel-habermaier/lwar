using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Specifies the visual appearance and structure of a control of the given type.
	/// </summary>
	/// <typeparam name="T">The type of the control the template is defined for.</typeparam>
	public sealed class ControlTemplate<T> : ControlTemplate
		where T : Control
	{
		/// <summary>
		///   Creates a visual tree of UI elements that represents an instance of the template.
		/// </summary>
		/// <param name="control">The control the template is instantiated for.</param>
		public delegate UIElement TemplateCreator(T control);

		/// <summary>
		///   The creator function that can be used to create a visual tree of UI elements that
		///   represents an instance of the template.
		/// </summary>
		private readonly TemplateCreator _templateCreator;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="templateCreator">
		///   The creator function that should be used to create a visual tree of UI elements that
		///   represents an instance of the template.
		/// </param>
		public ControlTemplate(TemplateCreator templateCreator)
		{
			Assert.ArgumentNotNull(templateCreator);
			_templateCreator = templateCreator;
		}

		/// <summary>
		///   Instantiates the template and returns the the root of the instantiated visual tree.
		/// </summary>
		/// <param name="control">The control the template should be instantiated for.</param>
		internal override UIElement Instantiate(Control control)
		{
			Assert.ArgumentNotNull(control);
			Assert.ArgumentOfType<T>(control);

			return _templateCreator(control as T);
		}
	}
}