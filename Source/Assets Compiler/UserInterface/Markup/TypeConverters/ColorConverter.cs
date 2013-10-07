namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using System;
	using System.Drawing;
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
		///   Gets the runtime type for the given value.
		/// </summary>
		protected override string RuntimeType
		{
			get { return "Pegasus.Platform.Graphics.Color"; }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(XamlFile xamlFile, string value)
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
		/// <param name="value">The value the code should be generated for.</param>
		protected override string GenerateInstantiationCode(object value)
		{
			var color = (Color)value;
			return String.Format("Pegasus.Platform.Graphics.Color.FromRgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}
	}
}