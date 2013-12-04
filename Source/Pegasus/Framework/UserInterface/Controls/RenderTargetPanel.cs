namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Draws the contents of a render target into the UI. A render target panel can therefore be used to draw non-UI 2D
	///     and 3D content into the UI.
	/// </summary>
	public class RenderTargetPanel : UIElement
	{
		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get { return UIElementCollection.Enumerator.Empty; }
		}

		/// <summary>
		///     Gets or sets the render target that is rendered into the UI.
		/// </summary>
		public RenderTarget RenderTarget { get; set; }

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
			return new SizeD(100, 100);
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
			return new SizeD(100, 100);
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			// ?
		}
	}
}