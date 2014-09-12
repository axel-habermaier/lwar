namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Allows each child to take up the entire area of the panel.
	/// </summary>
	public sealed class AreaPanel : Panel
	{
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
		///     Draws the child UI elements of the current UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element's children.</param>
		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			var count = VisualChildrenCount;
			for (var i = 0; i < count; ++i)
			{
				// We draw each child on its own range of layers, as the children of an area panel are typically stacked
				// along the Z axis and therefore overlap each other.
				spriteBatch.Layer += 10000;

				var child = GetVisualChild(i);
				child.Draw(spriteBatch);
			}
		}
	}
}