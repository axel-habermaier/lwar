using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Collections;
	using System.Linq;
	using System.Xml.Linq;
	using CodeGeneration;

	/// <summary>
	///   Represents a base class for an object specification in a Xaml file.
	/// </summary>
	internal abstract class XamlElement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="isRoot">Indicates whether the Xaml object is the root object of a Xaml file.</param>
		protected XamlElement(bool isRoot)
		{
			IsRoot = isRoot;
		}

		/// <summary>
		///   Gets the name of the object.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Gets a value indicating whether the Xaml object is the root object of a Xaml file.
		/// </summary>
		protected bool IsRoot { get; private set; }

		/// <summary>
		///   Gets or sets the CLR type of the element.
		/// </summary>
		public Type Type { get;protected set; }

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public abstract void GenerateCode(CodeWriter writer, string assignmentFormat);

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
			Assert.That(IsRoot, "This Xaml element is not the root of the tree.");
			Assert.NotNull(Type, "The type of the Xaml element is unknown.");

			writer.AppendLine("namespace {0}", namespaceName);
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("public class {0} : {1}.{2}", className, GetRuntimeNamespace(), Type.Name);
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("public {0}(Pegasus.Platform.Assets.AssetsManager assets)", className);
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("Pegasus.Assert.ArgumentNotNull(assets);");
						GenerateCode(writer, "");
					});
				});
			});
		}

		/// <summary>
		///   Gets the runtime namespace for the element's type.
		/// </summary>
		protected string GetRuntimeNamespace()
		{
			Assert.NotNull(Type, "No type has been set.");

			var runtimeNamespace = Type.GetCustomAttributes(typeof(RuntimeNamespaceAttribute), true)
									   .OfType<RuntimeNamespaceAttribute>()
									   .SingleOrDefault();

			if (runtimeNamespace == null)
				return Type.Namespace;

			return runtimeNamespace.Name;
		}

		/// <summary>
		///   Creates a Xaml element for the given Xml elemnt.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml element.</param>
		/// <param name="element">The element the Xaml element should be created for.</param>
		public static XamlElement Create(XamlFile xamlFile, XElement element)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(element);

			var type = xamlFile.GetClrType(element);
			var isDictionary = typeof(IDictionary).IsAssignableFrom(type);
			var isList = typeof(IList).IsAssignableFrom(type) && !isDictionary;

			if (isDictionary)
				return new XamlDictionary(xamlFile, element);
			
			if (isList)
				return null;
			
			return new XamlObject(xamlFile, element);
		}
	}
}