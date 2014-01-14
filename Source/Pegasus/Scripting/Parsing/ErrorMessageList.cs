﻿namespace Pegasus.Scripting.Parsing
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	///   Represents a list of error messages returned by a parser.
	/// </summary>
	public struct ErrorMessageList : IEnumerable<ErrorMessage>
	{
		/// <summary>
		///   Represents the empty error message list.
		/// </summary>
		internal static readonly ErrorMessageList Empty = new ErrorMessageList(new ErrorMessage[0]);

		/// <summary>
		///   A cached string builder instance.
		/// </summary>
		private static readonly StringBuilder Builder = new StringBuilder(1024);

		/// <summary>
		///   Gets the error messages that were generated by the parser.
		/// </summary>
		private readonly ErrorMessage[] _errorMessages;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorsMessage">The errors that should be added to the error list.</param>
		internal ErrorMessageList(ErrorMessage[] errorsMessage)
			: this()
		{
			_errorMessages = errorsMessage;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorMessage">The error message that should be added to the error message list.</param>
		internal ErrorMessageList(ErrorMessage errorMessage)
			: this()
		{
			_errorMessages = new[] { errorMessage };
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="first">The first error message that should be added to the error message list.</param>
		/// <param name="second">The second error message that should be added to the error message list.</param>
		internal ErrorMessageList(ErrorMessage first, ErrorMessage second)
			: this()
		{
			_errorMessages = new[] { first, second };
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="errorList">The error message list whose error messages should be added to the new error message list.</param>
		/// <param name="errorMessages">The errors that should be added to the new error message list.</param>
		internal ErrorMessageList(ErrorMessageList errorList, params ErrorMessage[] errorMessages)
			: this()
		{
			Assert.ArgumentSatisfies(errorList._errorMessages != null, "Uninitialized error list.");
			Assert.ArgumentNotNull(errorMessages);

			_errorMessages = new ErrorMessage[errorList._errorMessages.Length + errorMessages.Length];
			Array.Copy(errorList._errorMessages, _errorMessages, errorList._errorMessages.Length);
			Array.Copy(errorMessages, 0, _errorMessages, errorList._errorMessages.Length, errorMessages.Length);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="first">The first error message list whose error messages should be added to the new error message list.</param>
		/// <param name="second">The second error message list whose error messages should be added to the new error message list.</param>
		internal ErrorMessageList(ErrorMessageList first, ErrorMessageList second)
			: this()
		{
			Assert.ArgumentSatisfies(first._errorMessages != null, "Uninitialized error list.");
			Assert.ArgumentSatisfies(second._errorMessages != null, "Uninitialized error list.");

			_errorMessages = new ErrorMessage[first._errorMessages.Length + second._errorMessages.Length];
			Array.Copy(first._errorMessages, _errorMessages, first._errorMessages.Length);
			Array.Copy(second._errorMessages, 0, _errorMessages, first._errorMessages.Length, second._errorMessages.Length);
		}

		/// <summary>
		///   Gets the formatted error message.
		/// </summary>
		public string ErrorMessage { get; private set; }

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator<ErrorMessage> IEnumerable<ErrorMessage>.GetEnumerator()
		{
			return ((IEnumerable<ErrorMessage>)_errorMessages).GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<ErrorMessage>)this).GetEnumerator();
		}

		/// <summary>
		///   Generates the formatted error message.
		/// </summary>
		/// <param name="inputStream">The input stream that generated the parser errors.</param>
		internal void GenerateErrorMessage(InputStream inputStream)
		{
			Builder.Clear();
			Builder.AppendFormat("Error at line {0}, column {1}:\n", inputStream.State.Line, inputStream.State.Column);
			Builder.Append(inputStream.CurrentLine);
			Builder.Append("\n");
			for (var i = 0; i < inputStream.State.Column - 1; ++i)
				Builder.Append(" ");
			Builder.Append("\\red^");

			// Show expected messages, if any
			var expected = _errorMessages.Where(e => e.Type == ErrorType.Expected).ToArray();
			if (expected.Length != 0)
			{
				Builder.Append("\nExpected: ");
				Builder.Append(String.Join(" or ", expected.Distinct().Select(e => e.Message)));
			}

			// Show unexpected messages, if any
			var unexpected = _errorMessages.Where(e => e.Type == ErrorType.Unexpected).ToArray();
			if (unexpected.Length != 0)
			{
				Builder.Append("\nUnexpected: ");
				Builder.Append(String.Join(" or ", unexpected.Distinct().Select(e => e.Message)));
			}

			// Show other messages, if any
			var messages = _errorMessages.Where(e => e.Type == ErrorType.Message).ToArray();
			foreach (var message in messages)
				Builder.AppendFormat("\n{0}", message.Message);

			ErrorMessage = Builder.ToString();
		}
	}
}