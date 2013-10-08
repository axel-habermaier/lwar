namespace Pegasus.AssetsCompiler.UserInterface.Markup.TypeConverters
{
	using System;
	using Platform.Logging;

	/// <summary>
	///   Converts strings to thickness values.
	/// </summary>
	internal class ThicknessConverter : TypeConverter
	{
		/// <summary>
		///   Gets the type the string value is converted to.
		/// </summary>
		protected override Type TargetType
		{
			get { return typeof(Thickness); }
		}

		/// <summary>
		///   Gets the runtime type for the given value.
		/// </summary>
		protected override string RuntimeType
		{
			get { return "Pegasus.Framework.UserInterface.Thickness"; }
		}

		/// <summary>
		///   Converts the given string value into an instance of the target type.
		/// </summary>
		/// <param name="xamlFile">The Xaml file the value is specified in.</param>
		/// <param name="value">The value that should be converted.</param>
		protected override object Convert(XamlFile xamlFile, string value)
		{
			double left, right, top, bottom;
			var split = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			if (split.Length == 1 && Double.TryParse(split[0], out left))
				return new Thickness(left);
			
			if (split.Length == 4 && Double.TryParse(split[0], out left) && Double.TryParse(split[1], out right) &&
					 Double.TryParse(split[2], out top) && Double.TryParse(split[3], out bottom))
				return new Thickness(left, right, top, bottom);

			Log.Die("Failed to parse Thickness value '{0}'.", value);
			return null;
		}

		/// <summary>
		///   Generates the code for the object value.
		/// </summary>
		/// <param name="value">The value the code should be generated for.</param>
		protected override string GenerateInstantiationCode(object value)
		{
			var thickness = (Thickness)value;
			return String.Format("new {0}({1}, {2}, {3}, {4})", RuntimeType, thickness.Left, thickness.Right, thickness.Top, thickness.Bottom);
		}
	}
}