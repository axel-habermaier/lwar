using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Platform.Memory;

	/// <summary>
	///   Defines assertion helpers that can be used to check for errors. The checks are only performed in debug builds.
	/// </summary>
	public static class Assert
	{
		/// <summary>
		///   Throws an ArgumentNullException if the argument is null.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check for null.</typeparam>
		/// <param name="argument">The argument to check for null.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNull<T>(T argument, Expression<Func<T>> argumentName)
			where T : class
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (argument == null)
				throw new ArgumentNullException(argumentName.GetReferencedVariableName(),
												String.Format("Parameter '{0}' cannot be null.", argumentName.GetReferencedVariableName()));
		}

		/// <summary>
		///   Throws an InvalidOperationException if the pointer is null.
		/// </summary>
		/// <param name="ptr">The ptr to check for null.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNull(IntPtr ptr, Expression<Func<IntPtr>> argumentName)
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (ptr == IntPtr.Zero)
				throw new ArgumentNullException(argumentName.GetReferencedVariableName(),
												String.Format("Parameter '{0}' cannot be null.", argumentName.GetReferencedVariableName()));
		}

		/// <summary>
		///   Throws an ArgumentNullException if the string argument is null or an
		///   ArgumentException if the string is empty (or only whitespace).
		/// </summary>
		/// <param name="argument">The argument to check.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNullOrWhitespace(string argument, Expression<Func<string>> argumentName)
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (argument == null)
				throw new ArgumentNullException(argumentName.GetReferencedVariableName());

			if (String.IsNullOrWhiteSpace(argument))
			{
				var message = String.Format("String parameter '{0}' cannot be empty or consist of whitespace only.",
											argumentName.GetReferencedVariableName());
				throw new ArgumentException(message, argumentName.GetReferencedVariableName());
			}
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the enum argument is outside the range of the enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="argument">The enumeration value to check.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<TEnum>(TEnum argument, Expression<Func<TEnum>> argumentName)
			where TEnum : struct
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (!Enum.IsDefined(typeof(TEnum), argument))
				throw new ArgumentOutOfRangeException(argumentName.GetReferencedVariableName());
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="argument">The value to check.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check.</param>
		/// <param name="min">The lower bound of the range.</param>
		/// <param name="max">The upper bound of the range.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<T>(T argument, Expression<Func<T>> argumentName, T min, T max)
			where T : IComparable<T>
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (argument.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException(argumentName.GetReferencedVariableName());

			if (argument.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(argumentName.GetReferencedVariableName());
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the array that specifies the bounds.</typeparam>
		/// <param name="argument">The value of the index argument to check.</param>
		/// <param name="argumentName">An expression that identifies the name of the argument to check.</param>
		/// <param name="array">The array that defines the valid range of the given index argument.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<T>(int argument, Expression<Func<int>> argumentName, T[] array)
		{
			ArgumentNotNull(array, () => array);
			ArgumentInRange(argument, argumentName, 0, array.Length);
		}

		/// <summary>
		///   Throws an ArgumentException if the condition does not hold.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check.</typeparam>
		/// <param name="condition">The condition that, if false, causes the exception to be raised.</param>
		/// <param name="argumentName">
		///   An expression identifying the name of the argument that caused the exception.
		/// </param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentSatisfies<T>(bool condition,
												Expression<Func<T>> argumentName,
												string formatMessage,
												params object[] parameters)
		{
			if (argumentName == null)
				throw new ArgumentNullException("argumentName");

			if (formatMessage == null)
				throw new ArgumentNullException("formatMessage");

			if (!condition)
			{
				throw new ArgumentException(String.Format(formatMessage, parameters),
											argumentName.GetReferencedVariableName());
			}
		}

		/// <summary>
		///   Throws an InvalidOperationException if the object is null.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check for null.</typeparam>
		/// <param name="obj">The object to check for null.</param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotNull<T>(T obj, string formatMessage, params object[] parameters)
			where T : class
		{
			if (formatMessage == null)
				throw new ArgumentNullException("formatMessage");

			if (obj == null)
				throw new InvalidOperationException(String.Format(formatMessage, parameters));
		}

		/// <summary>
		///   Throws an InvalidOperationException if the object is null.
		/// </summary>
		/// <typeparam name="T">The type of the argument to check for null.</typeparam>
		/// <param name="obj">The object to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotNull<T>(T obj)
			where T : class
		{
			if (obj == null)
				throw new InvalidOperationException("Expected valid reference.");
		}

		/// <summary>
		///   Throws an InvalidOperationException if the string argument is null or empty (or only whitespace).
		/// </summary>
		/// <param name="s">The string to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotNullOrWhitespace(string s)
		{
			if (s == null)
				throw new ArgumentNullException("s");

			if (String.IsNullOrWhiteSpace(s))
				throw new InvalidOperationException("String cannot be empty or consist of whitespace only.");
		}

		/// <summary>
		///   Throws an InvalidOperationException if the condition does not hold.
		/// </summary>
		/// <param name="condition">The condition that, if false, causes the exception to be raised.</param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void That(bool condition, string formatMessage, params object[] parameters)
		{
			if (formatMessage == null)
				throw new ArgumentNullException("formatMessage");

			if (!condition)
				throw new InvalidOperationException(String.Format(formatMessage, parameters));
		}

		/// <summary>
		///   Throws an ObjectDisposedException if the given object has already been disposed.
		/// </summary>
		/// <param name="obj">The object that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void NotDisposed<T>(T obj)
			where T : DisposableObject
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			if (obj.IsDisposed)
				throw new ObjectDisposedException(obj.GetType().FullName);
		}

		/// <summary>
		///   Throws an InvalidOperationException if the enum argument is outside the range of the enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="argument">The enumeration value to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void InRange<TEnum>(TEnum argument)
			where TEnum : struct
		{
			if (!Enum.IsDefined(typeof(TEnum), argument))
				throw new InvalidOperationException("The enumeration value lies outside the allowable range.");
		}

		/// <summary>
		///   Throws an InvalidOperationException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="index">The value to check.</param>
		/// <param name="min">The lower bound of the range.</param>
		/// <param name="max">The upper bound of the range.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void InRange<T>(T index, T min, T max)
			where T : IComparable<T>
		{
			if (index.CompareTo(min) < 0)
				throw new InvalidOperationException(String.Format(
					"Lower bound violation. Expected argument to lie between {0} and {1}.", min, max));

			if (index.CompareTo(max) > 0)
				throw new InvalidOperationException(String.Format(
					"Upper bound violation. Expected argument to lie between {0} and {1}.", min, max));
		}

		/// <summary>
		///   Throws an InvalidOperationException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the array that specifies the bounds.</typeparam>
		/// <param name="index">The value of the index to check.</param>
		/// <param name="array">The array that defines the valid range of the given index argument.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void InRange<T>(int index, T[] array)
		{
			ArgumentNotNull(array, () => array);
			InRange(index, 0, array.Length);
		}

		/// <summary>
		///   Gets the name of the variable referenced by the expression. Expression must be a lambda function
		///   of the form () => var, where var is a locally defined variable.
		/// </summary>
		/// <typeparam name="T">The return type of expression.</typeparam>
		/// <param name="expression">The expression from which to get the variable name.</param>
		/// <returns>Returns the variable name.</returns>
		/// <exception cref="ArgumentException">Expression is not of the expected form.</exception>
		private static string GetReferencedVariableName<T>(this Expression<Func<T>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("Expression must be of the form () => var, where var is a locally defined variable.", "expression");
			}

			return memberExpression.Member.Name;
		}
	}
}