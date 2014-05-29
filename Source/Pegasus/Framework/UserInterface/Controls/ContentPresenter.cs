namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Rendering;

	/// <summary>
	///     Displays arbitrary content in the UI.
	/// </summary>
	public sealed class ContentPresenter : UIElement
	{
		/// <summary>
		///     The content of a content control.
		/// </summary>
		public static readonly DependencyProperty<object> ContentProperty =
			new DependencyProperty<object>(affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     The presented element of the content presenter.
		/// </summary>
		private UIElement _presentedElement;

		/// <summary>
		///     A cached text block instance that the presenter can use to present text content.
		/// </summary>
		private TextBlock _textBlock;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ContentPresenter()
		{
			ContentProperty.Changed += OnContentChanged;
		}

		/// <summary>
		///     Gets or sets the content of the content control.
		/// </summary>
		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		/// <summary>
		///     Gets the number of visual children for this visual.
		/// </summary>
		protected internal override int VisualChildrenCount
		{
			get { return _presentedElement == null ? 0 : 1; }
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (_presentedElement == null)
					return UIElementCollection.Enumerator.Empty;

				return UIElementCollection.Enumerator.FromElement(_presentedElement);
			}
		}

		/// <summary>
		///     Updates the logical and visual parents of the new and old content.
		/// </summary>
		private static void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
			var contentPresenter = obj as ContentPresenter;
			if (contentPresenter == null)
				return;

			var previousElement = contentPresenter._presentedElement;

			if (args.NewValue is UIElement)
				contentPresenter._presentedElement = args.NewValue as UIElement;
			else if (args.NewValue == null)
				contentPresenter._presentedElement = null;
			else if (contentPresenter._textBlock == null)
				contentPresenter._presentedElement = contentPresenter._textBlock = new TextBlock(args.NewValue.ToString());
			else
			{
				// Reuse the previous text block instance
				contentPresenter._textBlock.Text = args.NewValue.ToString();
				previousElement = null; // No need to remove the text block from the logical tree
			}

			if (previousElement != null)
				previousElement.ChangeLogicalParent(null);

			if (contentPresenter._presentedElement != null)
				contentPresenter._presentedElement.ChangeLogicalParent(contentPresenter.Parent);
		}

		/// <summary>
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_presentedElement);
			Assert.ArgumentSatisfies(index == 0, "The UI element has only one visual child.");

			return _presentedElement;
		}

		/// <summary>
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			if (_presentedElement == null)
				return new SizeD();

			_presentedElement.Measure(availableSize);
			return _presentedElement.DesiredSize;
		}

		/// <summary>
		///     Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///     element. If this value is smaller than the given size, the UI element's alignment properties position it
		///     appropriately.
		/// </summary>
		/// <param name="finalSize">
		///     The final area allocated by the UI element's parent that the UI element should use to arrange
		///     itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			if (_presentedElement == null)
				return new SizeD();

			_presentedElement.Arrange(new RectangleD(0, 0, finalSize));
			return _presentedElement.RenderSize;
		}

		/// <summary>
		///     Invoked when the UI element is attached to a new logical tree.
		/// </summary>
		protected override void OnAttached()
		{
			if (_presentedElement != null)
				_presentedElement.ChangeLogicalParent(Parent);
		}

		/// <summary>
		///     Invoked when the UI element has been detached from its current logical tree.
		/// </summary>
		protected override void OnDetached()
		{
			if (_presentedElement != null)
				_presentedElement.ChangeLogicalParent(null);
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
		}
	}
}