using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Represents a base class for control typically defined in Xaml.
	/// </summary>
	public class UserControl : ContentControl
	{
		/// <summary>
		///   The default template that defines the visual appearance of a user control.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control =>
		{
			var presenter = new ContentPresenter();
			presenter.CreateTemplateBinding(control, ContentProperty, ContentPresenter.ContentProperty);

			return presenter;
		};

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UserControl()
		{
			Template = DefaultTemplate;
		}
	}
}