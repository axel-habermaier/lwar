namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Xml.Linq;
	using Markup;
	using Platform.Logging;

	/// <summary>
	///   Provides metadata for the 'UIElement' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal abstract class UIElement : IRequiresNormalization
	{
		[IgnoreAtRuntime]
		public string Name { get; set; }

		public ResourceDictionary Resources { get; set; }

		public double Width { get; set; }
		public double Height { get; set; }
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }
		public double MaxWidth { get; set; }
		public double MaxHeight { get; set; }

		public Color Background { get; set; }
		public Color Foreground { get; set; }

		public Style Style { get; set; }
		public bool Visible { get; set; }
		public Thickness Margin { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }

		public string FontFamily { get; set; }
		public int FontSize { get; set; }
		public bool FontBold { get; set; }
		public bool FontAliased { get; set; }
		public bool FontItalic { get; set; }

		/// <summary>
		///   Normalizes the contents of the given XAML element.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		public virtual void Normalize(XElement xamlElement)
		{
			NormalizeFontBold(xamlElement);
			NormalizeFontAliased(xamlElement);
			NormalizeFontItalic(xamlElement);
			NormalizeVisibility(xamlElement);
		}

		/// <summary>
		///   Normalizes the 'FontWeight' Xaml property to a Boolean value and renames it to 'FontBold'.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private static void NormalizeFontBold(XElement xamlElement)
		{
			Normalize(xamlElement, "FontWeight", "FontBold", value =>
			{
				switch (value)
				{
					case "Normal":
						return "false";
					case "Bold":
						return "true";
					default:
						Log.Die("Unsupported font weight value '{0}'.", value);
						return String.Empty;
				}
			});
		}

		/// <summary>
		///   Normalizes the 'FontStyle' Xaml property to a Boolean value and renames it to 'FontItalic'.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private static void NormalizeFontItalic(XElement xamlElement)
		{
			Normalize(xamlElement, "FontStyle", "FontItalic", value =>
			{
				switch (value)
				{
					case "Normal":
						return "false";
					case "Italic":
						return "true";
					default:
						Log.Die("Unsupported font style value '{0}'.", value);
						return String.Empty;
				}
			});
		}

		/// <summary>
		///   Normalizes the 'TextOptions.TextRenderingMode' Xaml attached property to a Boolean value and renames it to
		///   'FontAliased'.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private static void NormalizeFontAliased(XElement xamlElement)
		{
			Normalize(xamlElement, "TextOptions.TextRenderingMode", "FontAliased", value =>
			{
				switch (value)
				{
					case "ClearType":
						return "false";
					case "Aliased":
						return "true";
					default:
						Log.Die("Unsupported text rendering mode value '{0}'.", value);
						return String.Empty;
				}
			});
		}

		/// <summary>
		///   Normalizes the 'Visibility' Xaml attached property to a Boolean value and renames it to 'Visible'.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		private static void NormalizeVisibility(XElement xamlElement)
		{
			Normalize(xamlElement, "Visibility", "Visible", value =>
			{
				switch (value)
				{
					case "Collapsed":
						return "false";
					case "Visible":
						return "true";
					default:
						Log.Die("Unsupported visibility value '{0}'.", value);
						return String.Empty;
				}
			});
		}

		/// <summary>
		///   Normalizes the given Xaml element's attribute or element with the given property name by applying the value converter
		///   and renaming the property.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		/// <param name="propertyName">The name of the property that should be normalized.</param>
		/// <param name="newPropertyName">The name of the property after normalization.</param>
		/// <param name="valueConverter">The value converter that normalizes the property's value.</param>
		private static void Normalize(XElement xamlElement, string propertyName, string newPropertyName, Func<string, string> valueConverter)
		{
			Assert.ArgumentNotNull(xamlElement);
			Assert.ArgumentNotNullOrWhitespace(propertyName);
			Assert.ArgumentNotNullOrWhitespace(newPropertyName);
			Assert.ArgumentNotNull(valueConverter);

			var attribute = xamlElement.Attributes().SingleOrDefault(a => a.Name.LocalName == propertyName);
			var element = xamlElement.Elements().SingleOrDefault(e => e.Name.LocalName.EndsWith(propertyName));

			if (attribute != null)
			{
				attribute.Remove();
				xamlElement.Add(new XAttribute(XamlFile.DefaultNamespace + newPropertyName, valueConverter(attribute.Value)));
			}

			if (element == null)
				return;

			element.Remove();
			xamlElement.Add(new XElement(XamlFile.DefaultNamespace + element.Name.LocalName.Replace(propertyName, newPropertyName),
										 valueConverter(element.Value)));
		}
	}
}