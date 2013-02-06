using System;

namespace Pegasus.Framework.Network
{
	using Platform;
	using Processes;

	/// <summary>
	///   Represents an awaitable service operation that returns a result.
	/// </summary>
	/// <typeparam name="TResult">The type of the operation's result.</typeparam>
	internal class ServiceOperation<TResult> : PooledObject<ServiceOperation<TResult>>, IServiceOperation,
											   IAsyncOperation<TResult>
	{
		/// <summary>
		///   The clock that provides time measurements.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///   Deserializes the operation's result value from an incoming packet.
		/// </summary>
		private Func<BufferReader, TResult> _resultDeserializer;

		/// <summary>
		///   The amount of time in seconds to wait for the completion of the service operation before a timeout exception is
		///   thrown.
		/// </summary>
		private double _timeout;

		/// <summary>
		///   Gets the result produced by the asynchronous operation.
		/// </summary>
		public TResult Result { get; private set; }

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
			if (_clock.Seconds >= _timeout)
				Exception = new TimeoutException("The asynchronous call of the service operation timed out.");
		}

		/// <summary>
		///   Gets the exception that has been raised during the execution of the operation by the host.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		///   Sets the result of the operation invocation and marks the invocation as completed.
		/// </summary>
		/// <param name="packet">The packet that contains the result returned by the server.</param>
		public void SetResult(IncomingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Result = _resultDeserializer(packet.Reader);
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
			Result = default(TResult);
			IsCompleted = false;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="resultDeserializer">Deserializes the operation's result value from an incoming packet.</param>
		/// <param name="timeout">
		///   The amount of time in seconds to wait for the completion of the service operation before a
		///   timeout exception should be thrown.
		/// </param>
		public static ServiceOperation<TResult> Create(Func<BufferReader, TResult> resultDeserializer, double timeout)
		{
			var invocation = GetInstance();
			invocation._resultDeserializer = resultDeserializer;
			invocation.IsCompleted = false;
			invocation._clock = Clock.Create();
			invocation._timeout = timeout;
			invocation.Exception = null;
			invocation.Result = default(TResult);
			return invocation;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_clock.SafeDispose();
		}
	}
}