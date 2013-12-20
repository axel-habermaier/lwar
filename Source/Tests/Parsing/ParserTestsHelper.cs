namespace Tests.Parsing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using Pegasus.Scripting.Parsing;

	public abstract class ParserTestsHelper<TResult>
	{
		private readonly Parser<TResult> _parser;

		protected ParserTestsHelper(Parser<TResult> parser)
		{
			_parser = parser;
		}

		protected void Success(string input, TResult expectedResult, bool parseAll = true)
		{
			var stream = new InputStream(input);
			var reply = _parser.Parse(stream);

			if (reply.Status != ReplyStatus.Success)
			{
				reply.GenerateErrorMessage(stream);
				Console.WriteLine(reply.Errors.ErrorMessage);
			}

			reply.Status.Should().Be(ReplyStatus.Success);
			reply.Result.Should().Be(expectedResult);

			if (parseAll)
				stream.EndOfInput.Should().BeTrue("parser should consume all input");
			else
				stream.EndOfInput.Should().BeFalse("parser should not consume all input");
		}

		protected TResult Success(string input, bool parseAll = true)
		{
			var stream = new InputStream(input);
			var reply = _parser.Parse(stream);

			reply.Status.Should().Be(ReplyStatus.Success);

			if (parseAll)
				stream.EndOfInput.Should().BeTrue("parser should consume all input");
			else
				stream.EndOfInput.Should().BeFalse("parser should not consume all input");

			return reply.Result;
		}

		private void Error(string input, IEnumerable<ErrorMessage> errorMessages)
		{
			var stream = new InputStream(input);
			var reply = _parser.Parse(stream);

			if (reply.Status == ReplyStatus.Success)
				Console.WriteLine("Result: {0}", reply.Result);

			reply.Status.Should().Be(ReplyStatus.Error);
			reply.Errors.Should().Equal(errorMessages);
		}

		private void Error(string input, ErrorType type, params string[] expected)
		{
			Error(input, expected.Select(e => new ErrorMessage(type, e)).ToList());
		}

		protected void Expected(string input, params string[] expectedErrors)
		{
			Error(input, ErrorType.Expected, expectedErrors);
		}

		protected void UnexpectedEndOfInput(string input)
		{
			Error(input, new List<ErrorMessage> { ErrorMessage.UnexpectedEndOfInput });
		}

		protected void Message(string input, params string[] expectedMessages)
		{
			Error(input, ErrorType.Message, expectedMessages);
		}
	}
}