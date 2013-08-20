using System;

namespace Pegasus.Rendering.UserInterface
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
		///   Initializes a new instance.
		/// </summary>
		public ContentControl()
		{
			AddChangedHandler(ContentProperty, OnContentChanged);
		}

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

		/// <summary>
		///   Updates the logical and visual parents of the new and old content.
		/// </summary>
		private void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
			var oldContent = args.OldValue as UIElement;
			var newContent = args.NewValue as UIElement;

			if (oldContent != null)
				oldContent.ChangeLogicalParent(null);

			if (newContent != null)
				newContent.ChangeLogicalParent(this);
		}
	}
}