using System;

namespace Pegasus.Framework.Network
{
	using System.Net.Sockets;
	using Platform.Memory;
	using Processes;

	/// <summary>
	///   Represents a process that waits for an asychronous socket operation to complete.
	/// </summary>
	/// <typeparam name="TResult">The result of the socket operation.</typeparam>
	internal class SocketOperation<TResult> : PooledObject<SocketOperation<TResult>>, IAsyncOperation<TResult>
	{
		/// <summary>
		///   The object used for thread synchronization.
		/// </summary>
		private readonly object _lockObj = new object();

		/// <summary>
		///   The event args that report the completion of the socket operation.
		/// </summary>
		private SocketAsyncEventArgs _eventArgs;

		/// <summary>
		///   A value indicating whether the socket operation has completed.
		/// </summary>
		private bool _isCompleted;

		/// <summary>
		///   Gets the exception that has been thrown during the execution of the operation.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the process has completed.
		/// </summary>
		public bool IsCompleted { get; private set; }

		/// <summary>
		///   Updates the state of the asynchronous operation, setting its Exception and IsCompleted properties if the
		///   process has terminated.
		/// </summary>
		public void UpdateState()
		{
			// IsCompleted cannot return _isCompleted, as that sometimes leads to an inconsistent state where IsCompleted is
			// false when the process registers the socket operation but already true when IsCompleted on the awaiter is called
			// by C#'s async/await state machine
			lock (_lockObj)
				IsCompleted = _isCompleted;
		}

		/// <summary>
		///   Gets the result produced by the asynchronous operation.
		/// </summary>
		public TResult Result { get; private set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="eventArgs">The event args that report the completion of the socket operation.</param>
		public static SocketOperation<TResult> Create(SocketAsyncEventArgs eventArgs)
		{
			Assert.ArgumentNotNull(eventArgs, () => eventArgs);

			var process = GetInstance();
			process.SetDescription("Waiting for socket operation");
			process._isCompleted = false;
			process.IsCompleted = false;
			process._eventArgs = eventArgs;
			process.Exception = null;
			eventArgs.Completed += process.OnCompleted;

			return process;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			// If the process has been canceled before the operation completes, make sure we're not getting informed
			// about the completion at some later point
			_eventArgs.Completed -= OnCompleted;
		}

		/// <summary>
		///   Invoked when the socket operation is completed.
		/// </summary>
		private void OnCompleted(object sender, SocketAsyncEventArgs e)
		{
			OnCompleted(e);
		}

		/// <summary>
		///   Invoked when the socket operation is completed.
		/// </summary>
		private void OnCompleted(SocketAsyncEventArgs eventArgs)
		{
			Assert.ArgumentNotNull(eventArgs, () => eventArgs);
			lock (_lockObj)
			{
				if (eventArgs.SocketError == SocketError.Success)
					Result = ((Func<SocketAsyncEventArgs, TResult>)eventArgs.UserToken)(eventArgs);
				else
					Exception = new SocketException((int)eventArgs.SocketError);

				_isCompleted = true;
			}
		}

		/// <summary>
		///   Invoked when the socket operation is completed.
		/// </summary>
		internal void OnSynchronouslyCompleted(SocketAsyncEventArgs eventArgs)
		{
			Assert.ArgumentNotNull(eventArgs, () => eventArgs);
			if (eventArgs.SocketError == SocketError.Success)
				Result = ((Func<SocketAsyncEventArgs, TResult>)eventArgs.UserToken)(eventArgs);
			else
				Exception = new SocketException((int)eventArgs.SocketError);

			IsCompleted = true;
		}
	}
}