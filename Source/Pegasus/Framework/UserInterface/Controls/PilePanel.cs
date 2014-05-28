namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;

	/// <summary>
	///     Stacks its children along the z-axis, allowing each child to take up the entire area of the panel.
	/// </summary>
	public sealed class PilePanel : Panel
	{
		/// <summary>
		///     The current number of modal UI elements that the layout root contains.
		/// </summary>
		private int _modalElementCount;

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
			foreach (var child in Children)
				child.Measure(availableSize);

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
			foreach (var child in Children)
				child.Arrange(new RectangleD(Vector2d.Zero, finalSize));

			return finalSize;
		}

		/// <summary>
		///     Adds the given UI element to the layout root, showing it above all other previously added non-modal UI elements.
		/// </summary>
		/// <param name="element">The UI element that should be added.</param>
		public void Add(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			Children.Add(element);
		}

		/// <summary>
		///     Removes the given UI element from the layout root.
		/// </summary>
		/// <param name="element">The UI element that should be removed.</param>
		public bool Remove(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return Children.Remove(element);
		}
	}
}