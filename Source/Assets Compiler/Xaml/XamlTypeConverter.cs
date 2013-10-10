namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using Framework.UserInterface;
	using Platform.Graphics;
	using Platform.Logging;

	/// <summary>
	///   Converts a Xaml value string into a given type's string representation.
	/// </summary>
	internal static class XamlTypeConverter
	{
		/// <summary>
		///   Maps types to their type converters.
		/// </summary>
		private static readonly Dictionary<Type, Func<string, string>> Converters = new Dictionary<Type, Func<string, string>>
		{
			{ typeof(bool), s => s },
			{ typeof(string), s => String.Format("\"{0}\"", s) },
			{ typeof(object), s => String.Format("\"{0}\"", s) },
			{ typeof(double), s => s },
			{ typeof(float), s => s },
			{ typeof(byte), s => s },
			{ typeof(char), s => s },
			{ typeof(short), s => s },
			{ typeof(ushort), s => s },
			{ typeof(int), s => s },
			{ typeof(uint), s => s },
			{ typeof(long), s => s },
			{ typeof(ulong), s => s },
			{ typeof(Thickness), s => String.Format("new Pegasus.Framework.UserInterface.Thickness({0})", s) },
			{ typeof(Color), ConvertColor },
			{ typeof(Type), s => String.Format("typeof({0})", s) },
			{ typeof(XamlLiteral), s => s }
		};

		/// <summary>
		///   Converts the given Xaml value string to the given type's string representation.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="value">The value that should be converted.</param>
		public static string Convert(Type targetType, string value)
		{
			Assert.ArgumentNotNull(targetType);
			Assert.ArgumentNotNull(value);

			if (targetType.IsEnum)
				return String.Format("{0}.{1}", targetType.FullName, Enum.Parse(targetType, value).ToString());

			Func<string, string> converter;
			if (!Converters.TryGetValue(targetType, out converter))
				Log.Die("Unable to find a type converter for type '{0}'.", targetType.FullName);

			try
			{
				return converter(value.Trim());
			}
			catch (Exception e)
			{
				Log.Die("Failed to convert '{0}' to type '{1}': {2}", value, targetType.FullName, e.Message);
				return null;
			}
		}

		/// <summary>
		///   Converts a color value.
		/// </summary>
		/// <param name="value">The color that should be converted.</param>
		private static string ConvertColor(string value)
		{
			var color = new System.Drawing.Color();
			if (value.StartsWith("#"))
			{
				try
				{
					color = System.Drawing.Color.FromArgb(System.Convert.ToInt32(value.Substring(1), 16));
				}
				catch (Exception)
				{
					Log.Die("Failed to convert color value '{0}'.", value);
				}
			}
			else
			{
				color = System.Drawing.Color.FromName(value);
				if (value.ToLower() != "transparent" && color.ToArgb() == 0)
					Log.Die("Failed to convert color value '{0}'.", value);
			}

			return String.Format("Pegasus.Platform.Graphics.Color.FromRgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}
	}
}