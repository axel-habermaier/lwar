using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Linq;
	using System.Xml.Linq;

	/// <summary>
	///   Represents an object instantiation in a Xaml file.
	/// </summary>
	internal class XamlObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="xamlElement">The Xaml element this object should represent.</param>
		public XamlObject(XamlFile xamlFile, XElement xamlElement)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			var clrType = xamlFile.GetClrType(xamlElement);
			Type = clrType.FullName;

			Properties = xamlElement.Attributes().Select(attribute => new XamlProperty(xamlFile, clrType,attribute))
									.Union(xamlElement.Elements().Select(element => new XamlProperty(xamlFile,clrType, element)))
									.Where(property => property.IsValid)
									.ToArray();
		}

		/// <summary>
		///   Gets the CLR type name of the object.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		///   Gets the Xaml properties of the Xaml object.
		/// </summary>
		public XamlProperty[] Properties { get; private set; }
	}
}