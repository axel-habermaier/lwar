namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Math;
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
		}

		/// <summary>
		///     Gets a value indicating whether any of the currently open windows has the operating system focus.
		/// </summary>
		public bool HasFocusedWindows
		{
			get
			{
				foreach (var child in LogicalChildren)
				{
					Assert.OfType<Window>(child);

					var window = child as Window;
					if (window.OsFocused)
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

		/// <summary>
		///     Presents the contents of all windows' backbuffers.
		/// </summary>
		public void Present()
		{
			foreach (var child in LogicalChildren)
			{
				Assert.OfType<Window>(child);

				var window = child as Window;
				window.Present();
			}
		}

		/// <summary>
		///     Not supported by root UI elements.
		/// </summary>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			throw new NotSupportedException("Call UpdateLayout() instead.");
		}

		/// <summary>
		///     Not supported by root UI elements.
		/// </summary>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			throw new NotSupportedException("Call UpdateLayout() instead.");
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			throw new NotSupportedException("Call Draw() instead.");
		}
	}
}