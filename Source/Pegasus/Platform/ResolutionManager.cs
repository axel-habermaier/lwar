namespace Pegasus.Platform
{
	using System;
	using Framework.UserInterface.Controls;
	using Graphics;
	using Logging;
	using Memory;
	using Scripting;

	/// <summary>
	///     Manages resolution and fullscreen mode changes.
	/// </summary>
	internal class ResolutionManager : DisposableObject
	{
		/// <summary>
		///     The swap chain that is affected by resolution changes.
		/// </summary>
		private readonly SwapChain _swapChain;

		/// <summary>
		///     The window that is affected by resolution changes.
		/// </summary>
		private readonly NativeWindow _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that is affected by resolution changes.</param>
		/// <param name="swapChain">The swap chain that should be affected by resolution changes.</param>
		public ResolutionManager(NativeWindow window, SwapChain swapChain)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(swapChain);

			_window = window;
			_swapChain = swapChain;

			Commands.OnRestartGraphics += UpdateGraphicsState;
		}

		/// <summary>
		///     Updates the resolution manager, ensuring that the application state remains consistent.
		/// </summary>
		public void Update()
		{
			if (Cvars.Fullscreen != _swapChain.IsFullscreen)
				Cvars.FullscreenCvar.SetImmediate(_swapChain.IsFullscreen);

			// We do not care about the window placement in full screen mode
			if (Cvars.Fullscreen)
				return;

			if (Cvars.WindowMode != _window.Mode)
				Cvars.WindowMode = _window.Mode;

			// We do not care about the window size in minimized or maximized mode
			if (_window.Mode != WindowMode.Normal)
				return;

			if (Cvars.WindowPosition != _window.Position)
				Cvars.WindowPosition = _window.Position;

			if (Cvars.WindowSize != _window.Size)
				Cvars.WindowSize = _window.Size;
		}

		/// <summary>
		///     Updates the state of the graphics subsystem, handling transitions in and out of fullscreen mode.
		/// </summary>
		private void UpdateGraphicsState()
		{
			var switchFullscreen = Cvars.FullscreenCvar.HasDeferredValue && Cvars.FullscreenCvar.DeferredValue;
			var switchWindowed = Cvars.FullscreenCvar.HasDeferredValue && !Cvars.FullscreenCvar.DeferredValue;
			var changeResolution = Cvars.ResolutionCvar.HasDeferredValue;

			// Execute all deferred cvar updates
			CvarRegistry.ExecuteDeferredUpdates(UpdateMode.OnGraphicsRestart);

			// Resize and update the window and the swap chain depending on whether we're in fullscreen or windowed mode
			if (switchFullscreen || changeResolution)
			{
				Log.Info("Switching to fullscreen mode, resolution {0}x{1}.", Cvars.Resolution.Width, Cvars.Resolution.Height);

				if (_swapChain.SwitchToFullscreen(Cvars.Resolution))
					return;

				// There was an error switching to fullscreen mode, so switch back to windowed mode
				Cvars.Fullscreen = false;
				UpdateGraphicsState();
			}

			if (switchWindowed)
			{
				Log.Info("Switching to windowed mode, resolution {0}x{1}.", Cvars.WindowSize.Width, Cvars.WindowSize.Height);
				_swapChain.SwitchToWindowed();
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnRestartGraphics -= UpdateGraphicsState;
		}
	}
}