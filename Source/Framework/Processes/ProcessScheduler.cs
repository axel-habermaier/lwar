using System;

namespace Pegasus.Framework.Processes
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///   Represents a scheduler for asynchronous processes that always schedules the processes non-concurrently.
	/// </summary>
	public sealed class ProcessScheduler : DisposableObject
	{
		/// <summary>
		///   The processes that have been added since or during the last call to RunProcesses.
		/// </summary>
		private readonly List<Process> _added = new List<Process>();

		/// <summary>
		///   The processes currently being scheduled by the scheduler.
		/// </summary>
		private readonly List<Process> _processes = new List<Process>();

		/// <summary>
		///   Creates a new process that the scheduler is responsible for.
		/// </summary>
		/// <param name="asyncAction">The asynchronous action that the process should execute.</param>
		public IProcess CreateProcess(AsyncAction asyncAction)
		{
			Assert.ArgumentNotNull(asyncAction, () => asyncAction);

			var process = Process.Create(this, asyncAction);
			_added.Add(process);
			return process;
		}

		/// <summary>
		///   Creates a new process that the scheduler is responsible for.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the process.</typeparam>
		/// <param name="asyncFunc">The asynchronous function that the process should execute.</param>
		public IProcess<TResult> CreateProcess<TResult>(AsyncFunc<TResult> asyncFunc)
		{
			Assert.ArgumentNotNull(asyncFunc, () => asyncFunc);

			var process = Process<TResult>.Create(this, asyncFunc);
			_added.Add(process);
			return process;
		}

		/// <summary>
		///   Resumes all asynchronous processes that are waiting to be resumed. This method must be called periodically.
		/// </summary>
		public void RunProcesses()
		{
			_processes.AddRange(_added);
			_added.Clear();

			for (var i = 0; i < _processes.Count; ++i)
			{
				var process = _processes[i];
				process.Resume();

				if (!process.IsCanceled && !process.IsCompleted && !process.IsFaulted)
					continue;

				var last = _processes.Count - 1;
				_processes[i] = _processes[last];
				_processes.RemoveAt(last);
				--i;
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Assert.That(_added.All(p => p.IsCanceled), "There are still uncanceled processes managed by the scheduler.");
			Assert.That(_processes.All(p => p.IsCanceled), "There are still uncanceled processes managed by the scheduler.");
		}
	}
}