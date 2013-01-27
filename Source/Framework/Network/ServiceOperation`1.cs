using System;

namespace Pegasus.Framework.Network
{
	using Processes;

	/// <summary>
	///   Represents an awaitable service operation that returns a result.
	/// </summary>
	/// <typeparam name="TResult">The type of the operation's result.</typeparam>
	internal class ServiceOperation<TResult> : PooledObject<ServiceOperation<TResult>>, IServiceOperation,
											   IAsyncOperation<TResult>
	{
		/// <summary>
		///   The number of frames to wait before a timeout exception is thrown.
		/// </summary>
		private int _frameCount;

		/// <summary>
		///   Deserializes the operation's result value from an incoming packet.
		/// </summary>
		private Func<IncomingPacket, TResult> _resultDeserializer;

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
			if (_frameCount == 0)
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
			Result = _resultDeserializer(packet);
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
		public static ServiceOperation<TResult> Create(Func<IncomingPacket, TResult> resultDeserializer)
		{
			var invocation = GetInstance();
			invocation._resultDeserializer = resultDeserializer;
			invocation.IsCompleted = false;
			invocation._frameCount = 10000 * App.UpdatesPerSecond / 1000;
			invocation.Exception = null;
			invocation.Result = default(TResult);
			return invocation;
		}
	}
}