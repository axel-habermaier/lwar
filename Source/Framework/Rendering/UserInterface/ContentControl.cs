using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Represents a control with a single logical child of any type as its content.
	/// </summary>
	public class ContentControl : Control
	{
		/// <summary>
		///   The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty = new DependencyProperty<object>();

		/// <summary>
		///   Gets or sets the content of the content control.
		/// </summary>
		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ContentControl()
		{
			AddChangedHandler(ContentProperty, OnContentChanged);
		}

		private void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
		}
	}
}