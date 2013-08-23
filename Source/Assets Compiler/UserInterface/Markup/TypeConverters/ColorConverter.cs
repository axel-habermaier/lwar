using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using System.Drawing;
	using CodeGeneration;
	using Platform.Logging;

	/// <summary>
	///   Converts strings to colors.
	/// </summary>
	internal class ColorConverter : TypeConverter
	{
		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected override Type TargetType
		{
			get { return typeof(Color); }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(string value)
		{
			if (value.StartsWith("#"))
			{
				try
				{
					return Color.FromArgb(System.Convert.ToInt32(value.Substring(1), 16));
				}
				catch (Exception)
				{
					Log.Die("Failed to convert color value '{0}'.", value);
				}
			}

			var color = Color.FromName(value);
			if (value.ToLower() != "transparent" && color.ToArgb() == 0)
				Log.Die("Failed to convert color value '{0}'.", value);

			return color;
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="value">The value the code should be generated for.</param>
		protected override void GenerateInstantiationCode(CodeWriter writer, object value)
		{
			var color = (Color)value;
			writer.Append("Pegasus.Platform.Graphics.Color.FromRgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}
	}
}