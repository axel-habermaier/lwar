using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Collections.Generic;
	using System.Xml.Linq;
	using CodeGeneration;

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
					key = item.Type;
				else
					key = keyAttribute.Value;

				_values.Add(key, item);
			}
		}

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		public override void GenerateCode(CodeWriter writer)
		{
			writer.AppendLine("var {0} = new {1}.{2}();", Name, GetRuntimeNamespace(), Type.Name);
			foreach (var pair in _values)
			{
				pair.Value.GenerateCode(writer);
				writer.AppendLine("{0}.Add(\"{1}\", {2});", Name, pair.Key, pair.Value.Name);
			}
		}
	}
}