﻿namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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
		private static readonly Dictionary<string, Func<string, string>> Converters = new Dictionary<string, Func<string, string>>
		{
			{ "bool", s => s },
			{ "string", s => String.Format("\"{0}\"", s) },
			{ "object", s => String.Format("\"{0}\"", s) },
			{ "double", s => s },
			{ "float", s => s },
			{ "byte", s => s },
			{ "char", s => s },
			{ "short", s => s },
			{ "ushort", s => s },
			{ "int", s => s },
			{ "uint", s => s },
			{ "long", s => s },
			{ "ulong", s => s },
			{ "Pegasus.Framework.UserInterface.Thickness", s => String.Format("new Thickness({0})", s) },
			{ "Pegasus.Platform.Graphics.Color", ConvertColor },
			{ "System.Type", s => String.Format("typeof({0})", s) },
			{ "Pegasus.AssetCompiler.Xaml.XamlLiteral", s => s }
		};

		/// <summary>
		///   Converts the given Xaml value string to the given type's string representation.
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

			return String.Format("Color.FromRgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}
	}
}