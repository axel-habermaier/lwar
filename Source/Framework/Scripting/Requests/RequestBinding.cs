using System;

namespace Pegasus.Framework.Scripting.Requests
{
	using Platform.Input;

	/// <summary>
	///   Binds a user request to a logical input. Whenever the input is triggered, the user request is executed.
	/// </summary>
	internal struct RequestBinding
	{
		/// <summary>
		///   The input that triggers the execution of the request.
		/// </summary>
		private readonly LogicalInput _input;

		/// <summary>
		///   The user request that is executed when the input is triggered.
		/// </summary>
		private readonly IRequest _request;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="input">The input that should trigger the execution of the request.</param>
		/// <param name="request">The user request that should be executed when the input is triggered.</param>
		public RequestBinding(LogicalInput input, IRequest request)
		{
			Assert.ArgumentNotNull(input, () => input);
			Assert.ArgumentNotNull(request, () => request);

			_input = input;
			_request = request;
		}

		/// <summary>
		///   Executes the user request if the input has been triggered.
		/// </summary>
		public void ExecuteIfTriggered()
		{
			if (_input.IsTriggered)
				_request.Execute();
		}
	}
}