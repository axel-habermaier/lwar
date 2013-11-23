﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///   Represents a control with a single logical child of any type as its content.
	/// </summary>
	[ContentProperty("Content")]
	public class ContentControl : Control
	{
		/// <summary>
		///   The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty =
			new DependencyProperty<object>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///   The default template that defines the visual appearance of a user control.
		/// </summary>
		internal static readonly ControlTemplate DefaultTemplate = control =>
		{
			var presenter = new ContentPresenter();
			presenter.CreateTemplateBinding(control, ContentProperty, ContentPresenter.ContentProperty);

			return presenter;
		};

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ContentControl()
		{
			// We set an implicit default template as a style value. This is an inexpensive way of setting
			// an implicit default style for all content controls. Using SetValue(), on the other hand,
			// would take precedence over all templates set by a style
			SetStyleValue(TemplateProperty, DefaultTemplate);
		}

		/// <summary>
		///   Gets or sets the content of the content control.
		/// </summary>
		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
}