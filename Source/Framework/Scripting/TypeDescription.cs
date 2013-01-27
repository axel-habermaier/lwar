using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;

	/// <summary>
	///   Gets a more user-friendly string representation of registered types instead of the default one.
	/// </summary>
	public static class TypeDescription
	{
		/// <summary>
		///   Assigns a more user-friendly names to each registered type.
		/// </summary>
		private static readonly Dictionary<Type, string> Map = new Dictionary<Type, string>
		{
			{ typeof(byte), "8-bit unsigned integer" },
			{ typeof(sbyte), "8-bit signed integer" },
			{ typeof(char), "UTF-16 character" },
			{ typeof(short), "16-bit signed integer" },
			{ typeof(ushort), "16-bit unsigned integer" },
			{ typeof(int), "32-bit signed integer" },
			{ typeof(uint), "32-bit unsigned integer" },
			{ typeof(long), "64-bit signed integer" },
			{ typeof(ulong), "64-bit unsigned integer" },
			{ typeof(float), "32-bit floating point number" },
			{ typeof(double), "64-bit floating point number" },
			{ typeof(string), "string" }
		};

		/// <summary>
		///   Gets the user-friendly name of the given type. If no user friendly name has been provided,
		///   the default name is returned.
		/// </summary>
		/// <typeparam name="T">The type for which the user-friendly name should be returned.</typeparam>
		public static string GetDescription<T>()
		{
			return GetDescription(typeof(T));
		}

		/// <summary>
		///   Gets the user-friendly name of the given type. If no user friendly name has been provided,
		///   the default name is returned.
		/// </summary>
		/// <param name="type">The type for which the user-friendly name should be returned.</param>
		public static string GetDescription(Type type)
		{
			string name;
			return Map.TryGetValue(type, out name) ? name : type.Name;
		}
	}
}