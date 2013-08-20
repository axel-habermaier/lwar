using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Represents a base class for templated UI elements.
	/// </summary>
	public abstract class Control : UIElement
	{
		/// <summary>
		///   The template that defines the control's appearance.
		/// </summary>
		public static readonly DependencyProperty<ControlTemplate> TemplateProperty = new DependencyProperty<ControlTemplate>();

		/// <summary>
		///   Gets or sets the template that defines the control's appearance.
		/// </summary>
		public ControlTemplate Template
		{
			get { return GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
	}
}