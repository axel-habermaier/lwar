namespace Lwar.States
{
	using System;
	using Pegasus;

	/// <summary>
	///     Manages the current state of an application or a state and handles state transitions.
	/// </summary>
	public class StateManager
	{
		/// <summary>
		///     The current state of the state manager.
		/// </summary>
		private State _currentState;

		/// <summary>
		///     Transitions to the given state, leaving the current one.
		/// </summary>
		/// <param name="state">The state that should be transitioned to.</param>
		public void TransitionTo(State state)
		{
			Assert.ArgumentNotNull(state);
			Assert.ArgumentSatisfies(state != _currentState, "State is already current.");

			if (_currentState != null)
				_currentState.OnLeft();

			_currentState = state;
			_currentState.OnEntered();
		}

		/// <summary>
		///     Updates the current state.
		/// </summary>
		public void Update()
		{
			_currentState.Update();
		}
	}
}