﻿namespace Pegasus.Scripting.Parsing
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes the reply of a parser.
	/// </summary>
	/// <typeparam name="TResult">The type of the parser result.</typeparam>
	public struct Reply<TResult>
	{
		/// <summary>
		///     The errors that were generated by the parser.
		/// </summary>
		private ErrorMessageList _errors;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="result">The result generated by the parser if it was successful.</param>
		internal Reply(TResult result)
			: this()
		{
			Status = ReplyStatus.Success;
			Result = result;
			_errors = ErrorMessageList.Empty;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="status">A value indicating whether the parser that generated the reply failed fatally.</param>
		/// <param name="error">The error that was generated by the parser.</param>
		internal Reply(ReplyStatus status, ErrorMessage error)
			: this(status, new ErrorMessageList(error))
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="status">A value indicating whether the parser that generated the reply failed fatally.</param>
		/// <param name="errors">The errors that were generated by the parser.</param>
		internal Reply(ReplyStatus status, ErrorMessageList errors)
			: this()
		{
			Assert.ArgumentInRange(status);

			Status = status;
			Result = default(TResult);
			_errors = errors;
		}

		/// <summary>
		///     Gets the errors that were generated by the parser.
		/// </summary>
		internal ErrorMessageList Errors
		{
			get { return _errors; }
		}

		/// <summary>
		///     Gets a value indicating whether the parser that generated the reply succeeded.
		/// </summary>
		public ReplyStatus Status { get; private set; }

		/// <summary>
		///     Gets the result generated by the parser if it was successful.
		/// </summary>
		public TResult Result { get; private set; }

		/// <summary>
		///     Generates a formatted error message if the reply indicates a failed parsing attempt.
		/// </summary>
		/// <param name="inputStream">The input stream that generated the parser errors.</param>
		internal void GenerateErrorMessage(InputStream inputStream)
		{
			_errors.GenerateErrorMessage(inputStream);
		}
	}
}