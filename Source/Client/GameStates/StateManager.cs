using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;
	using Scripting;

	/// <summary>
	///   Manages several layered game states.
	/// </summary>
	public sealed class StateManager : DisposableObject
	{
		/// <summary>
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		private readonly IAppContext _context;

		/// <summary>
		///   The stack of layered game states.
		/// </summary>
		private readonly DeferredList<GameState> _states = new DeferredList<GameState>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="context">
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </param>
		public StateManager(IAppContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			_context = context;
		}

		/// <summary>
		///   Gets the window that displays the game.
		/// </summary>
		public Window Window
		{
			get { return _context.Window; }
		}

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game.
		/// </summary>
		public LogicalInputDevice InputDevice
		{
			get { return _context.LogicalInputDevice; }
		}

		/// <summary>
		///   Gets the graphics device that is used to draw the game.
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get { return _context.GraphicsDevice; }
		}

		/// <summary>
		///   Gets the assets manager that manages all assets of the game.
		/// </summary>
		public AssetsManager Assets
		{
			get { return _context.Assets; }
		}

		/// <summary>
		///   Gets the command registry that handles the application commands.
		/// </summary>
		public CommandRegistry Commands
		{
			get { return (CommandRegistry)_context.Commands; }
		}

		/// <summary>
		///   Gets the cvar registry that handles the application cvars.
		/// </summary>
		public CvarRegistry Cvars
		{
			get { return (CvarRegistry)_context.Cvars; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_states.SafeDispose();
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
		/// <param name="output">The output that the states should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output, () => output);

			for (var i = GetFirstVisibleScreen(); i < _states.Count; ++i)
				_states[i].Draw(output);
		}

		/// <summary>
		///   Draws the user interface elements of all visible states from top to bottom.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public void DrawUserInterface(SpriteBatch spriteBatch)
		{
			for (var i = GetFirstVisibleScreen(); i < _states.Count; ++i)
			{
				_states[i].DrawUserInterface(spriteBatch);
				spriteBatch.DrawBatch();
			}
		}

		/// <summary>
		///   Gets the index of the first visible screen that must be drawn.
		/// </summary>
		private int GetFirstVisibleScreen()
		{
			var firstScreen = _states.Count - 1;
			while (firstScreen > 0 && !_states[firstScreen].IsOpaque)
				--firstScreen;
			return firstScreen;
		}
	}
}