using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Represents a control with a single logical child of any type as its content.
	/// </summary>
	public class ContentControl : Control
	{
		/// <summary>
		///   The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty =
			new DependencyProperty<object>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///   Gets or sets the content of the content control.
		/// </summary>
		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the content control.
		/// </summary>
		protected override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				var child = Content as UIElement;
				if (child != null)
					return UIElementCollection.Enumerator.FromElement(child);

				return UIElementCollection.Enumerator.Empty;
			}
		}
	}
}