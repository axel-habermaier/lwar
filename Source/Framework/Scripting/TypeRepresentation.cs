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
			{ typeof(bool), o => (bool)o ? "true" : "false" },
			{ typeof(float), o => ((float)o).ToString("F") },
			{ typeof(double), o => ((double)o).ToString("F") },
			{ typeof(string), o => (string)o }
		};

		/// <summary>
		///   Gets the string representation of the given value.
		/// </summary>
		/// <param name="value">The value for which the string representation should be returned.</param>
		/// <param name="requireQuotes">A value indicating whether string values require quotes.</param>
		public static string ToString(object value, bool requireQuotes = false)
		{
			// Strings have to be enclosed in quotes if required
			if (value is string && requireQuotes)
				return String.Format("\"{0}\"", value);

			// Input triggers cannot be looked up in the dictionary, as they are never of type InputTrigger
			if (value is InputTrigger)
				return String.Format("[{0}]", value);

			Func<object, string> toString;
			if (Map.TryGetValue(value.GetType(), out toString))
				return toString(value);

			return value.ToString();
		}
	}
}