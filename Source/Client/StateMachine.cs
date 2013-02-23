using System;

namespace Lwar.Client
{
	using System.Threading.Tasks;
	using Pegasus.Framework;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Represents a state machine whose states are asynchronous processes that are executed for as long as the state remains
	///   current.
	/// </summary>
	public class StateMachine : PooledObject<StateMachine>
	{
		/// <summary>
		///   The process that handles delayed state changes.
		/// </summary>
		private IProcess _delayedStateChangeProcess;

		/// <summary>
		///   The state function that should be set next.
		/// </summary>
		private AsyncAction _nextState;

		/// <summary>
		///   The process scheduler that schedules the asynchronous state processes.
		/// </summary>
		private ProcessScheduler _scheduler;

		/// <summary>
		///   The current state process.
		/// </summary>
		private IProcess _stateProcess;

		/// <summary>
		///   Creates a new state machine instance.
		/// </summary>
		/// <param name="processManager">The process scheduler that should be used to schedule the asynchronous state processes.</param>
		public static StateMachine Create(ProcessScheduler processManager)
		{
			var stateMachine = GetInstance();
			stateMachine._scheduler = processManager;
			stateMachine._delayedStateChangeProcess = processManager.CreateProcess(stateMachine.HandleDelayedStateChanges);
			return stateMachine;
		}

		/// <summary>
		///   Immediately changes the state to the given state function.
		/// </summary>
		/// <param name="asyncAction">The state function representing the new state of the state machine.</param>
		public void ChangeState(AsyncAction asyncAction)
		{
			_stateProcess.SafeDispose();
			_stateProcess = null;	// Important, otherwise the same process might be disposed twice
									// if the creation of the next state process throws an exception

			_stateProcess = _scheduler.CreateProcess(asyncAction);
		}

		/// <summary>
		///   Changes the state to the given state function within the duration of one frame.
		/// </summary>
		/// <param name="asyncAction">The state function representing the new state of the state machine.</param>
		public void ChangeStateDelayed(AsyncAction asyncAction)
		{
			_nextState = asyncAction;
		}

		/// <summary>
		///   Updates the current state if a delayed state change has been requested.
		/// </summary>
		/// <param name="context">The context in which the delayed state changes should be handled.</param>
		private async Task HandleDelayedStateChanges(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				if (_nextState != null)
				{
					// We have to reset _nextState before calling ChangeState, otherwise we'd overwrite the next state
					// change if the next state changes again immediately
					var nextState = _nextState;
					_nextState = null;

					ChangeState(nextState);
				}

				await context.NextFrame();
			}
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_delayedStateChangeProcess.SafeDispose();
			_stateProcess.SafeDispose();
		}
	}
}