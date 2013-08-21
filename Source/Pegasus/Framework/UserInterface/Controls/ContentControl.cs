using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Rendering;
	using Rendering.UserInterface;

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

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="constraint">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD constraint)
		{
			if (Content == null)
				return new SizeD();

			var uiElement = Content as UIElement;
			if (uiElement != null)
			{
				uiElement.Measure(constraint);
				return uiElement.DesiredSize;
			}

			var s = Content as string;
			return new SizeD(s.Length * 10, 14);// TODO
		}

		/// <summary>
		///   Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///   element. If this value is smaller than the given size, the UI element's alignment properties position it
		///   appropriately.
		/// </summary>
		/// <param name="finalSize">
		///   The final area allocated by the UI element's parent that the UI element should use to arrange
		///   itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			if (Content == null)
				return new SizeD();

			var uiElement = Content as UIElement;
			if (uiElement != null)
			{
				uiElement.Arrange(new RectangleD(0, 0, finalSize));
				return uiElement.RenderSize;
			}

			var s = Content as string;
			return new SizeD(s.Length * 10, 14);// TODO
		}

		public override void Draw(SpriteBatch spriteBatch, Font font)
		{
			var visual = Content as Visual;
			if (visual == null)
				return;

			visual.Draw(spriteBatch, font);
		}
	}
}