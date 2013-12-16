using System;

namespace Lwar.Screens
{
	using System;
	using System.Collections.Generic;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///   Manages several layered app screens.
	/// </summary>
	public sealed class ScreenManager : DisposableObject
	{
		/// <summary>
		///   The stack of layered game screens.
		/// </summary>
		private readonly List<Screen> _screens = new List<Screen>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="context">
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </param>
		public ScreenManager(AppContext context)
		{
			Context = context;
		}

		/// <summary>
		///   Gets the context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		internal AppContext Context { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Clear();
		}

		/// <summary>
		///   Removes all app screens from the screen manager.
		/// </summary>
		public void Clear()
		{
			// Copy the list, otherwise we would get problems if a screen modified the list during its disposal
			var screens = _screens.ToArray();
			screens.SafeDisposeAll();

			_screens.Clear();
		}

		/// <summary>
		///   Adds the app screen onto the top of the stack.
		/// </summary>
		/// <param name="appscreen">The app screen that should be added.</param>
		public void Add(Screen appscreen)
		{
			Assert.ArgumentNotNull(appscreen);

			_screens.Add(appscreen);

			appscreen.ScreenManager = this;
			appscreen.Initialize();
		}

		/// <summary>
		///   Removes the app screen from the stack.
		/// </summary>
		/// <param name="appscreen">The app screen that should be removed.</param>
		public void Remove(Screen appscreen)
		{
			Assert.ArgumentNotNull(appscreen);

			if (_screens.Remove(appscreen))
				appscreen.SafeDispose();
		}

		/// <summary>
		///   Updates all screens from top to bottom.
		/// </summary>
		public void Update()
		{
			for (var i = 0; i < _screens.Count; ++i)
				_screens[i].Update(i == _screens.Count - 1);
		}

		/// <summary>
		///   Draws all visible screens from top to bottom.
		/// </summary>
		/// <param name="output">The output that the screens should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output);

			for (var i = GetFirstVisibleScreen(); i < _screens.Count; ++i)
				_screens[i].Draw(output);
		}

		/// <summary>
		///   Draws the user interface elements of all visible screens from top to bottom.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public void DrawUserInterface(SpriteBatch spriteBatch)
		{
			spriteBatch.WorldMatrix = Matrix.Identity;

			for (var i = GetFirstVisibleScreen(); i < _screens.Count; ++i)
			{
				// Each screen can use up to 100 different layers
				spriteBatch.Layer = i * 100;
				_screens[i].DrawUserInterface(spriteBatch);
			}
		}

		/// <summary>
		///   Gets the index of the first visible app screen that must be drawn.
		/// </summary>
		private int GetFirstVisibleScreen()
		{
			var firstScreen = _screens.Count - 1;
			while (firstScreen > 0 && !_screens[firstScreen].IsOpaque)
				--firstScreen;
			return firstScreen;
		}
	}
}