using System;

namespace Pegasus.Framework.Processes
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;

	/// <summary>
	///   Represents a scheduler for asynchronous processes that always schedules the processes non-concurrently.
	/// </summary>
	public sealed class ProcessScheduler : DisposableObject
	{
		/// <summary>
		///   The processes that have been added since or during the last call to RunProcesses.
		/// </summary>
		private readonly List<IResumableProcess> _added = new List<IResumableProcess>();

		/// <summary>
		///   The processes currently being scheduled by the scheduler.
		/// </summary>
		private readonly List<IResumableProcess> _processes = new List<IResumableProcess>();

		/// <summary>
		///   Creates a new process that the scheduler is responsible for.
		/// </summary>
		/// <param name="asyncAction">The asynchronous action that the process should execute.</param>
		public IProcess CreateProcess(AsyncAction asyncAction, [CallerFilePath] string path = "")
		{
			Assert.ArgumentNotNull(asyncAction, () => asyncAction);

			var process = new Process();
			try
			{
				process.SetDescription(path);
				process.Run(asyncAction);
				_added.Add(process);
				return process;
			}
			catch (Exception)
			{
				process.SafeDispose();
				throw;
			}
		}

		/// <summary>
		///   Creates a new process that the scheduler is responsible for.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the process.</typeparam>
		/// <param name="asyncFunc">The asynchronous function that the process should execute.</param>
		public IProcess<TResult> CreateProcess<TResult>(AsyncFunc<TResult> asyncFunc)
		{
			Assert.ArgumentNotNull(asyncFunc, () => asyncFunc);

			var process = new Process<TResult>();
			try
			{
				process.Run(asyncFunc);
				_added.Add(process);
				return process;
			}
			catch (Exception)
			{
				process.SafeDispose();
				throw;
			}
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

				try
				{
					process.Resume();
				}
				finally
				{
					if (process.IsCanceled || process.IsCompleted || process.IsFaulted)
					{
						var last = _processes.Count - 1;
						_processes[i] = _processes[last];
						_processes.RemoveAt(last);
						--i;
					}
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Assert.That(_processes.All(p => p.IsCanceled || p.IsCompleted || p.IsFaulted),
						"There are still running processes managed by the scheduler.");
		}
	}
}