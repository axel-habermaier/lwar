namespace Pegasus.AssetsCompiler.Utilities
{
	using System;
	using System.Diagnostics;

	/// <summary>
	///     Defines assertion helpers that can be used to check for errors. The checks are only performed in debug builds.
	/// </summary>
	public static class Assert
	{
		/// <summary>
		///     Throws an ArgumentNullException if the argument is null.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check for null.</typeparam>
		/// <param name="argument">The argument to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden, ContractAnnotation("null => halt")]
		public static void ArgumentNotNull<T>(T argument)
			where T : class
		{
			if (argument == null)
				throw new ArgumentNullException();
		}

		/// <summary>
		///     Throws an ArgumentNullException if the pointer is null.
		/// </summary>
		/// <param name="pointer">The pointer to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden, ContractAnnotation("null => halt")]
		public static void ArgumentNotNull(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
				throw new ArgumentNullException();
		}

		/// <summary>
		///     Throws an ArgumentException if the string argument is null or an ArgumentException if the string is empty (or
		///     only whitespace).
		/// </summary>
		/// <param name="argument">The argument to check.</param>
		[Conditional("DEBUG"), DebuggerHidden, ContractAnnotation("null => halt")]
		public static void ArgumentNotNullOrWhitespace(string argument)
		{
			ArgumentNotNull(argument);

			if (String.IsNullOrWhiteSpace(argument))
				throw new ArgumentException("String parameter cannot be empty or consist of whitespace only.");
		}

		/// <summary>
		///     Throws an ArgumentOutOfRangeException if the enum argument is outside the range of the enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="argument">The enumeration value to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<TEnum>(TEnum argument)
			where TEnum : struct
		{
			if (!Enum.IsDefined(typeof(TEnum), argument))
				throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		///     Throws an ArgumentOutOfRangeException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="argument">The value to check.</param>
		/// <param name="min">The inclusive lower bound of the range.</param>
		/// <param name="max">The inclusive upper bound of the range.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<T>(T argument, T min, T max)
			where T : IComparable<T>
		{
			if (argument.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException();

			if (argument.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		///     Throws an ArgumentException if the condition does not hold.
		/// </summary>
		/// <param name="condition">The condition that, if false, causes the exception to be raised.</param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden, StringFormatMethod("formatMessage"), ContractAnnotation("condition: false => halt")]
		public static void ArgumentSatisfies(bool condition, string formatMessage, params object[] parameters)
		{
			ArgumentNotNull(formatMessage);

			if (!condition)
				throw new ArgumentException(String.Format(formatMessage, parameters));
		}

		/// <summary>
		///     Throws a PegasusException if the object is null.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check for null.</typeparam>
		/// <param name="obj">The object to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden, ContractAnnotation("null => halt")]
		public static void NotNull<T>(T obj)
			where T : class
		{
			if (obj == null)
				throw new PegasusException("Expected a valid reference.");
		}

		/// <summary>
		///     Throws a PegasusException if the condition does not hold.
		/// </summary>
		/// <param name="condition">The condition that, if false, causes the exception to be raised.</param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden, StringFormatMethod("formatMessage"), ContractAnnotation("condition: false => halt")]
		public static void That(bool condition, string formatMessage, params object[] parameters)
		{
			ArgumentNotNull(formatMessage);

			if (!condition)
				throw new PegasusException(formatMessage, parameters);
		}

		/// <summary>
		///     Throws a PegasusException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="index">The value to check.</param>
		/// <param name="min">The inclusive lower bound of the range.</param>
		/// <param name="max">The inclusive upper bound of the range.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void InRange<T>(T index, T min, T max)
			where T : IComparable<T>
		{
			if (index.CompareTo(min) < 0)
				throw new PegasusException("Lower bound violation. Expected argument to lie between {0} and {1}.", min, max);

			if (index.CompareTo(max) > 0)
				throw new PegasusException("Upper bound violation. Expected argument to lie between {0} and {1}.", min, max);
		}
	}
}