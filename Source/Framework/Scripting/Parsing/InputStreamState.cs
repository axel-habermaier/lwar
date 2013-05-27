using System;

namespace Pegasus.Framework.Scripting.Parsing
{
	using System.Collections.Generic;

	/// <summary>
	///   Describes the state of an input stream.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public struct InputStreamState<TUserState> : IEquatable<InputStreamState<TUserState>>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The zero-based position of the next character that will be read by the input stream.</param>
		/// <param name="line">The line number of the next character that will be read by the input stream, starting with 1.</param>
		/// <param name="lineBegin">The absolute, zero-based position of the first character of the current line.</param>
		/// <param name="userState">The current user state.</param>
		public InputStreamState(int position, int line, int lineBegin, TUserState userState)
			: this()
		{
			Assert.ArgumentInRange(position, 0, Int32.MaxValue);
			Assert.ArgumentInRange(line, 1, Int32.MaxValue);
			Assert.ArgumentInRange(lineBegin, 0, Int32.MaxValue);

			Position = position;
			Line = line;
			LineBegin = lineBegin;
			UserState = userState;
		}

		/// <summary>
		///   Gets the zero-based position of the next character that will be read by the input stream.
		/// </summary>
		public int Position { get; private set; }

		/// <summary>
		///   Gets the line number of the next character that will be read by the input stream, starting with 1.
		/// </summary>
		public int Line { get; private set; }

		/// <summary>
		///   Gets the absolute, zero-based position of the first character of the current line.
		/// </summary>
		public int LineBegin { get; private set; }

		/// <summary>
		///   Gets the relative position of the first character of the current line, starting with 1.
		/// </summary>
		public int Column
		{
			get { return Position - LineBegin + 1; }
		}

		/// <summary>
		///   Gets the current user state.
		/// </summary>
		public TUserState UserState { get; private set; }

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(InputStreamState<TUserState> other)
		{
			return Position == other.Position && Line == other.Line && LineBegin == other.LineBegin &&
				   EqualityComparer<TUserState>.Default.Equals(UserState, other.UserState);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is InputStreamState<TUserState> && Equals((InputStreamState<TUserState>)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Position;
				hashCode = (hashCode * 397) ^ Line;
				hashCode = (hashCode * 397) ^ LineBegin;
				hashCode = (hashCode * 397) ^ EqualityComparer<TUserState>.Default.GetHashCode(UserState);
				return hashCode;
			}
		}

		/// <summary>
		///   Checks the two input streams for equality.
		/// </summary>
		/// <param name="left">The left input stream.</param>
		/// <param name="right">The right input stream.</param>
		public static bool operator ==(InputStreamState<TUserState> left, InputStreamState<TUserState> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Checks the two input streams for inequality.
		/// </summary>
		/// <param name="left">The left input stream.</param>
		/// <param name="right">The right input stream.</param>
		public static bool operator !=(InputStreamState<TUserState> left, InputStreamState<TUserState> right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Advances the position, updating the line count and line begin if necessary.
		/// </summary>
		/// <param name="newline">Indicates whether a new line should be registered.</param>
		internal InputStreamState<TUserState> Advance(bool newline)
		{
			if (newline)
				return new InputStreamState<TUserState>(Position + 1, Line + 1, Position + 1, UserState);

			return new InputStreamState<TUserState>(Position + 1, Line, LineBegin, UserState);
		}
	}
}