using System;

namespace Pegasus.AssetsCompiler.Xaml
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using Framework;
	using Framework.UserInterface;
	using Platform.Logging;

	/// <summary>
	///   Applies a number of transformations to a Xaml file to facilitate the cross-compilation of the Xaml code to C#.
	/// </summary>
	internal class XamlFile
	{
		/// <summary>
		///   The default Xaml namespace.
		/// </summary>
		public static readonly XNamespace DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		/// <summary>
		///   The Xaml markup namespace.
		/// </summary>
		public static readonly XNamespace MarkupNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

		/// <summary>
		///   Maps a class name to the number of instances created of the class.
		/// </summary>
		private readonly Dictionary<string, int> _instancesCount = new Dictionary<string, int>();

		/// <summary>
		///   Maps an Xml namespace to the corresponding Xaml namespaces.
		/// </summary>
		private readonly Dictionary<string, XamlNamespace[]> _namespaceMap = new Dictionary<string, XamlNamespace[]>();

		/// <summary>
		///   The object names that are used by the Xaml file.
		/// </summary>
		private readonly HashSet<string> _usedNames = new HashSet<string>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The file name of the Xaml file.</param>
		public XamlFile(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			Root = XElement.Parse(File.ReadAllText(fileName), LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);

			BuildNamespaceMap();
			Transform();
		}

		/// <summary>
		///   The root object of the Xaml file.
		/// </summary>
		public XElement Root { get; private set; }

		/// <summary>
		///   Transforms the Xaml root object.
		/// </summary>
		private void Transform()
		{
			TrimElementLiterals();
			RemoveIgnorableElements();
			AddImplicitContentElements();
			RemoveNamespaceDeclarations();
			ReplaceAttributesWithElements(Root);

			PushDownStyleTargetType();
			AddStyleSetterLiterals();

			ReplaceTextRenderingMode();

			AddLiterals();
		}

		/// <summary>
		///   Trims the literal values of all elements.
		/// </summary>
		private void TrimElementLiterals()
		{
			foreach (var element in Root.DescendantsAndSelf())
			{
				if (!element.Elements().Any())
					element.SetValue(element.Value.Trim());
			}
		}

		/// <summary>
		///   Renames the 'TextOptions.TextRenderingMode' Xaml attached property to 'FontAliased' and normalizes it to a Boolean
		///   value.
		/// </summary>
		private void ReplaceTextRenderingMode()
		{
			foreach (var element in GetNamedElements(Root, "TextOptions.TextRenderingMode").ToArray())
			{
				string value = null;
				switch (element.Value)
				{
					case "ClearType":
						value = "false";
						break;
					case "Aliased":
						value = "true";
						break;
					default:
						Log.Die("Unsupported text rendering mode value '{0}'.", element.Value);
						break;
				}

				element.Parent.Add(new XElement(element.Parent.Name + "." + "FontAliased", value));
				element.Remove();
			}
		}

		/// <summary>
		///   Adds 'Literal' elements for literal setter values.
		/// </summary>
		private void AddStyleSetterLiterals()
		{
			foreach (var setter in GetNamedElements(Root, "Setter"))
			{
				var value = GetNamedElement(setter, "Setter.Value");
				if (value.Elements().Any())
					continue;

				var property = GetNamedElement(setter, "Setter.Property");
				var type = GetPropertyType(property.Value);

				ReplaceWithLiteral(value, type);
			}
		}

		/// <summary>
		///   Adds 'Literal' elements for non-style literal values.
		/// </summary>
		private void AddLiterals()
		{
			foreach (var element in Root.DescendantsAndSelf().Where(e => !e.Name.LocalName.StartsWith("Setter.")))
			{
				if (element.Elements().Any() || String.IsNullOrWhiteSpace(element.Value))
					continue;

				var type = GetPropertyType(element.Name.LocalName);
				ReplaceWithLiteral(element, type);
			}
		}

		/// <summary>
		///   Replaces the literal value of the given element with a 'Literal' element of the given type.
		/// </summary>
		/// <param name="element">The element whose literal value should be replaced.</param>
		/// <param name="type">The type of the literal value.</param>
		private static void ReplaceWithLiteral(XElement element, Type type)
		{
			var oldValue = element.Value;
			element.SetValue(String.Empty);
			element.Add(new XElement(DefaultNamespace + "Literal", new XAttribute("Type", type.AssemblyQualifiedName),
									 new XAttribute("Value", oldValue)));
		}

		/// <summary>
		///   Gets the type of the dependency property.
		/// </summary>
		/// <param name="dependencyProperty">The dependency property whose type should be determined.</param>
		private Type GetPropertyType(string dependencyProperty)
		{
			var split = dependencyProperty.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			var type = GetClrType(split[0]);

			var property = type.GetProperty(split[1], BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			if (property == null)
				Log.Die("Property '{0}' could not be found.", dependencyProperty);

			return property.PropertyType;
		}

		/// <summary>
		///   Adds the target type defined on a style to its setters and removes the target type from the file.
		/// </summary>
		private void PushDownStyleTargetType()
		{
			foreach (var style in GetNamedElements(Root, "Style"))
			{
				var targetType = GetNamedElement(style, "Style.TargetType");
				if (targetType == null)
					continue;

				foreach (var setter in GetNamedElements(style, "Setter"))
				{
					var property = GetNamedElement(setter, "Setter.Property");
					if (!property.Value.Contains("."))
						property.SetValue(String.Format("{0}.{1}", targetType.Value, property.Value));
				}

				targetType.Remove();
			}
		}

		/// <summary>
		///   Removes all ignorable elements from the file.
		/// </summary>
		private void RemoveIgnorableElements()
		{
			var ignorableAttributes = from element in Root.DescendantsAndSelf()
									  from attribute in element.Attributes()
									  where !attribute.IsNamespaceDeclaration
									  where GetXamlNamespaces(attribute.Name).Any(n => n.Ignored)
									  select attribute;

			var ignorableElements = from element in Root.DescendantsAndSelf()
									where GetXamlNamespaces(element.Name).Any(n => n.Ignored)
									select element;

			foreach (var ignorable in ignorableAttributes)
				ignorable.Remove();

			foreach (var ignorable in ignorableElements)
				ignorable.Remove();
		}

		/// <summary>
		///   Recursively removes all attributes from the file and replaces them with element syntax.
		/// </summary>
		/// <param name="element">The element whose attributes should be replaced.</param>
		private static void ReplaceAttributesWithElements(XElement element)
		{
			var attributes = element.Attributes().ToArray();
			foreach (var attribute in attributes)
			{
				attribute.Remove();
				XName name;
				if (attribute.Name.LocalName.Contains("."))
					name = attribute.Name;
				else
					name = element.Name + "." + attribute.Name.LocalName;

				element.Add(new XElement(name, attribute.Value));
			}

			foreach (var subElement in element.Elements())
				ReplaceAttributesWithElements(subElement);
		}

		/// <summary>
		///   Makes the implicit content property elements explicit.
		/// </summary>
		private void AddImplicitContentElements()
		{
			foreach (var instantiation in GetInstantiations(Root))
			{
				var xamlType = GetClrType(instantiation.Name);
				var contentProperty = xamlType.GetCustomAttribute<ContentPropertyAttribute>();

				if (contentProperty == null)
					continue;

				var contentPropertyElements = instantiation.Elements().Where(element => !element.Name.LocalName.Contains(".")).ToArray();
				if (contentPropertyElements.Length != 0)
				{
					foreach (var element in contentPropertyElements)
						element.Remove();

					instantiation.Add(new XElement(instantiation.Name + "." + contentProperty.Name, contentPropertyElements));
					continue;
				}

				if (!instantiation.Elements().Any() && !String.IsNullOrWhiteSpace(instantiation.Value))
				{
					var value = instantiation.Value;
					instantiation.SetValue(String.Empty);
					instantiation.Add(new XElement(instantiation.Name + "." + contentProperty.Name, value));
				}
			}
		}

		/// <summary>
		///   Gets all Xaml object instantiations defined below (and including) the given root element.
		/// </summary>
		/// <param name="root">The root element all object instantiations are returned for.</param>
		private static IEnumerable<XElement> GetInstantiations(XElement root)
		{
			return root.DescendantsAndSelf().Where(e => !e.Name.LocalName.Contains("."));
		}

		/// <summary>
		///   Gets all elements with the given name, starting with and including the given root element.
		/// </summary>
		/// <param name="root">The root element all named elements are returned for.</param>
		/// <param name="name">The name of the elements that should be returned.</param>
		private static IEnumerable<XElement> GetNamedElements(XElement root, string name)
		{
			return root.DescendantsAndSelf().Where(e => e.Name.LocalName == name);
		}

		/// <summary>
		///   Gets the element with the given name directly below the root element. Returns null if no such element exists.
		/// </summary>
		/// <param name="root">The root element the named element is returned for.</param>
		/// <param name="name">The name of the element that should be returned.</param>
		private static XElement GetNamedElement(XElement root, string name)
		{
			return root.DescendantsAndSelf().SingleOrDefault(e => e.Name.LocalName == name);
		}

		/// <summary>
		///   Gets the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="typeName">The name of the type the CLR type should be returned for.</param>
		public Type GetClrType(XName typeName)
		{
			Assert.ArgumentNotNull(typeName);

			foreach (var typeNamespace in GetXamlNamespaces(typeName))
			{
				var fullTypeName = String.Format("{0}.{1}, {2}", typeNamespace.Namespace, typeName.LocalName, typeNamespace.AssemblyName);
				var type = Type.GetType(fullTypeName, false);

				if (type != null)
					return type;
			}

			Log.Die("Unable to find CLR type for Xaml name '{0}'.", typeName);
			return null;
		}

		/// <summary>
		///   Gets the Xaml namespaces for the Xaml name.
		/// </summary>
		/// <param name="xamlName">The Xaml name the Xaml namespaces should be returned for.</param>
		private IEnumerable<XamlNamespace> GetXamlNamespaces(XName xamlName)
		{
			var xamlNamespace = xamlName.NamespaceName;
			if (xamlNamespace == String.Empty)
				xamlNamespace = DefaultNamespace.NamespaceName;

			if (!_namespaceMap.ContainsKey(xamlNamespace))
				Log.Die("Unknown Xaml namespace '{0}'.", xamlNamespace);

			return _namespaceMap[xamlNamespace];
		}

		/// <summary>
		///   Removes all namespace declarations from the root element.
		/// </summary>
		private void RemoveNamespaceDeclarations()
		{
			var namespaceDeclarations = Root.Attributes().Where(a => a.IsNamespaceDeclaration).ToArray();
			foreach (var namespaceDeclaration in namespaceDeclarations)
				namespaceDeclaration.Remove();
		}

		/// <summary>
		///   Builds the namespace map for the Xaml root object.
		/// </summary>
		private void BuildNamespaceMap()
		{
			// Add the default namespaces
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml", new[]
			{
				new XamlNamespace("Pegasus.Framework.UserInterface", typeof(UIElement).Assembly.FullName)
			});

			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml/presentation", new[]
			{
				new XamlNamespace("Pegasus.Framework.UserInterface", typeof(UIElement).Assembly.FullName),
				new XamlNamespace("Pegasus.Framework.UserInterface.Controls", typeof(UIElement).Assembly.FullName)
			});

			// Ignored namespaces
			_namespaceMap.Add("http://schemas.openxmlformats.org/markup-compatibility/2006", new[] { new XamlNamespace() });
			_namespaceMap.Add("http://schemas.microsoft.com/expression/blend/2008", new[] { new XamlNamespace() });

			// Add the CLR namespaces defined in the Xaml file
			const string clrNamespace = "clr-namespace";
			foreach (var attribute in Root.Attributes().Where(a => a.IsNamespaceDeclaration && a.Value.Trim().StartsWith(clrNamespace)))
			{
				var colon = attribute.Value.IndexOf(":", StringComparison.Ordinal);
				var semicolon = attribute.Value.IndexOf(";", StringComparison.Ordinal);
				var equals = attribute.Value.IndexOf("=", StringComparison.Ordinal);

				var importedNamespace = semicolon == -1
					? attribute.Value.Substring(colon + 1)
					: attribute.Value.Substring(colon + 1, semicolon - colon - 1);
				var assemblyName = semicolon == -1
					? GetType().Assembly.FullName
					: attribute.Value.Substring(equals + 1);
				var assembly = Assembly.Load(assemblyName);

				_namespaceMap.Add(attribute.Value, new[] { new XamlNamespace(importedNamespace.Trim(), assembly.FullName) });
			}
		}
	}
}