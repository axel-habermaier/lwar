﻿using System;

namespace Pegasus.Framework.Platform
{
	using Graphics;
	using Logging;
	using Math;
	using Memory;
	using Scripting;

	/// <summary>
	///   Manages resolution and fullscreen mode changes.
	/// </summary>
	internal class ResolutionManager : DisposableObject
	{
		/// <summary>
		///   The swap chain that is affected by resolution changes.
		/// </summary>
		private readonly SwapChain _swapChain;

		/// <summary>
		///   The window that is affected by resolution changes.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that is affected by resolution changes.</param>
		/// <param name="swapChain">The swap chain that should be affected by resolution changes.</param>
		public ResolutionManager(Window window, SwapChain swapChain)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(swapChain);

			_window = window;
			_swapChain = swapChain;

			//Cvars.WindowSizeChanged += UpdateWindowSize;
			//Cvars.WindowPositionChanged += UpdateWindowPosition;
			//Cvars.WindowModeChanged += UpdateWindowState;
			//Commands.OnRestartGraphics += UpdateGraphicsState;
		}

		/// <summary>
		///   Updates the resolution manager, ensuring that the application state remains consistent.
		/// </summary>
		public void Update()
		{
			//if (Cvars.Fullscreen != _swapChain.IsFullscreen)
			//	Cvars.Fullscreen = _swapChain.IsFullscreen;

			//// We do not care about the window placement in full screen mode
			//if (Cvars.Fullscreen)
			//	return;

			//if (Cvars.WindowMode != _window.Mode)
			//	Cvars.WindowMode = _window.Mode;

			//// We do not care about the window size in minimized or maximized mode
			//if (_window.Mode != WindowMode.Normal)
			//	return;

			//// Make sure the windows cvars are always up-to-date
			//if (Cvars.WindowPosition != _window.Position)
			//	Cvars.WindowPosition = _window.Position;

			//if (Cvars.WindowSize != _window.Size)
			//	Cvars.WindowSize = _window.Size;
		}

		/// <summary>
		///   Updates the state of the graphics subsystem, handling transitions in and out of fullscreen mode.
		/// </summary>
		private void UpdateGraphicsState()
		{
			// Execute all deferred cvar updates
			CvarRegistry.ExecuteDeferredUpdates(UpdateMode.OnGraphicsRestart);

			// Resize and update the window and the swap chain depending on whether we're in fullscreen or windowed mode
			if (Cvars.Fullscreen)
				SwitchToFullscreen();
			else
				SwitchToWindowed();
		}

		/// <summary>
		///   Switches to fullscreen mode.
		/// </summary>
		private void SwitchToFullscreen()
		{
			Log.Info("Switching to fullscreen mode, resolution {0}x{1}.", Cvars.Resolution.Width, Cvars.Resolution.Height);

			if (_swapChain.SwitchToFullscreen(Cvars.Resolution))
				return;

			// There was an error switching to fullscreen mode, so switch back to windowed mode
			Cvars.Fullscreen = false;
			UpdateGraphicsState();
		}

		/// <summary>
		///   Switches to windowed mode.
		/// </summary>
		private void SwitchToWindowed()
		{
			Log.Info("Switching to windowed mode, resolution {0}x{1}.", Cvars.WindowSize.Width, Cvars.WindowSize.Height);
			_swapChain.SwitchToWindowed();

			// The cvars might have been changed in the mean-time, but that did not yet have any effect
			_window.Mode = Cvars.WindowMode;
			_window.Size = Cvars.WindowSize;
			_window.Position = Cvars.WindowPosition;
		}

		/// <summary>
		///   Sets the window's size to the value stored in the window size cvar.
		/// </summary>
		/// <param name="size">The old window size.</param>
		private void UpdateWindowSize(Size size)
		{
			if (!Cvars.Fullscreen)
				_window.Size = Cvars.WindowSize;
		}

		/// <summary>
		///   Sets the window's position to the value stored in the window position cvar.
		/// </summary>
		/// <param name="size">The old window position.</param>
		private void UpdateWindowPosition(Vector2i size)
		{
			if (!Cvars.Fullscreen)
				_window.Position = Cvars.WindowPosition;
		}

		/// <summary>
		///   Sets the window's state to the value stored in the window state cvar.
		/// </summary>
		/// <param name="mode">The old window state.</param>
		private void UpdateWindowState(WindowMode mode)
		{
			if (!Cvars.Fullscreen)
				_window.Mode = Cvars.WindowMode;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.WindowSizeChanged -= UpdateWindowSize;
			Cvars.WindowPositionChanged -= UpdateWindowPosition;
			Cvars.WindowModeChanged -= UpdateWindowState;
			Commands.OnRestartGraphics -= UpdateGraphicsState;
		}
	}
}