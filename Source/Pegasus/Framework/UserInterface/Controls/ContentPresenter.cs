using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Rendering;
	using Rendering.UserInterface;

	/// <summary>
	///   Converts arbitrary content into UI elements.
	/// </summary>
	public class ContentPresenter : UIElement
	{
		/// <summary>
		///   The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty =
			new DependencyProperty<object>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///   The presented element of the content presenter.
		/// </summary>
		private UIElement _presentedElement;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public ContentPresenter()
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
		///   Gets the number of visual children for this visual.
		/// </summary>
		protected override int VisualChildrenCount
		{
			get { return _presentedElement == null ? 0 : 1; }
		}

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (_presentedElement == null)
					return UIElementCollection.Enumerator.Empty;

				return UIElementCollection.Enumerator.FromElement(_presentedElement);
			}
		}

		/// <summary>
		///   Updates the logical and visual parents of the new and old content.
		/// </summary>
		private void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
			if (args.NewValue is UIElement)
				_presentedElement = args.NewValue as UIElement;

			if (args.NewValue == null)
				_presentedElement = null;
			else
				_presentedElement = new TextBlock(args.NewValue.ToString());
		}

		/// <summary>
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected override Visual GetVisualChild(int index)
		{
			Assert.NotNull(_presentedElement);
			Assert.ArgumentSatisfies(index == 0, "The UI element has only one visual child.");

			return _presentedElement;
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
			if (_presentedElement == null)
				return new SizeD();

			_presentedElement.Measure(constraint);
			return _presentedElement.DesiredSize;
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
			if (_presentedElement != null)
				_presentedElement.Arrange(new RectangleD(0, 0, finalSize));

			return finalSize;
		}

		public override void Draw(SpriteBatch spriteBatch, Font font)
		{
		}
	}
}