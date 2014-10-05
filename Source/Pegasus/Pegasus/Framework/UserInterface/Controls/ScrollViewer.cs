namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents a scrollable area.
	/// </summary>
	public class ScrollViewer : ContentControl, IScrollHandler
	{
		/// <summary>
		///     The scroll controller that can be used to control the scroll viewer.
		/// </summary>
		public static readonly DependencyProperty<IScrollController> ScrollControllerProperty =
			new DependencyProperty<IScrollController>(defaultBindingMode: BindingMode.OneWayToSource);

		/// <summary>
		///     The horizontal scroll speed.
		/// </summary>
		public static readonly DependencyProperty<float> HorizontalScrollStepProperty =
			new DependencyProperty<float>(defaultValue: 10, validationCallback: ValidateScrollStep);

		/// <summary>
		///     The vertical scroll speed.
		/// </summary>
		public static readonly DependencyProperty<float> VerticalScrollStepProperty =
			new DependencyProperty<float>(defaultValue: 10, validationCallback: ValidateScrollStep);

		/// <summary>
		///     The size of the arranged scrolled content.
		/// </summary>
		private Size _arrangedContentSize;

		/// <summary>
		///     The measured size of the scrolled content.
		/// </summary>
		private Size _measuredContentSize;

		/// <summary>
		///     The scroll aware child of the scroll viewer, if any.
		/// </summary>
		private IScrollAware _scrollAwareChild;

		/// <summary>
		///     The current scroll offset.
		/// </summary>
		private Vector2 _scrollOffset;

		/// <summary>
		///     The scrolled child.
		/// </summary>
		private UIElement _scrolledChild;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ScrollViewer()
		{
			ActualWidthProperty.Changed += OnSizeChanged;
			ActualHeightProperty.Changed += OnSizeChanged;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ScrollViewer()
		{
			ScrollController = new Controller(this);
		}

		/// <summary>
		///     Gets or sets the horizontal scroll speed.
		/// </summary>
		public float HorizontalScrollStep
		{
			get { return GetValue(HorizontalScrollStepProperty); }
			set { SetValue(HorizontalScrollStepProperty, value); }
		}

		/// <summary>
		///     Gets or sets the vertical scroll speed.
		/// </summary>
		public float VerticalScrollStep
		{
			get { return GetValue(VerticalScrollStepProperty); }
			set { SetValue(VerticalScrollStepProperty, value); }
		}

		/// <summary>
		///     Gets or sets the scroll controller that can be used to control the scroll viewer.
		/// </summary>
		public IScrollController ScrollController
		{
			get { return GetValue(ScrollControllerProperty); }
			set { SetValue(ScrollControllerProperty, value); }
		}

		/// <summary>
		///     Gets or sets the scroll offset, taking the alignment of the scroll child into account.
		/// </summary>
		private Vector2 ScrollOffset
		{
			get
			{
				var offset = _scrollOffset;

				if (_scrolledChild != null && _scrolledChild.HorizontalAlignment == HorizontalAlignment.Right)
					offset.X = _scrolledChild.ActualWidth - ActualWidth - offset.X;

				if (_scrolledChild != null && _scrolledChild.VerticalAlignment == VerticalAlignment.Bottom)
					offset.Y = _scrolledChild.ActualHeight - ActualHeight - offset.Y;

				return offset;
			}
		}

		/// <summary>
		///     Gets the area the scrolled content is presented in.
		/// </summary>
		public Rectangle ScrollArea
		{
			get { return new Rectangle(VisualOffset + ScrollOffset, ActualWidth, ActualHeight); }
		}

		/// <summary>
		///     Ensures that the scroll offset is still valid after a size changed. When the scroll viewer's size is increased, more
		///     content fits into its scroll area, possibly resulting in the scroll viewer displaying nothing if the scroll offset
		///     is not reclamped to the new size.
		/// </summary>
		private static void OnSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<float> args)
		{
			var scrollViewer = obj as ScrollViewer;
			if (scrollViewer != null)
				scrollViewer.Scroll(Vector2.Zero);
		}

		/// <summary>
		///     Validates a scroll step value.
		/// </summary>
		/// <param name="value">The value that should be checked.</param>
		private static bool ValidateScrollStep(float value)
		{
			return value > 0;
		}

		/// <summary>
		///     Invoked when the content has been changed.
		/// </summary>
		/// <param name="content">The new content that has been set.</param>
		protected override void OnContentChanged(object content)
		{
			_scrolledChild = content as UIElement;
			var scrollAwareChild = GetScrollAwareChild(_scrolledChild);

			if (_scrollAwareChild != null)
				_scrollAwareChild.ScrollHandler = null;

			_scrollAwareChild = scrollAwareChild;

			if (_scrollAwareChild != null)
				_scrollAwareChild.ScrollHandler = this;
		}

		/// <summary>
		///     Scrolls the content by the given delta.
		/// </summary>
		/// <param name="delta">The delta in the scrolling position.</param>
		private void Scroll(Vector2 delta)
		{
			if (_scrolledChild != null && _scrolledChild.VerticalAlignment == VerticalAlignment.Bottom)
				delta.Y *= -1;

			var offset = _scrollOffset + delta;
			var width = _arrangedContentSize.Width;
			var height = _arrangedContentSize.Height;

			// Clamp to the range
			offset.X = MathUtils.Clamp(offset.X, 0, Math.Max(0, width - HorizontalScrollStep));
			offset.Y = MathUtils.Clamp(offset.Y, 0, Math.Max(0, height - VerticalScrollStep));

			_scrollOffset = offset;
		}

		/// <summary>
		///     Gets the scroll aware child of the scroll viewer, if any.
		/// </summary>
		/// <param name="element">The element the scroll aware child should be searched in.</param>
		private static IScrollAware GetScrollAwareChild(UIElement element)
		{
			while (true)
			{
				if (element == null)
					return null;

				var scrollAware = element as IScrollAware;
				if (scrollAware != null)
					return scrollAware;

				// Otherwise, descend the tree until we find a control with more than one child; in that case,
				// there can be no scroll aware control for this scroll viewer anymore.
				var childCount = element.VisualChildrenCount;
				if (childCount != 1)
					return null;

				element = element.GetVisualChild(0);
			}
		}

		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's
		///     bounds. This method should be overridden to implement special hit testing logic that is more precise than a
		///     simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2 position)
		{
			return true;
		}

		/// <summary>
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override Size MeasureCore(Size availableSize)
		{
			_measuredContentSize = base.MeasureCore(availableSize);
			return availableSize;
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
		protected override Size ArrangeCore(Size finalSize)
		{
			_arrangedContentSize = base.ArrangeCore(_measuredContentSize);
			return finalSize;
		}

		/// <summary>
		///     Draws the child UI elements of the current UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element's children.</param>
		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			var useScissorTest = spriteBatch.UseScissorTest;
			var worldMatrix = spriteBatch.WorldMatrix;

			spriteBatch.UseScissorTest = true;
			spriteBatch.ScissorArea = new Rectangle(x, y, width, height);

			var offset = ScrollOffset;
			spriteBatch.WorldMatrix = Matrix.CreateTranslation(-(int)Math.Round(offset.X), -(int)Math.Round(offset.Y), 0);

			base.DrawChildren(spriteBatch);

			spriteBatch.UseScissorTest = useScissorTest;
			spriteBatch.WorldMatrix = worldMatrix;
		}

		/// <summary>
		///     Represents a scroll controller for this scroll viewer.
		/// </summary>
		private class Controller : IScrollController
		{
			/// <summary>
			///     The scroll viewer controlled by this controller.
			/// </summary>
			private readonly ScrollViewer _viewer;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="viewer">The scroll viewer that should be controlled.</param>
			public Controller(ScrollViewer viewer)
			{
				Assert.ArgumentNotNull(viewer);
				_viewer = viewer;
			}

			/// <summary>
			///     Scrolls up by a step.
			/// </summary>
			public void ScrollUp()
			{
				_viewer.Scroll(new Vector2(0, -_viewer.VerticalScrollStep));
			}

			/// <summary>
			///     Scrolls down by a step.
			/// </summary>
			public void ScrollDown()
			{
				_viewer.Scroll(new Vector2(0, _viewer.VerticalScrollStep));
			}

			/// <summary>
			///     Scrolls left by a step.
			/// </summary>
			public void ScrollLeft()
			{
				_viewer.Scroll(new Vector2(-_viewer.HorizontalScrollStep, 0));
			}

			/// <summary>
			///     Scrolls right by a step.
			/// </summary>
			public void ScrollRight()
			{
				_viewer.Scroll(new Vector2(_viewer.HorizontalScrollStep, 0));
			}

			/// <summary>
			///     Scrolls to the top of the content area.
			/// </summary>
			public void ScrollToTop()
			{
				_viewer.Scroll(new Vector2(0, Single.NegativeInfinity));
			}

			/// <summary>
			///     Scrolls to the bottom of the content area.
			/// </summary>
			public void ScrollToBottom()
			{
				_viewer.Scroll(new Vector2(0, Single.PositiveInfinity));
			}

			/// <summary>
			///     Scrolls to the left of the content area.
			/// </summary>
			public void ScrollToLeft()
			{
				_viewer.Scroll(new Vector2(Single.NegativeInfinity, 0));
			}

			/// <summary>
			///     Scrolls to the right of the content area.
			/// </summary>
			public void ScrollToRight()
			{
				_viewer.Scroll(new Vector2(Single.PositiveInfinity, 0));
			}
		}
	}
}