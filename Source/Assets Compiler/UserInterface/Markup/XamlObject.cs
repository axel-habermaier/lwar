using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Linq;
	using System.Xml.Linq;
	using CodeGeneration;

	/// <summary>
	///   Represents an object instantiation in a Xaml file.
	/// </summary>
	internal class XamlObject
	{
		/// <summary>
		///   Indicates whether the Xaml object is the root object of a Xaml file.
		/// </summary>
		private readonly bool _isRoot;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="xamlElement">The Xaml element this object should represent.</param>
		/// <param name="isRoot">Indicates whether the Xaml object is the root object of a Xaml file.</param>
		public XamlObject(XamlFile xamlFile, XElement xamlElement, bool isRoot = false)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(xamlElement);

			// Get the type of the object
			var clrType = xamlFile.GetClrType(xamlElement);
			Type = clrType.FullName;

			// Initialize all properties
			Properties = xamlElement.Attributes().Select(attribute => new XamlProperty(xamlFile, clrType, attribute))
									.Union(xamlElement.Elements().Select(element => new XamlProperty(xamlFile, clrType, element)))
									.Where(property => property.IsValid)
									.ToArray();

			// Get the name of the object
			if (isRoot)
				Name = "this";
			else
			{
				var nameProperty = Properties.SingleOrDefault(p => p.Name == "Name");
				if (nameProperty == null)
					Name = xamlFile.GenerateUniqueName(clrType);
				else
					Name = (string)nameProperty.XamlValue.Value;
			}

			_isRoot = isRoot;
		}

		/// <summary>
		///   Gets the CLR type name of the object.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		///   Gets the name of the object.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the Xaml properties of the Xaml object.
		/// </summary>
		public XamlProperty[] Properties { get; private set; }

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		public void GenerateCode(CodeWriter writer)
		{
			Assert.ArgumentNotNull(writer);

			writer.Newline();
			if (!_isRoot)
				writer.AppendLine("var {0} = new {1}();", Name, Type);

			foreach (var property in Properties)
				property.GenerateCode(writer, Name);
		}

		/// <summary>
		///   Generates the code for the Xaml file root object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="namespaceName">The namespace of the generated class.</param>
		/// <param name="className">The name of the generated class.</param>
		public void GenerateCode(CodeWriter writer, string namespaceName, string className)
		{
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNullOrWhitespace(namespaceName);
			Assert.ArgumentNotNullOrWhitespace(className);

			writer.AppendLine("namespace {0}", namespaceName);
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("public class {0} : {1}", className, Type);
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("public {0}(AssetsManager assets, ViewModel viewModel)", className);
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("Assert.ArgumentNotNull(assets);");
						writer.AppendLine("Assert.ArgumentNotNull(viewModel);");
						writer.Newline();

						writer.AppendLine("this.ViewModel = viewModel;");
						GenerateCode(writer);
					});
				});
			});
		}
	}
}