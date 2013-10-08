using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	/// <summary>
	///   Converts strings to Booleans.
	/// </summary>
	internal class BooleanConverter : TypeConverter
	{
		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected override Type TargetType
		{
			get { return typeof(bool); }
		}

		/// <summary>
		///   Gets the runtime type for the given value.
		/// </summary>
		protected override string RuntimeType
		{
			get { return "bool"; }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(XamlFile xamlFile, string value)
		{
			return Boolean.Parse(value);
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="value">The value the code should be generated for.</param>
		protected override string GenerateInstantiationCode(object value)
		{
			return String.Format("{0}", (bool)value ? "true" : "false");
		}
	}
}