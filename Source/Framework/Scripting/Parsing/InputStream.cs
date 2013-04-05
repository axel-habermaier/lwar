using System;

namespace Pegasus.Framework.Scripting.Parsing
{
	/// <summary>
	///   Provides read-access to a sequence of UTF-16 characters. The only supported newline token is \n.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class InputStream<TUserState>
	{
		/// <summary>
		///   Gets the input string that should be parsed.
		/// </summary>
		private readonly string _input;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="input">The string that should be parsed.</param>
		/// <param name="name">The name of the input stream.</param>
		/// <param name="line">If set to a number greater than 0, sets the number of the line that line counting starts at.</param>
		/// <param name="userState">The initial user state.</param>
		public InputStream(string input, string name = null, int line = -1, TUserState userState = default(TUserState))
		{
			Assert.ArgumentNotNull(input, () => input);

			_input = input;
			Name = name ?? String.Empty;

			var lineNumber = line > 0 ? line : 1;
			State = new InputStreamState<TUserState>(0, lineNumber, 0, userState);
		}

		/// <summary>
		///   Gets the name of the input stream.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the current state of the input stream.
		/// </summary>
		internal InputStreamState<TUserState> State { get; set; }

		/// <summary>
		///   Gets a value indicating whether the end of the input has been reached.
		/// </summary>
		internal bool EndOfInput
		{
			get { return State.Position >= _input.Length; }
		}

		/// <summary>
		///   Returns the current line.
		/// </summary>
		internal string CurrentLine
		{
			get
			{
				var lineEnd = _input.IndexOf('\n', State.LineBegin);
				lineEnd = lineEnd == -1 ? _input.Length : lineEnd;

				return _input.Substring(State.LineBegin, lineEnd - State.LineBegin);
			}
		}

		/// <summary>
		///   Returns the current character without changing the stream state. If the stream is at the end of the input,
		///   Char.MaxValue is returned to indicate the failure.
		/// </summary>
		/// <returns></returns>
		internal char Peek()
		{
			if (EndOfInput)
				return Char.MaxValue;

			return _input[State.Position];
		}

		/// <summary>
		///   Returns a substring of the input with the requested length, starting at the given offset.
		/// </summary>
		/// <param name="position">The first character of the substring.</param>
		/// <param name="length">The length of the substring.</param>
		internal string Substring(int position, int length)
		{
			Assert.ArgumentInRange(position, () => position, 0, Int32.MaxValue);
			Assert.ArgumentInRange(length, () => length, 0, Int32.MaxValue);
			Assert.That(position + length <= _input.Length, "Buffer overflow");

			return _input.Substring(position, length);
		}

		/// <summary>
		///   Skips the given number of characters.
		/// </summary>
		/// <param name="count">The number of characters that should be skipped.</param>
		internal void Skip(int count)
		{
			Assert.ArgumentInRange(count, () => count, 0, Int32.MaxValue);

			for (var i = 0; i < count && !EndOfInput; ++i)
				State = State.Advance(IsNewline(Peek()));
		}

		/// <summary>
		///   Skips zero or more white spaces until the first non-white space character or the end of the input is reached.
		/// </summary>
		internal void SkipWhiteSpaces()
		{
			while (!EndOfInput && Char.IsWhiteSpace(Peek()))
				Skip(1);
		}

		/// <summary>
		///   Skips all characters that satisfy the given predicate and returns the number of skipped characters.
		/// </summary>
		/// <param name="predicate">The predicate that is used to decide how many characters to skip.</param>
		internal int Skip(Predicate<char> predicate)
		{
			Assert.ArgumentNotNull(predicate, () => predicate);

			var count = 0;
			while (!EndOfInput && predicate(Peek()))
			{
				Skip(1);
				++count;
			}
			return count;
		}

		/// <summary>
		///   Skips the given string and returns false if either the end of the input has been reached or there were any other
		///   characters in the stream.
		/// </summary>
		/// <param name="s">The string that should be skipped.</param>
		internal bool Skip(string s)
		{
			Assert.ArgumentNotNull(s, () => s);

			foreach (var character in s)
			{
				if (EndOfInput || Peek() != character)
					return false;

				Skip(1);
			}

			return true;
		}

		/// <summary>
		///   Returns true if the given character represents a newline token.
		/// </summary>
		/// <param name="character">The character that should be checked.</param>
		private static bool IsNewline(char character)
		{
			return character == '\n';
		}
	}
}