namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///   Provides metadata for the 'Style' UI class.
	/// </summary>
	[ContentProperty("Setters")]
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	[ImplicitKey("TargetType")]
	internal class Style : IRequiresNormalization
	{
		[IgnoreAtRuntime]
		public Type TargetType { get; set; }

		public SetterCollection Setters { get; set; }

		/// <summary>
		///   Normalizes the contents of the style definition such that the style's target type is pushed down to all setters' that
		///   do not specify the target type of the property. Furthermore, the property type information is added to all setters'
		///   value elements.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		public void Normalize(XElement xamlElement)
		{
			var targetType = FindTargetType(xamlElement);

			foreach (var settersCollection in xamlElement.Elements().Where(e => e.Name.LocalName == "Style.Setters"))
			{
				foreach (var setter in settersCollection.Elements().Where(e => e.Name.LocalName == "Setter"))
				{
					var property = NormalizePropertyType(setter, targetType);
					NormalizeValue(setter, property);
				}
			}
		}

		/// <summary>
		///   Normalizes the property type of the given setter.
		/// </summary>
		/// <param name="setter">The setter that should be normalized.</param>
		/// <param name="targetType">The target type of the property or null if unspecified.</param>
		private static string NormalizePropertyType(XElement setter, string targetType)
		{
			var attribute = setter.Attributes().FirstOrDefault(a => a.Name.LocalName == "Property");
			if (attribute != null && !attribute.Value.Contains("."))
			{
				attribute.SetValue(String.Format("{0}.{1}", targetType, attribute.Value));
				return attribute.Value;
			}

			if (attribute != null)
				return attribute.Value;

			var element = setter.Elements().FirstOrDefault(e => e.Name.LocalName == "Setter.Property");
			if (element != null && !element.Value.Contains("."))
			{
				element.SetValue(String.Format("{0}.{1}", targetType, element.Value));
				return element.Value;
			}

			if (element != null)
				return element.Value;

			Log.Die("Unable to find 'Property' element or attribute of setter '{0}'.", setter);
			return null;
		}

		/// <summary>
		///   Normalizes the value of the given setter.
		/// </summary>
		/// <param name="setter">The setter that should be normalized.</param>
		/// <param name="property">The property that is set.</param>
		private static void NormalizeValue(XElement setter, string property)
		{
			var attribute = setter.Attributes().FirstOrDefault(a => a.Name.LocalName == "Value");
			if (attribute != null)
				attribute.SetValue(String.Format("[typeof({0})]{1}", property, attribute.Value));

			var element = setter.Elements().FirstOrDefault(e => e.Name.LocalName == "Setter.Value");
			if (element != null)
				element.SetValue(String.Format("[typeof({0})]{1}", property, element.Value));

			if (attribute == null && element == null)
				Log.Die("Unable to find 'Value' element or attribute of setter '{0}'.", setter);
		}

		/// <summary>
		///   Finds the target type of the given style element.
		/// </summary>
		/// <param name="xamlElement">The XAML style element the target type should be found for.</param>
		private static string FindTargetType(XElement xamlElement)
		{
			var attribute = xamlElement.Attributes().FirstOrDefault(a => a.Name.LocalName == "TargetType");
			if (attribute != null)
				return attribute.Value;

			var element = xamlElement.Elements().FirstOrDefault(a => a.Name.LocalName == "Style.TargetType");
			if (element != null)
				return element.Value;

			return null;
		}
	}
}