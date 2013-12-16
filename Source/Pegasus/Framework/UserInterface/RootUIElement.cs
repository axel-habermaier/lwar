namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents the root element of all visual trees within an application.
	/// </summary>
	internal class RootUIElement : UIElement
	{
		/// <summary>
		///     The collection of windows attached to the root element.
		/// </summary>
		private readonly UIElementCollection _windows;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RootUIElement()
		{
			IsConnectedToRoot = true;
			_windows = new UIElementCollection(this);
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (_windows == null)
					return UIElementCollection.Enumerator.Empty;

				return _windows.GetEnumerator();
			}
		}

		/// <summary>
		///     Gets the number of visual children for this visual.
		/// </summary>
		protected internal override int VisualChildrenCount
		{
			get { return _windows == null ? 0 : _windows.Count; }
		}

		/// <summary>
		///     Gets a value indicating whether any of the currently open windows has the focus.
		/// </summary>
		public bool HasFocusedWindows
		{
			get
			{
				foreach (Window window in _windows)
				{
					if (window.Focused)
						return true;
				}
				return false;
			}
		}

		/// <summary>
		///     Adds the window to the visual tree.
		/// </summary>
		/// <param name="window">The window that should be added.</param>
		public void Add(Window window)
		{
			Assert.ArgumentNotNull(window);
			Assert.That(!_windows.Contains(window), "The window has already been added.");

			_windows.Add(window);
		}

		/// <summary>
		///     Removes the window from the visual tree.
		/// </summary>
		/// <param name="window">The window that should be removed.</param>
		public void Remove(Window window)
		{
			Assert.ArgumentNotNull(window);
			_windows.Remove(window);
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
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_windows);
			Assert.ArgumentInRange(index, _windows);

			return _windows[index];
		}

		/// <summary>
		///     Handles the user input.
		/// </summary>
		public void HandleInput()
		{
			foreach (Window window in _windows)
				window.ProcessEvents();
		}

		/// <summary>
		///     Updates the layout of all windows.
		/// </summary>
		public void UpdateLayout()
		{
			foreach (Window window in _windows)
				window.UpdateLayout();
		}

		/// <summary>
		///     Draws the contents of all windows.
		/// </summary>
		/// <param name="spriteBatch"></param>
		public new void Draw(SpriteBatch spriteBatch)
		{
			foreach (Window window in _windows)
				window.Draw(spriteBatch);
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			throw new NotSupportedException();
		}
	}
}