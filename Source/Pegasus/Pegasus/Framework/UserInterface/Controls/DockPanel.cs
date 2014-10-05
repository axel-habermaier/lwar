namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;

	/// <summary>
	///     A panel that arranges its children both vertically and horizontally.
	/// </summary>
	public class DockPanel : Panel
	{
		/// <summary>
		///     Indicates the position of a UI child element within a dock panel.
		/// </summary>
		public static readonly DependencyProperty<Dock> DockProperty =
			new DependencyProperty<Dock>(defaultValue: Dock.Left, affectsMeasure: true);

		/// <summary>
		///     A value indicating whether the last child element within the dock panel fills the remaining available space.
		/// </summary>
		public static readonly DependencyProperty<bool> LastChildFillProperty =
			new DependencyProperty<bool>(defaultValue: true, affectsMeasure: true);

		/// <summary>
		///     Gets or sets a value indicating whether the last child element within the dock panel fills the remaining available
		///     space.
		/// </summary>
		public bool LastChildFill
		{
			get { return GetValue(LastChildFillProperty); }
			set { SetValue(LastChildFillProperty, value); }
		}

		/// <summary>
		///     Gets or sets the position of the dock panel within another dock panel.
		/// </summary>
		public Dock Dock
		{
			get { return GetValue(DockProperty); }
			set { SetValue(DockProperty, value); }
		}

		/// <summary>
		///     Gets the position of the given UI element within a dock panel.
		/// </summary>
		/// <param name="element">The UI element that should be checked.</param>
		public static Dock GetDock(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(DockProperty);
		}

		/// <summary>
		///     Sets the position of the given UI element within a dock panel.
		/// </summary>
		/// <param name="element">The UI element whose docking position should be set.</param>
		/// <param name="dock">The docking position of the UI element.</param>
		public static void SetDock(UIElement element, Dock dock)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentInRange(dock);

			element.SetValue(DockProperty, dock);
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
			// The desired size of the dock panel itself
			var dockSize = new Size();

			// The accumulated sizes of the children in the direction they actually take up space
			var accumulatedSize = new Size();

			foreach (var child in Children)
			{
				// The available size for the child is what we've got left after allocating some of the available space to
				// the previously enumerated children
				var availableChildSize = new Size(
					Math.Max(0, availableSize.Width - accumulatedSize.Width),
					Math.Max(0, availableSize.Height - accumulatedSize.Height));

				child.Measure(availableChildSize);
				var desiredChildSize = child.DesiredSize;

				// We now update the dock size and the accumulated size depending on the child's Dock value
				switch (GetDock(child))
				{
					case Dock.Top:
					case Dock.Bottom:
						dockSize.Width = Math.Max(dockSize.Width, accumulatedSize.Width + desiredChildSize.Width);
						accumulatedSize.Height += desiredChildSize.Height;
						break;
					case Dock.Left:
					case Dock.Right:
						dockSize.Height = Math.Max(dockSize.Height, accumulatedSize.Height + desiredChildSize.Height);
						accumulatedSize.Width += desiredChildSize.Width;
						break;
					default:
						Assert.NotReached("Unknown dock type.");
						break;
				}
			}

			return new Size(
				Math.Max(dockSize.Width, accumulatedSize.Width),
				Math.Max(dockSize.Height, accumulatedSize.Height));
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
			var accumulatedLeft = 0.0f;
			var accumulatedTop = 0.0f;
			var accumulatedRight = 0.0f;
			var accumulatedBottom = 0.0f;

			var count = Children.Count;
			var i = 0;
			foreach (var child in Children)
			{
				var desiredChildSize = child.DesiredSize;
				var childArea = new Rectangle(
					accumulatedLeft,
					accumulatedTop,
					Math.Max(0.0f, finalSize.Width - (accumulatedLeft + accumulatedRight)),
					Math.Max(0.0f, finalSize.Height - (accumulatedTop + accumulatedBottom)));

				if (i != count - 1 || LastChildFill)
				{
					switch (GetDock(child))
					{
						case Dock.Top:
							accumulatedTop += desiredChildSize.Height;
							childArea.Height = desiredChildSize.Height;
							break;
						case Dock.Bottom:
							accumulatedBottom += desiredChildSize.Height;
							childArea.Top = Math.Max(0.0f, finalSize.Height - accumulatedBottom);
							childArea.Height = desiredChildSize.Height;
							break;
						case Dock.Left:
							accumulatedLeft += desiredChildSize.Width;
							childArea.Width = desiredChildSize.Width;
							break;

						case Dock.Right:
							accumulatedRight += desiredChildSize.Width;
							childArea.Left = Math.Max(0.0f, finalSize.Width - accumulatedRight);
							childArea.Width = desiredChildSize.Width;
							break;
					}
				}

				child.Arrange(childArea);
				++i;
			}

			return finalSize;
		}
	}
}