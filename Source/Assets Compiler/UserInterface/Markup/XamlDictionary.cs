using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using CodeGeneration;
	using Platform.Logging;
	using TypeConverters;

	/// <summary>
	///   Represents a dictionary instantiation in a Xaml file.
	/// </summary>
	internal class XamlDictionary : XamlElement
	{
		/// <summary>
		///   The values stored in the dictionary.
		/// </summary>
		private readonly Dictionary<object, XamlElement> _values = new Dictionary<object, XamlElement>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="xamlElement">The Xaml element this object should represent.</param>
		/// <param name="isRoot">Indicates whether the Xaml object is the root object of a Xaml file.</param>
		public XamlDictionary(XamlFile xamlFile, XElement xamlElement, bool isRoot = false)
			: base(isRoot)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			Type = xamlFile.GetClrType(xamlElement);
			Name = xamlFile.GenerateUniqueName(Type);

			foreach (var element in xamlElement.Elements())
			{
				var item = Create(xamlFile, element);

				object key;
				var keyAttribute = element.Attribute(XamlFile.MarkupNamespace + "Key");

				if (keyAttribute == null)
				{
					// Check whether the item type defines an implicit key and use that instead
					var implicitKeyAttribute = item.Type.GetCustomAttribute<ImplicitKeyAttribute>();
					if (implicitKeyAttribute == null)
						Log.Die("Key definition missing for dictionary item of type '{0}'.", item.Type.FullName);

					var xamlObject = item as XamlObject;
					if (xamlObject == null)
						Log.Die("Key definition missing for dictionary item of type '{0}'.", item.Type.FullName);

					var xamlProperty = xamlObject.Properties.FirstOrDefault(p => p.Name == implicitKeyAttribute.Property);
					if (xamlProperty == null)
						Log.Die("Key definition missing for dictionary item of type '{0}'.", item.Type.FullName);

					var xamlValue = xamlProperty.Value as XamlValue;
					if (xamlValue == null)
						Log.Die("Key definition missing for dictionary item of type '{0}'.", item.Type.FullName);

					var implicitKey = xamlValue.Value as Type;
					if (implicitKey == null)
						Log.Die("Key definition missing for dictionary item of type '{0}'.", item.Type.FullName);

					key = String.Format("typeof({0})", implicitKey.GetRuntimeType());
				}
				else
					key = String.Format("\"{0}\"", keyAttribute.Value);

				_values.Add(key, item);
			}
		}

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			foreach (var pair in _values)
				pair.Value.GenerateCode(writer, String.Format(assignmentFormat, String.Format("Add({1}, {{0}});", Name, pair.Key)));
		}
	}
}