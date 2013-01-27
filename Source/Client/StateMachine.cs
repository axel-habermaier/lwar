using System;

namespace Client
{
	using Pegasus.Framework;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Represents a state machine whose states are asynchronous processes that are executed for as long as the state remains
	///   current.
	/// </summary>
	public class StateMachine : PooledObject<StateMachine>
	{
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
			return stateMachine;
		}

		/// <summary>
		///   Changes the state to the given state function.
		/// </summary>
		/// <param name="asyncAction">The state function representing the new state of the state machine.</param>
		public void ChangeState(AsyncAction asyncAction)
		{
			_stateProcess.SafeDispose();
			_stateProcess = _scheduler.CreateProcess(asyncAction);
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_stateProcess.SafeDispose();
		}
	}
}