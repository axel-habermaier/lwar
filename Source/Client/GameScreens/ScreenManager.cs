using System;

namespace Lwar.Client.GameScreens
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Manages several layered game screens.
	/// </summary>
	public sealed class ScreenManager : DisposableObject
	{
		/// <summary>
		///   The stack of layered game screens.
		/// </summary>
		private readonly DeferredList<GameScreen> _screens = new DeferredList<GameScreen>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the game session.</param>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		/// <param name="inputDevice">The logical input device that provides all the user input to the game session.</param>
		public ScreenManager(Window window, GraphicsDevice graphicsDevice, AssetsManager assets, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;
			Assets = assets;
			InputDevice = inputDevice;

			SpriteBatch = new SpriteBatch(graphicsDevice);
		}

		/// <summary>
		///   Gets or sets the window that displays the game session.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game session.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to draw the game session.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the assets manager that manages all assets of the game session.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///   Gets the sprite batch that all game screens should use to draw 2D content on the screen.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SpriteBatch.SafeDispose();
		}

		/// <summary>
		///   Removes all game screens from the screen manager.
		/// </summary>
		public void Clear()
		{
			_screens.SafeDisposeAll();
			_screens.Clear();
		}

		/// <summary>
		///   Adds the game screen on the top of the stack.
		/// </summary>
		/// <param name="gameScreen">The game screen that should be added.</param>
		public void Add(GameScreen gameScreen)
		{
			Assert.ArgumentNotNull(gameScreen, () => gameScreen);

			_screens.Add(gameScreen);

			gameScreen.ScreenManager = this;
			gameScreen.Initialize();
		}

		/// <summary>
		///   Removes the game screen from the stack.
		/// </summary>
		/// <param name="gameScreen">The game screen that should be removed.</param>
		public void Remove(GameScreen gameScreen)
		{
			Assert.ArgumentNotNull(gameScreen, () => gameScreen);
			_screens.Remove(gameScreen);
		}

		/// <summary>
		///   Updates all screens from top to bottom.
		/// </summary>
		public void Update()
		{
			for (var i = _screens.Count - 1; i >= 0; --i)
				_screens[i].Update(i == _screens.Count - 1);
		}

		/// <summary>
		///   Draws all visible screens from top to bottom.
		/// </summary>
		public void Draw()
		{
			for (var i = _screens.Count - 1; i >= 0; --i)
			{
				_screens[i].Draw();

				if (_screens[i].IsOpaque)
					break;
			}
		}
	}
}