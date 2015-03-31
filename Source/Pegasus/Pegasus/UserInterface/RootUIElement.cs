namespace Pegasus.UserInterface
{
	using System;
	using Controls;
	using Math;
	using Platform;
	using Rendering;

	/// <summary>
	///     Represents the root element of all visual trees within an application.
	/// </summary>
	internal sealed class RootUIElement : Panel
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RootUIElement()
		{
			IsAttachedToRoot = true;
			SetReadOnlyValue(IsVisibleProperty, true);
		}

		/// <summary>
		///     Gets a value indicating whether any of the currently open windows has the operating system focus.
		/// </summary>
		public bool HasFocusedWindows
		{
			get
			{
				foreach (Window window in LogicalChildren)
				{
					if (window.Focused)
						return true;
				}
				return false;
			}
		}

		/// <summary>
		///     Handles the user input.
		/// </summary>
		public void HandleInput()
		{
			NativeMethods.HandleEventMessages();

			foreach (Window window  in LogicalChildren)
				window.HandleInput();
		}

		/// <summary>
		///     Updates the layout of all windows.
		/// </summary>
		public void UpdateLayout()
		{
			foreach (Window window  in LogicalChildren)
				window.UpdateLayout();
		}

		/// <summary>
		///     Draws the contents of all windows.
		/// </summary>
		public void Draw()
		{
			foreach (Window window  in LogicalChildren)
				window.Draw();
		}

		/// <summary>
		///     Presents the contents of all windows' backbuffers.
		/// </summary>
		public void Present()
		{
			foreach (Window window in LogicalChildren)
				window.Present();
		}

		/// <summary>
		///     Not supported by root UI elements.
		/// </summary>
		protected override Size MeasureCore(Size availableSize)
		{
			throw new NotSupportedException("Call UpdateLayout() instead.");
		}

		/// <summary>
		///     Not supported by root UI elements.
		/// </summary>
		protected override Size ArrangeCore(Size finalSize)
		{
			throw new NotSupportedException("Call UpdateLayout() instead.");
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			throw new NotSupportedException("Call Draw() instead.");
		}
	}
}