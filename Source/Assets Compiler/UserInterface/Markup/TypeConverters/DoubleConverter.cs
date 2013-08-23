﻿using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using CodeGeneration;

	/// <summary>
	///   Converts strings to doubles.
	/// </summary>
	internal class DoubleConverter : TypeConverter
	{
		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected override Type TargetType
		{
			get { return typeof(double); }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(string value)
		{
			return Double.Parse(value);
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="value">The value the code should be generated for.</param>
		protected override void GenerateInstantiationCode(CodeWriter writer, object value)
		{
			writer.Append("{0}", value);
		}
	}
}