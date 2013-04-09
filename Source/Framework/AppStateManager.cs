using System;

namespace Pegasus.Framework
{
	using System.Collections.Generic;
	using Rendering;

	/// <summary>
	///   Manages several layered app states.
	/// </summary>
	public sealed class AppStateManager : DisposableObject
	{
		/// <summary>
		///   The stack of layered game states.
		/// </summary>
		private readonly List<AppState> _states = new List<AppState>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="context">
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </param>
		public AppStateManager(IAppContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			Context = context;
		}

		/// <summary>
		///   Gets the context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		internal IAppContext Context { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Clear();
		}

		/// <summary>
		///   Removes all app states from the state manager.
		/// </summary>
		public void Clear()
		{
			_states.SafeDisposeAll();
			_states.Clear();
		}

		/// <summary>
		///   Adds the app state onto the top of the stack.
		/// </summary>
		/// <param name="appState">The app state that should be added.</param>
		public void Add(AppState appState)
		{
			Assert.ArgumentNotNull(appState, () => appState);

			_states.Add(appState);

			appState.StateManager = this;
			appState.Initialize();
		}

		/// <summary>
		///   Removes the app state from the stack.
		/// </summary>
		/// <param name="appState">The app state that should be removed.</param>
		public void Remove(AppState appState)
		{
			Assert.ArgumentNotNull(appState, () => appState);

			if (_states.Remove(appState))
				appState.SafeDispose();
		}

		/// <summary>
		///   Updates all states from top to bottom.
		/// </summary>
		public void Update()
		{
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
		///   Gets the index of the first visible app state that must be drawn.
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