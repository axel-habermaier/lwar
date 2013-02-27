using System;

namespace Lwar.Client.GameStates
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Manages several layered game states.
	/// </summary>
	public sealed class StateManager : DisposableObject
	{
		/// <summary>
		///   The stack of layered game states.
		/// </summary>
		private readonly DeferredList<GameState> _states = new DeferredList<GameState>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the game.</param>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game.</param>
		/// <param name="assets">The assets manager that manages all assets of the game.</param>
		/// <param name="inputDevice">The logical input device that provides all the user input to the game.</param>
		public StateManager(Window window, GraphicsDevice graphicsDevice, AssetsManager assets, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;
			Assets = assets;
			InputDevice = inputDevice;

			SpriteBatch = new SpriteBatch(graphicsDevice, assets);
			ResizeSpriteBatch(Window.Size);
			Window.Resized += ResizeSpriteBatch;
		}

		/// <summary>
		///   Gets or sets the window that displays the game.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to draw the game.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the assets manager that manages all assets of the game.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///   Gets the sprite batch that all game states should use to draw 2D content on the screen.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///   Updates the projection matrix of the sprite batch.
		/// </summary>
		/// <param name="size">The new size of the window.</param>
		private void ResizeSpriteBatch(Size size)
		{
			SpriteBatch.ProjectionMatrix = Matrix.CreateOrthographic(0, size.Width, size.Height, 0, 0, 1);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_states.SafeDisposeAll();
			SpriteBatch.SafeDispose();
			Window.Resized -= ResizeSpriteBatch;
		}

		/// <summary>
		///   Removes all game states from the state manager.
		/// </summary>
		public void Clear()
		{
			_states.Clear();
		}

		/// <summary>
		///   Adds the game state onto the top of the stack.
		/// </summary>
		/// <param name="gameState">The game state that should be added.</param>
		public void Add(GameState gameState)
		{
			Assert.ArgumentNotNull(gameState, () => gameState);

			_states.Add(gameState);

			gameState.StateManager = this;
			gameState.Initialize();
		}

		/// <summary>
		///   Removes the game state from the stack.
		/// </summary>
		/// <param name="gameState">The game state that should be removed.</param>
		public void Remove(GameState gameState)
		{
			Assert.ArgumentNotNull(gameState, () => gameState);
			_states.Remove(gameState);
		}

		/// <summary>
		///   Updates all states from top to bottom.
		/// </summary>
		public void Update()
		{
			_states.Update();

			for (var i = 0; i < _states.Count; ++i)
				_states[i].Update(i == _states.Count - 1);
		}

		/// <summary>
		///   Draws all visible states from top to bottom.
		/// </summary>
		public void Draw()
		{
			// Find the first state that we need to draw
			var firstScreen = _states.Count - 1;
			while (firstScreen > 0 && !_states[firstScreen].IsOpaque)
				--firstScreen;

			// Draw from bottom to top
			for (var i = firstScreen; i < _states.Count; ++i)
			{
				_states[i].Draw();
				SpriteBatch.DrawBatch();
			}
		}
	}
}