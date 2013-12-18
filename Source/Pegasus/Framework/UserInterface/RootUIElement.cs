namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents the root element of all visual trees within an application.
	/// </summary>
	internal class RootUIElement : Panel
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RootUIElement()
		{
			IsAttachedToRoot = true;
		}

		/// <summary>
		///     Gets a value indicating whether any of the currently open windows has the focus.
		/// </summary>
		public bool HasFocusedWindows
		{
			get
			{
				foreach (var child in LogicalChildren)
				{
					Assert.OfType<Window>(child);

					var window = child as Window;
					if (window.Focused)
						return true;
				}
				return false;
			}
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
			throw new NotSupportedException();
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
			throw new NotSupportedException();
		}

		/// <summary>
		///     Handles the user input.
		/// </summary>
		public void HandleInput()
		{
			foreach (var child in LogicalChildren)
			{
				Assert.OfType<Window>(child);

				var window = child as Window;
				window.HandleInput();
			}
		}

		/// <summary>
		///     Updates the layout of all windows.
		/// </summary>
		public void UpdateLayout()
		{
			foreach (var child in LogicalChildren)
			{
				Assert.OfType<Window>(child);

				var window = child as Window;
				window.UpdateLayout();
			}
		}

		/// <summary>
		///     Draws the contents of all windows.
		/// </summary>
		public void Draw()
		{
			foreach (var child in LogicalChildren)
			{
				Assert.OfType<Window>(child);

				var window = child as Window;
				window.Draw();
			}
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			throw new NotSupportedException("Call Draw() instead.");
		}
	}
}