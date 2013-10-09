namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
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
			ReplaceAttributesWithElements(Root);

			ReplaceListSyntax(Root);
			ReplaceDictionarySyntax(Root);
			AddImplicitStyleKeys();
			MoveUpDictionaryKey();
			MoveUpExplicitNames();

			RewriteTemplateBindings();
			RewriteDynamicResourceBindings();
			RewriteStaticResourceBindings();
			RewriteDataBindings();

			PushDownControlTemplateTargetType();
			PushDownStyleTargetType();

			MakeObjectInstantiationsExplicit(Root);
			AssignNames();
			ResolveTypes();
		}

		/// <summary>
		/// Recursively makes all object instantiations explicit.
		/// </summary>
		private void MakeObjectInstantiationsExplicit(XElement element)
		{
			foreach (var child in element.Elements())
				MakeObjectInstantiationsExplicit(child);

			if (element.Name.LocalName.Contains("."))
				return;

			var newElement = new XElement(DefaultNamespace + "Create", new XAttribute("Type", element.Name.LocalName), element.Attributes(), element.Elements());
			if (element.Parent != null)
				element.ReplaceWith(newElement);
			else
				Root = newElement;
		}

		/// <summary>
		/// Resolves the full names of the types of all instantiated objects. 
		/// </summary>
		private void ResolveTypes()
		{
			foreach (var instantiation in Root.DescendantsAndSelf().Where(e => e.Name.LocalName == "Create"))
			{
				var typeAttribute = instantiation.Attribute("Type");
				Type type;
				if (!TryGetClrType(typeAttribute.Value, out type))
					continue;

				typeAttribute.SetValue(type.FullName);
			}
		}

		/// <summary>
		/// Converts explicit name elements to name attributes and registers the used name.
		/// </summary>
		private void MoveUpExplicitNames()
		{
			foreach (var element in Root.Descendants().Where(e => e.Name.LocalName.EndsWith(".Name")).ToArray())
			{
				_usedNames.Add(element.Value);
				element.Parent.Add(new XAttribute("Name", element.Value));
				element.Remove();
			}
		}

		/// <summary>
		/// Assigns names to all unnamed objects.
		/// </summary>
		private void AssignNames()
		{
			foreach (var element in Root.Descendants().Where(e => e.Name.LocalName == "Create"))
			{
				if (element.Attribute("Name") != null)
					continue;

				var typeName = element.Attribute("Type").Value;
				var name = GenerateUniqueName(typeName);
				element.Add(new XAttribute("Name", name));
			}

			// The root object must be called 'this'
			Root.Add(new XAttribute("Name", "this"));
		}

		/// <summary>
		///   Generates a file-wide unique name for the given CLR type.
		/// </summary>
		/// <param name="clrType">The CLR type the name should be generated for.</param>
		private string GenerateUniqueName(string clrType)
		{
			Assert.ArgumentNotNull(clrType);

			var name = Char.ToLower(clrType[0]) + clrType.Substring(1);

			int count;
			if (!_instancesCount.TryGetValue(name, out count))
				count = 0;

			string uniqueName;
			do
			{
				++count;
				_instancesCount[name] = count;

				uniqueName = name + count;
			} while (_usedNames.Contains(uniqueName));

			return uniqueName;
		}

		/// <summary>
		///   Makes implicit style keys explicit.
		/// </summary>
		private void AddImplicitStyleKeys()
		{
			foreach (var element in Root.DescendantsAndSelf().Where(e => e.Name.LocalName.Contains("..Add")))
			{
				var value = element.Elements(DefaultNamespace + "Style").SingleOrDefault();
				if (value == null)
					return;

				var key = value.Elements(value.Name + ".Key").SingleOrDefault();
				if (key != null)
					continue;

				var property = value.Elements(DefaultNamespace + "Style.TargetType").SingleOrDefault();
				if (property == null)
					continue;

				var type = GetClrType(property.Value);
				element.Add(new XAttribute("Key", String.Format("typeof({0})", type.FullName)));
			}
		}

		/// <summary>
		///   Moves the dictionary key from the value element of the dictionary to the dictionary's Add element.
		/// </summary>
		private void MoveUpDictionaryKey()
		{
			foreach (var element in Root.DescendantsAndSelf().Where(e => e.Name.LocalName.Contains("..Add")))
			{
				var value = element.Elements().Single();
				var key = value.Elements(value.Name + ".Key").SingleOrDefault();

				if (key == null)
					continue;

				key.Remove();
				element.Add(new XAttribute("Key", String.Format("\"{0}\"", key.Value)));
			}
		}

		/// <summary>
		///   Recursively replaces Xaml's special list syntax with explicit calls to the list's Add method.
		/// </summary>
		private void ReplaceListSyntax(XElement element)
		{
			foreach (var child in element.Elements())
				ReplaceListSyntax(child);

			if (element.Name.LocalName.Contains("."))
			{
				Type propertyType;
				if (TryGetPropertyType(element.Name.LocalName, out propertyType) && typeof(IList).IsAssignableFrom(propertyType))
					element.ReplaceWith(element.Elements().Select(e => new XElement(element.Name + "..Add", e)));
			}
		}

		/// <summary>
		///   Recursively replaces Xaml's special dictionary syntax with explicit calls to the list's Add method.
		/// </summary>
		private void ReplaceDictionarySyntax(XElement element)
		{
			foreach (var child in element.Elements())
				ReplaceDictionarySyntax(child);

			if (element.Name.LocalName.Contains("."))
			{
				Type propertyType;
				if (TryGetPropertyType(element.Name.LocalName, out propertyType) && typeof(ResourceDictionary).IsAssignableFrom(propertyType))
					element.ReplaceWith(element.Elements().Select(e => new XElement(element.Name + "...Add", e)));
			}
		}

		/// <summary>
		///   Adds the target type defined on a control template to its template bindings and removes the target type from the
		///   file.
		/// </summary>
		private void PushDownControlTemplateTargetType()
		{
			foreach (var controlTemplate in GetNamedElements(Root, "ControlTemplate"))
			{
				var targetType = GetNamedElement(controlTemplate, "ControlTemplate.TargetType");
				if (targetType == null)
					continue;

				foreach (var templateBinding in GetNamedElements(controlTemplate, "TemplateBinding"))
				{
					var property = templateBinding.Attribute("SourceProperty");
					if (!property.Value.Contains("."))
						property.SetValue(String.Format("{0}.{1}", targetType.Value, property.Value));
				}

				targetType.Remove();
			}
		}

		/// <summary>
		///   Rewrites the template binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteTemplateBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf().Where(e => !e.Elements().Any() && e.Value.StartsWith("{TemplateBinding")).ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{TemplateBinding ((Property=(?<property>.*))|(?<property>.*))\}");
				var match = regex.Match(binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "TemplateBinding",
												new XAttribute("SourceProperty", match.Groups["property"]),
												new XAttribute("TargetProperty", bindingElement.Name.LocalName)));
			}
		}

		/// <summary>
		///   Rewrites the dynamic resource binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteDynamicResourceBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf().Where(e => !e.Elements().Any() && e.Value.StartsWith("{DynamicResource")).ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{DynamicResource ((Key=(?<key>.*))|(?<key>.*))\}");
				var match = regex.Match(binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "DynamicResourceBinding",
												new XAttribute("ResourceKey", match.Groups["key"]),
												new XAttribute("TargetProperty", bindingElement.Name.LocalName)));
			}
		}

		/// <summary>
		///   Rewrites the static resource binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteStaticResourceBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf().Where(e => !e.Elements().Any() && e.Value.StartsWith("{StaticResource")).ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{StaticResource ((ResourceKey=(?<key>.*))|(?<key>.*))\}");
				var match = regex.Match(binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "StaticResourceBinding",
												new XAttribute("Key", match.Groups["key"]),
												new XAttribute("TargetProperty", bindingElement.Name.LocalName)));
			}
		}

		/// <summary>
		///   Rewrites the data binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteDataBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf().Where(e => !e.Elements().Any() && e.Value.StartsWith("{Binding")).ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{Binding ((Path=(?<path>.*))|(?<path>.*))\}");
				var match = regex.Match(binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "DataBinding",
												new XAttribute("Path", match.Groups["path"]),
												new XAttribute("TargetProperty", bindingElement.Name.LocalName)));
			}
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
		///   Tries to get the type of the dependency property.
		/// </summary>
		/// <param name="dependencyProperty">The dependency property whose type should be determined.</param>
		/// <param name="propertyType">Returns the type of the property.</param>
		private bool TryGetPropertyType(string dependencyProperty, out Type propertyType)
		{
			propertyType = null;
			var split = dependencyProperty.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

			Type type;
			if (!TryGetClrType(split[0], out type))
				return false;

			var property = type.GetProperty(split[1], BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (property == null)
				return false;

			propertyType = property.PropertyType;
			return true;
		}

		/// <summary>
		///   Gets the type of the dependency property.
		/// </summary>
		/// <param name="dependencyProperty">The dependency property whose type should be determined.</param>
		private Type GetPropertyType(string dependencyProperty)
		{
			Type propertyType;
			if (!TryGetPropertyType(dependencyProperty, out propertyType))
				Log.Die("Property '{0}' could not be found.", dependencyProperty);

			return propertyType;
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
			foreach (var attribute in attributes.Where(a => !a.IsNamespaceDeclaration).ToArray().Reverse())
			{
				attribute.Remove();
				XName name;
				if (attribute.Name.LocalName.Contains("."))
					name = element.Name.Namespace + attribute.Name.LocalName;
				else
					name = element.Name + "." + attribute.Name.LocalName;

				element.AddFirst(new XElement(name, attribute.Value));
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
		///   Tries to get the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="typeName">The name of the type the CLR type should be returned for.</param>
		/// <param name="type">Returns the CLR type.</param>
		private bool TryGetClrType(XName typeName, out Type type)
		{
			foreach (var typeNamespace in GetXamlNamespaces(typeName))
			{
				var fullTypeName = String.Format("{0}.{1}, {2}", typeNamespace.Namespace, typeName.LocalName, typeNamespace.AssemblyName);
				type = Type.GetType(fullTypeName, false);

				if (type != null)
					return true;
			}

			type = null;
			return false;
		}

		/// <summary>
		///   Gets the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="typeName">The name of the type the CLR type should be returned for.</param>
		private Type GetClrType(XName typeName)
		{
			Type type;
			if (!TryGetClrType(typeName, out type))
				Log.Die("Unable to find CLR type for Xaml name '{0}'.", typeName);

			return type;
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