﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a control with a single logical child of any type as its content.
	/// </summary>
	public class ContentControl : Control
	{
		/// <summary>
		///     The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty =
			new DependencyProperty<object>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     The default template that defines the visual appearance of a content control.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control =>
		{
			var presenter = new ContentPresenter();
			presenter.CreateTemplateBinding(control, ContentProperty, ContentPresenter.ContentProperty);

			return presenter;
		};

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ContentControl()
		{
			SetStyleValue(TemplateProperty, DefaultTemplate);
		}

		/// <summary>
		///     Gets or sets the content of the content control.
		/// </summary>
		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
}