﻿using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Represents an awaitable service operation that does not return a result.
	/// </summary>
	internal class ServiceOperation : PooledObject<ServiceOperation>, IServiceOperation
	{
		/// <summary>
		///   The number of frames to wait before a timeout exception is thrown.
		/// </summary>
		private int _frameCount;

		/// <summary>
		///   Gets the exception that has been raised during the execution of the operation by the host.
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
			if (_frameCount == 0)
				Exception = new TimeoutException("The asynchronous call of the service operation timed out.");
		}

		/// <summary>
		///   Sets the result of the operation invocation and marks the invocation as completed.
		/// </summary>
		/// <param name="packet">The packet that contains the result returned by the server.</param>
		public void SetResult(IncomingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			IsCompleted = true;
		}

		/// <summary>
		///   Sets the exception that was thrown during the execution of the operation and marks the invocation as completed.
		/// </summary>
		/// <param name="exception">The exception that has been thrown.</param>
		public void SetException(Exception exception)
		{
			Assert.ArgumentNotNull(exception, () => exception);

			Exception = exception;
			IsCompleted = true;
		}

		/// <summary>
		///   Invoked when the pooled instance is reused and should reset or reinitialize its state.
		/// </summary>
		protected override void OnReusing()
		{
			Exception = null;
			IsCompleted = false;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public static ServiceOperation Create()
		{
			var invocation = GetInstance();
			invocation.IsCompleted = false;
			invocation._frameCount = 10000 * App.UpdatesPerSecond / 1000;
			invocation.Exception = null;
			return invocation;
		}
	}
}