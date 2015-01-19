namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using Utilities;

	/// <summary>
	///     Converts a Xaml value string into a given type's string representation.
	/// </summary>
	internal static class XamlTypeConverter
	{
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
		///     Converts an asset value.
		/// </summary>
		/// <param name="value">The asset value that should be converted.</param>
		private static string ConvertAsset(string value)
		{
			var index = value.LastIndexOf(".", StringComparison.Ordinal);
			var bundleType = value.Substring(0, index);
			var assetName = value.Substring(index + 1);
			return String.Format("Pegasus.Application.Current.RenderContext.GetAssetBundle<{0}>().{1}", bundleType, assetName);
		}

		/// <summary>
		///     Converts a key modifiers value.
		/// </summary>
		/// <param name="value">The key modifiers value that should be converted.</param>
		private static string ConvertKeyModifiers(string value)
		{
			var modifiers = value.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
			return String.Join(" | ", modifiers.Select(m => "Pegasus.UserInterface.Input.KeyModifiers." + m));
		}

		/// <summary>
		///     Converts a nullable color value.
		/// </summary>
		/// <param name="value">The color that should be converted.</param>
		private static string ConvertNullableColor(string value)
		{
			if (value == "{x:Null}")
				return "null";

			return ConvertColor(value);
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

			return String.Format("new Pegasus.Rendering.Color({0}, {1}, {2}, {3})", color.R, color.G, color.B, color.A);
		}

		/// <summary>
		///     Maps types to their type converters.
		/// </summary>
		private static readonly Dictionary<string, Func<string, string>> Converters = new Dictionary<string, Func<string, string>>
		{
			{ "bool", s => s.ToLower() },
			{ "string", s => String.Format("\"{0}\"", s.Replace("\"", "\\\"")) },
			{ "object", s => String.Format("\"{0}\"", s.Replace("\"", "\\\"")) },
			{ "double", s => s.ToLower() == "auto" ? "Double.NaN" : s },
			{ "float", s => s.ToLower() == "auto" ? "Single.NaN" : s },
			{ "byte", s => s },
			{ "char", s => s },
			{ "short", s => s },
			{ "ushort", s => s },
			{ "int", s => s },
			{ "uint", s => s },
			{ "long", s => s },
			{ "ulong", s => s },
			{ "Pegasus.UserInterface.Thickness", s => String.Format("new Pegasus.UserInterface.Thickness({0})", s) },
			{ "Pegasus.Rendering.Color", ConvertColor },
			{ "Pegasus.Rendering.Color?", ConvertNullableColor },
			{ "System.Type", s => String.Format("typeof({0})", s) },
			{ "Pegasus.AssetCompiler.Xaml.XamlLiteral", s => s },
			{ "Pegasus.UserInterface.Input.KeyModifiers", ConvertKeyModifiers },
			{ "Pegasus.Scripting.Cvar", s => s },
			{ "Pegasus.Platform.Graphics.Texture2D", ConvertAsset },
			{ "Pegasus.Math.Vector2", s => String.Format("new Pegasus.Math.Vector2({0})", s) },
			{ "Pegasus.UserInterface.Input.Cursor", ConvertAsset }
		};
	}
}