namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents a scrollable area.
	/// </summary>
	public class ScrollViewer : ContentControl, IScrollHandler
	{
		/// <summary>
		///     The scroll controller that can be used to controll the scroll viewer.
		/// </summary>
		public static readonly DependencyProperty<IScrollController> ScrollControllerProperty =
			new DependencyProperty<IScrollController>(defaultBindingMode: BindingMode.OneWayToSource);

		/// <summary>
		///     The scroll aware child of the scroll viewer, if any.
		/// </summary>
		private IScrollAware _scrollAwareChild;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ScrollViewer()
		{
			ScrollController = new Controller(this);
		}

		/// <summary>
		///     Gets or sets the scroll controller that can be used to controll the scroll viewer.
		/// </summary>
		public IScrollController ScrollController
		{
			get { return GetValue(ScrollControllerProperty); }
			set { SetValue(ScrollControllerProperty, value); }
		}

		/// <summary>
		///     Gets the current scroll offset of the scroll handler.
		/// </summary>
		public Vector2d ScrollOffset { get; private set; }

		/// <summary>
		///     Invoked when the content has been changed.
		/// </summary>
		/// <param name="content">The new content that has been set.</param>
		protected override void OnContentChanged(object content)
		{
			var scrollAwareChild = GetScrollAwareChild(content as UIElement);

			if (_scrollAwareChild != null)
				_scrollAwareChild.ScrollHandler = null;

			_scrollAwareChild = scrollAwareChild;

			if (_scrollAwareChild != null)
				_scrollAwareChild.ScrollHandler = this;
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
		protected override bool HitTestCore(Vector2d position)
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
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			base.MeasureCore(availableSize);
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
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			base.ArrangeCore(finalSize);
			return finalSize;
		}

		protected override void OnDrawChildren(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			var useScissorTest = spriteBatch.UseScissorTest;
			var worldMatrix = spriteBatch.WorldMatrix;

			spriteBatch.UseScissorTest = true;
			spriteBatch.ScissorArea = new Rectangle(x, y, width, height);
			spriteBatch.WorldMatrix = Matrix.CreateTranslation((float)ScrollOffset.X, (float)ScrollOffset.Y, 0);

			base.OnDrawChildren(spriteBatch);

			spriteBatch.UseScissorTest = useScissorTest;
			spriteBatch.WorldMatrix = worldMatrix;
		}

		/// <summary>
		///     Represents a scroll controller for this scroll viewer.
		/// </summary>
		private class Controller : IScrollController
		{
			/// <summary>
			///     The distance of a single scroll step.
			/// </summary>
			private Vector2d _scrollStep;

			/// <summary>
			///     The scroll viewer controlled by this controller.
			/// </summary>
			private ScrollViewer _viewer;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="viewer">The scroll viewer that should be controlled.</param>
			public Controller(ScrollViewer viewer)
			{
				Assert.ArgumentNotNull(viewer);
				_viewer = viewer;
				ScrollStep = new Vector2d(1, 1);
			}

			/// <summary>
			///     Gets or sets the distance of a single scroll step.
			/// </summary>
			public Vector2d ScrollStep
			{
				get { return _scrollStep; }
				set
				{
					Assert.ArgumentSatisfies(value.X > 0, "Invalid scroll step size.");
					Assert.ArgumentSatisfies(value.Y > 0, "Invalid scroll step size.");

					_scrollStep = value;
				}
			}

			/// <summary>
			///     Scrolls up by a step.
			/// </summary>
			public void ScrollUp()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls down by a step.
			/// </summary>
			public void ScrollDown()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls left by a step.
			/// </summary>
			public void ScrollLeft()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls right by a step.
			/// </summary>
			public void ScrollRight()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls to the top of the content area.
			/// </summary>
			public void ScrollToTop()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls to the bottom of the content area.
			/// </summary>
			public void ScrollToBottom()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls to the left of the content area.
			/// </summary>
			public void ScrollToLeft()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Scrolls to the right of the content area.
			/// </summary>
			public void ScrollToRight()
			{
				throw new NotImplementedException();
			}
		}
	}
}