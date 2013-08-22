using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Represents a base class for control typically defined in XAML.
	/// </summary>
	public class UserControl : ContentControl
	{
		/// <summary>
		///   The default template that defines the visual appearance of a user control.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = new ControlTemplate<UserControl>(control =>
		{
			var binding = new TemplateBinding<object>(control, ContentProperty);
			var presenter = new ContentPresenter();
			presenter.SetBinding(ContentPresenter.ContentProperty, binding);

			return presenter;
		});

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UserControl()
		{
			Template = DefaultTemplate;
		}
	}
}