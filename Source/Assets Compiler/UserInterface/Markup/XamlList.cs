using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Collections.Generic;
	using System.Xml.Linq;
	using CodeGeneration;

	/// <summary>
	///   Represents a list instantiation in a Xaml file.
	/// </summary>
	internal class XamlList : XamlElement
	{
		/// <summary>
		///   The values stored in the list.
		/// </summary>
		private readonly List<XamlElement> _items = new List<XamlElement>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="xamlElement">The Xaml element this object should represent.</param>
		public XamlList(XamlFile xamlFile, XElement xamlElement)
			: base(false)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			Type = xamlFile.GetClrType(xamlElement);
			Name = xamlFile.GenerateUniqueName(Type);

			foreach (var element in xamlElement.Elements())
				_items.Add(Create(xamlFile, element));
		}

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			foreach (var item in _items)
				item.GenerateCode(writer, String.Format(assignmentFormat, String.Format("Add({{0}})", Name)));
		}
	}
}