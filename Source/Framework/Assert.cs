﻿using System;

// ReSharper disable UnusedParameter.Global

namespace Pegasus.Framework
{
	using System.Diagnostics;
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
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNull<T>(T argument)
			where T : class
		{
			if (argument == null)
				throw new ArgumentNullException("Reference type parameter cannot be null.");
		}

		/// <summary>
		///   Throws an InvalidOperationException if the pointer is null.
		/// </summary>
		/// <param name="ptr">The ptr to check for null.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNull(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				throw new ArgumentNullException("Pointer type parameter cannot be null.");
		}

		/// <summary>
		///   Throws an ArgumentNullException if the string argument is null or an ArgumentException if the string is empty (or
		///   only whitespace).
		/// </summary>
		/// <param name="argument">The argument to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentNotNullOrWhitespace(string argument)
		{
			if (argument == null)
				throw new ArgumentNullException("String parameter cannot be null.");

			if (String.IsNullOrWhiteSpace(argument))
				throw new ArgumentException("String parameter cannot be empty or consist of whitespace only.");
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the enum argument is outside the range of the enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="argument">The enumeration value to check.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<TEnum>(TEnum argument)
			where TEnum : struct
		{
			if (!Enum.IsDefined(typeof(TEnum), argument))
				throw new ArgumentOutOfRangeException("Enumeration parameter is out of range.");
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="argument">The value to check.</param>
		/// <param name="min">The lower bound of the range.</param>
		/// <param name="max">The upper bound of the range.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<T>(T argument, T min, T max)
			where T : IComparable<T>
		{
			if (argument.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException("Parameter underflows.");

			if (argument.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException("Parameter overflows.");
		}

		/// <summary>
		///   Throws an ArgumentOutOfRangeException if the argument is outside the range.
		/// </summary>
		/// <typeparam name="T">The type of the array that specifies the bounds.</typeparam>
		/// <param name="argument">The value of the index argument to check.</param>
		/// <param name="array">The array that defines the valid range of the given index argument.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentInRange<T>(int argument, T[] array)
		{
			ArgumentNotNull(array);
			ArgumentInRange(argument, 0, array.Length);
		}

		/// <summary>
		///   Throws an ArgumentException if the condition does not hold.
		/// </summary>
		/// <param name="condition">The condition that, if false, causes the exception to be raised.</param>
		/// <param name="formatMessage">An error message explaining the exception to the user.</param>
		/// <param name="parameters">The parameters for the error message.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		public static void ArgumentSatisfies(bool condition, string formatMessage, params object[] parameters)

		{
			ArgumentNotNull(formatMessage);

			if (!condition)
				throw new ArgumentException(String.Format(formatMessage, parameters));
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
			ArgumentNotNull(array);
			InRange(index, 0, array.Length);
		}
	}
}

// ReSharper restore UnusedParameter.Global