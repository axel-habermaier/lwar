namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///     Converts a Xaml value string into a given type's string representation.
	/// </summary>
	internal static class XamlTypeConverter
	{
		/// <summary>
		///     Maps types to their type converters.
		/// </summary>
		private static readonly Dictionary<string, Func<string, string>> Converters = new Dictionary<string, Func<string, string>>
		{
			{ "bool", s => s.ToLower() },
			{ "string", s => String.Format("\"{0}\"", s.Replace("\"", "\\\"")) },
			{ "object", s => String.Format("\"{0}\"", s.Replace("\"", "\\\"")) },
			{ "double", s => s.ToLower() == "auto" ? "Double.NaN" : s },
			{ "float", s => s },
			{ "byte", s => s },
			{ "char", s => s },
			{ "short", s => s },
			{ "ushort", s => s },
			{ "int", s => s },
			{ "uint", s => s },
			{ "long", s => s },
			{ "ulong", s => s },
			{ "Pegasus.Framework.UserInterface.Thickness", s => String.Format("new Pegasus.Framework.UserInterface.Thickness({0})", s) },
			{ "Pegasus.Platform.Graphics.Color", ConvertColor },
			{ "Pegasus.Platform.Graphics.Color?", ConvertColor },
			{ "System.Type", s => String.Format("typeof({0})", s) },
			{ "Pegasus.AssetCompiler.Xaml.XamlLiteral", s => s },
			{ "Pegasus.Framework.UserInterface.Input.KeyModifiers", ConvertKeyModifiers },
			{ "Pegasus.Scripting.Cvar", s => s }
		};

		/// <summary>
		///     Converts the given Xaml value string to the given type's string representation.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="value">The value that should be converted.</param>
		public static string Convert(IXamlType targetType, string value)
		{
			Assert.ArgumentNotNull(targetType);
			Assert.ArgumentNotNull(value);

			var enumeration = targetType as XamlEnumeration;
			if (enumeration != null)
			{
				if (enumeration.Literals.All(l => l != value))
					Log.Die("Unable to find enumeration literal '{0}.{1}'.", targetType.FullName, value);

				return String.Format("{0}.{1}", targetType.Name, value);
			}

			Func<string, string> converter;
			if (!Converters.TryGetValue(targetType.FullName, out converter))
				Log.Die("Unable to find a type converter for type '{0}'.", targetType.FullName);

			try
			{
				return converter(value);
			}
			catch (Exception e)
			{
				Log.Die("Failed to convert '{0}' to type '{1}': {2}", value, targetType.FullName, e.Message);
				return null;
			}
		}

		/// <summary>
		///     Converts a key modifiers value.
		/// </summary>
		/// <param name="value">The key modifiers value that should be converted.</param>
		private static string ConvertKeyModifiers(string value)
		{
			var modifiers = value.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
			return String.Join(" | ", modifiers.Select(m => "Pegasus.Framework.UserInterface.Input.KeyModifiers." + m));
		}

		/// <summary>
		///     Converts a color value.
		/// </summary>
		/// <param name="value">The color that should be converted.</param>
		private static string ConvertColor(string value)
		{
			var color = new Color();
			if (value.StartsWith("#"))
			{
				try
				{
					color = Color.FromArgb(System.Convert.ToInt32(value.Substring(1), 16));
				}
				catch (Exception)
				{
					Log.Die("Failed to convert color value '{0}'.", value);
				}
			}
			else
			{
				color = Color.FromName(value);
				if (value.ToLower() != "transparent" && color.ToArgb() == 0)
					Log.Die("Failed to convert color value '{0}'.", value);
			}

			return String.Format("new Color({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}
	}
}