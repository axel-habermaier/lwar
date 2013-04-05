using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using Platform.Input;

	/// <summary>
	///   Gets a string representation of a type's value that can be parsed by the cvar/command parser.
	/// </summary>
	public static class TypeRepresentation
	{
		/// <summary>
		///   Assigns stringification function to each registered type.
		/// </summary>
		private static readonly Dictionary<Type, Func<object, string>> Map = new Dictionary<Type, Func<object, string>>
		{
			{ typeof(bool), o => o.ToString().ToLower() },
			{ typeof(float), o => ((float)o).ToString("F") },
			{ typeof(double), o => ((double)o).ToString("F") },
			{ typeof(string), o => String.Format("\"{0}\"", o.ToString().Replace("\"", "\\\"")) },
			{ typeof(InputTrigger), o => String.Format("[{0}]", o) }
		};

		/// <summary>
		///   Gets the string representation of the given value.
		/// </summary>
		/// <param name="value">The value for which the string representation should be returned.</param>
		public static string ToString(object value)
		{
			Func<object, string> toString;
			if (Map.TryGetValue(value.GetType(), out toString))
				return toString(value);

			return value.ToString();
		}
	}
}