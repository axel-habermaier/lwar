namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
	using Platform;
	using Platform.Logging;
	using Scripting.Parsing;

	/// <summary>
	///     Applies a number of transformations to a Xaml file to facilitate the cross-compilation of the Xaml code to C#.
	/// </summary>
	internal class XamlFile
	{
		/// <summary>
		///     The default Xaml namespace.
		/// </summary>
		public static readonly XNamespace DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		/// <summary>
		///     The Xaml markup namespace.
		/// </summary>
		public static readonly XNamespace MarkupNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

		/// <summary>
		///     Maps a class name to the number of instances created of the class.
		/// </summary>
		private readonly Dictionary<string, int> _instancesCount = new Dictionary<string, int>();

		/// <summary>
		///     Maps an Xml namespace to the corresponding Xaml namespaces.
		/// </summary>
		private readonly Dictionary<string, XamlNamespace[]> _namespaceMap = new Dictionary<string, XamlNamespace[]>();

		/// <summary>
		///     Maps an Xml namespace shortcut to the corresponding Xml namespace.
		/// </summary>
		private readonly Dictionary<string, string> _namespaceShortcutMap = new Dictionary<string, string>();

		/// <summary>
		///     Provides type information about types referenced by the Xaml file.
		/// </summary>
		private readonly XamlTypeInfoProvider _typeInfoProvider;

		/// <summary>
		///     The object names that are used by the Xaml file.
		/// </summary>
		private readonly HashSet<string> _usedNames = new HashSet<string>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The file name of the Xaml file.</param>
		/// <param name="typeInfoProvider">Provides type information about types referenced by the Xaml file.</param>
		public XamlFile(string fileName, XamlTypeInfoProvider typeInfoProvider)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			Assert.ArgumentNotNull(typeInfoProvider);

			_typeInfoProvider = typeInfoProvider;
			Root = XElement.Parse(File.ReadAllText(fileName));

			BuildNamespaceMap();
			Transform();
		}

		/// <summary>
		///     Gets the namespaces imported by the Xaml file.
		/// </summary>
		public IEnumerable<string> Namespaces
		{
			get
			{
				return (from mappedNamespaces in _namespaceMap.Values
						from xamlNamespace in mappedNamespaces
						where !xamlNamespace.Ignored
						orderby xamlNamespace.Namespace
						select xamlNamespace.Namespace).Distinct();
			}
		}

		/// <summary>
		///     The root object of the Xaml file.
		/// </summary>
		public XElement Root { get; private set; }

		/// <summary>
		///     Transforms the Xaml root object.
		/// </summary>
		private void Transform()
		{
			var baseType = GetClrType(Root.Name);
			if (baseType.FullName == "Pegasus.Framework.UserInterface.ResourceDictionary")
			{
				Root = null;
				return;
			}

			InlineMergedResourceDictionaries();
			RemoveResourceDictionaryCreations();

			TrimElementLiterals();
			RemoveIgnorableElements();
			AddImplicitContentElements();
			ReplaceAttributesWithElements(Root);

			RemoveClassAttribute();
			ReplaceListSyntax(Root);
			ReplaceDictionarySyntax(Root);
			AddImplicitStyleKeys();
			MoveUpDictionaryKey();
			MoveUpExplicitNames();
			RemoveDuplicatedResourceKeys();

			RewriteTemplateBindings();
			RewriteDataBindings();
			RewriteDynamicResourceBindings();
			RewriteStaticResourceBindings();

			PushDownControlTemplateTargetType();
			PushDownStyleTargetType();

			MakeObjectInstantiationsExplicit(Root);
			MakeDictionaryAddCallsExplicit(Root);
			MakeListAddCallsExplicit(Root);

			AddSetterConstructorParameters();

			MakePropertySetCallsExplicit(Root);
			AssignNames();
			ResolveTypes();

			RewriteControlTemplateInstantiations(Root);
			RewriteDataTemplateInstantiations(Root);
		}

		/// <summary>
		///     Removes the code-behind class attribute from the top-level element.
		/// </summary>
		private void RemoveClassAttribute()
		{
			foreach (var element in Root.Elements())
			{
				if (element.Name.LocalName.EndsWith(".Class"))
				{
					element.Remove();
					break;
				}
			}
		}

		/// <summary>
		///     Removes duplicated resources from resource dictionaries. The last resource is kept, in accordance with the WPF
		///     resource lookup specification.
		/// </summary>
		private void RemoveDuplicatedResourceKeys()
		{
			foreach (var resourceDictionary in Root.DescendantsAndSelf().Where(e => e.Name.LocalName.EndsWith("...Add")).GroupBy(e => e.Name))
			{
				var resources = new Dictionary<string, XElement>();
				foreach (var resource in resourceDictionary)
				{
					var key = resource.Attribute("Key").Value;

					if (resources.ContainsKey(key))
						resources[key].Remove();

					resources[key] = resource;
				}
			}
		}

		/// <summary>
		///     Inlines merged dictionary files.
		/// </summary>
		private void InlineMergedResourceDictionaries()
		{
			foreach (var merge in GetNamedElements(Root, "ResourceDictionary.MergedDictionaries").ToArray())
			{
				foreach (var dictionary in merge.Elements(DefaultNamespace + "ResourceDictionary").ToArray())
				{
					var source = dictionary.Attribute("Source").Value;
					var content = XElement.Parse(File.ReadAllText(Path.Combine(Configuration.SourceDirectory, source)));

					dictionary.ReplaceWith(content.Elements());
				}

				merge.ReplaceWith(merge.Elements());
			}
		}

		/// <summary>
		///     Removes all explicit creations of resource dictionaries.
		/// </summary>
		private void RemoveResourceDictionaryCreations()
		{
			foreach (var dictionary in GetNamedElements(Root, "ResourceDictionary").ToArray())
				dictionary.ReplaceWith(dictionary.Elements());
		}

		/// <summary>
		///     Recursively rewrites the instantiation of all control templates to delegate instantiations.
		/// </summary>
		private void RewriteControlTemplateInstantiations(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				RewriteControlTemplateInstantiations(child);

			if (element.Name.LocalName != "Create" || element.Attribute("Type").Value != "Pegasus.Framework.UserInterface.Controls.ControlTemplate")
				return;

			element.ReplaceWith(new XElement(DefaultNamespace + "Delegate",
				new XElement(DefaultNamespace + "Parameter", new XAttribute("Name", "templatedControl")),
				new XElement(DefaultNamespace + "Return", element.Elements().Single().Attribute("Name").Value),
				element.Attributes(), element.Elements()));
		}

		/// <summary>
		///     Recursively rewrites the instantiation of all data templates to delegate instantiations.
		/// </summary>
		private void RewriteDataTemplateInstantiations(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				RewriteDataTemplateInstantiations(child);

			if (element.Name.LocalName != "Create" || element.Attribute("Type").Value != "Pegasus.Framework.UserInterface.Controls.DataTemplate")
				return;

			element.ReplaceWith(new XElement(DefaultNamespace + "Delegate",
				new XElement(DefaultNamespace + "Return", element.Elements().Single().Attribute("Name").Value),
				element.Attributes(), element.Elements()));
		}

		/// <summary>
		///     Adds the constructor parameters for setter instantiations.
		/// </summary>
		private void AddSetterConstructorParameters()
		{
			foreach (var setter in Root.DescendantsAndSelf()
									   .Where(e => e.Name.LocalName == "Create" && e.Attribute("Type").Value == "Pegasus.Framework.UserInterface.Setter"))
			{
				var property = setter.Element(DefaultNamespace + "Setter.Property");
				var value = setter.Element(DefaultNamespace + "Setter.Value");

				property.Remove();
				value.Remove();

				XamlProperty xamlProperty;
				if (!TryGetPropertyType(property.Value, out xamlProperty))
					Log.Die("Unable to get type of property '{0}'.", property.Value);

				var content = value.HasElements ? (object)value.Elements() : (object)value.Value;

				setter.Add(new XElement(DefaultNamespace + "TypeParameter", xamlProperty.Type.FullName));
				setter.Add(new XElement(DefaultNamespace + "Parameter",
					new XAttribute("Type", "Pegasus.AssetCompiler.Xaml.XamlLiteral"), property.Value + "Property"));
				setter.Add(new XElement(DefaultNamespace + "Parameter",
					new XAttribute("Type", xamlProperty.Type.FullName), content));
			}
		}

		/// <summary>
		///     Recursively makes all property set operations explicit.
		/// </summary>
		private void MakePropertySetCallsExplicit(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				MakePropertySetCallsExplicit(child);

			if (!element.Name.LocalName.Contains("."))
				return;

			var split = element.Name.LocalName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			Assert.That(split.Length == 2, "Expected property element to be of form 'TargetType.PropertyName'.");

			var parentType = element.Parent.Attribute("Type").Value;
			var typeName = parentType.Substring(parentType.LastIndexOf(".", StringComparison.Ordinal) + 1);
			var propertyName = split[1];

			var isAttached = typeName != split[0];
			var content = element.HasElements ? (object)element.Elements() : (object)element.Value;

			if (isAttached)
				parentType = GetClrType(split[0]).FullName;

			IXamlType xamlType;
			if (!_typeInfoProvider.TryFind(parentType, out xamlType))
				Log.Die("Unable to find class for type '{0}'.", parentType);

			var classType = (XamlClass)xamlType;
			XamlProperty xamlProperty;
			if (!classType.TryFind(split[1], out xamlProperty))
				Log.Die("Unable to find property '{0}' in '{1}'.", split[1], classType.FullName);

			XElement newElement;
			if (isAttached)
			{
				newElement = new XElement(DefaultNamespace + "SetAttached",
					new XAttribute("Type", classType.FullName),
					new XAttribute("Property", propertyName),
					new XElement(DefaultNamespace + "Value",
						new XAttribute("Type", xamlProperty.Type.FullName), content));
			}
			else
			{
				var propertyType = xamlProperty.Type.FullName;
				newElement = new XElement(DefaultNamespace + "Set", new XAttribute("Property", propertyName),
					new XElement(DefaultNamespace + "Value", new XAttribute("Type", propertyType), content));
			}

			if (element.Parent != null)
				element.ReplaceWith(newElement);
			else
				Root = newElement;
		}

		/// <summary>
		///     Recursively makes all Dictionary.Add method calls explicit.
		/// </summary>
		private void MakeDictionaryAddCallsExplicit(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				MakeDictionaryAddCallsExplicit(child);

			if (!element.Name.LocalName.EndsWith("...Add"))
				return;

			var targetProperty = element.Name.LocalName.Replace("...Add", String.Empty);
			targetProperty = targetProperty.Substring(targetProperty.LastIndexOf(".", StringComparison.Ordinal) + 1);

			var key = element.Attribute("Key").Value;
			string keyType;
			if (key.StartsWith("typeof"))
			{
				var length = "typeof(".Length;
				key = key.Substring(length, key.Length - length - 1);
				keyType = "System.Type";
			}
			else
			{
				key = key.Substring(1, key.Length - 2);
				keyType = "string";
			}

			var newElement = new XElement(DefaultNamespace + "Invoke", new XAttribute("Method", "Add"),
				new XAttribute("TargetProperty", targetProperty),
				new XElement(DefaultNamespace + "Parameter", key, new XAttribute("Type", keyType)),
				new XElement(DefaultNamespace + "Parameter",
					new XAttribute("Type", typeof(object).AssemblyQualifiedName), element.Elements()));

			if (element.Parent != null)
				element.ReplaceWith(newElement);
			else
				Root = newElement;
		}

		/// <summary>
		///     Recursively makes all List.Add method calls explicit.
		/// </summary>
		private void MakeListAddCallsExplicit(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				MakeListAddCallsExplicit(child);

			if (!element.Name.LocalName.EndsWith("..Add"))
				return;

			var targetProperty = element.Name.LocalName.Replace("..Add", String.Empty);
			targetProperty = targetProperty.Substring(targetProperty.LastIndexOf(".", StringComparison.Ordinal) + 1);

			var newElement = new XElement(DefaultNamespace + "Invoke", new XAttribute("Method", "Add"),
				new XAttribute("TargetProperty", targetProperty),
				new XElement(DefaultNamespace + "Parameter",
					new XAttribute("Type", typeof(object).AssemblyQualifiedName), element.Elements()));

			if (element.Parent != null)
				element.ReplaceWith(newElement);
			else
				Root = newElement;
		}

		/// <summary>
		///     Recursively makes all object instantiations explicit.
		/// </summary>
		private void MakeObjectInstantiationsExplicit(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				MakeObjectInstantiationsExplicit(child);

			if (element.Name.LocalName.Contains(".") || element.Name.LocalName == "Binding")
				return;

			var type = GetClrType(element.Name);
			var newElement = new XElement(DefaultNamespace + "Create",
				new XAttribute("Type", type.FullName),
				element.Attributes(),
				element.Elements());
			if (element.Parent != null)
				element.ReplaceWith(newElement);
			else
				Root = newElement;
		}

		/// <summary>
		///     Resolves the full names of the types of all instantiated objects.
		/// </summary>
		private void ResolveTypes()
		{
			foreach (var instantiation in Root.DescendantsAndSelf().Where(e => e.Name.LocalName == "Create" || e.Name.LocalName == "Delegate"))
			{
				var typeAttribute = instantiation.Attribute("Type");
				IXamlType type;
				if (!TryGetClrType(typeAttribute.Value, out type))
					continue;

				typeAttribute.SetValue(type.FullName);
			}
		}

		/// <summary>
		///     Converts explicit name elements to name attributes and registers the used name.
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
		///     Assigns names to all unnamed objects and assigns appropriate field names to named objects.
		/// </summary>
		private void AssignNames()
		{
			foreach (var element in Root.Descendants().Where(e => e.Name.LocalName == "Create" || e.Name.LocalName == "Delegate"))
			{
				element.Add(new XAttribute("GenerateMember", element.Attribute("Name") != null));
				var nameAttribute = element.Attribute("Name");

				if (nameAttribute != null)
				{
					var name = nameAttribute.Value;
					if (!name.StartsWith("_"))
					{
						nameAttribute.Remove();
						nameAttribute = new XAttribute("Name", "_" + Char.ToLower(name[0]) + name.Substring(1));
						element.Add(nameAttribute);
					}
				}
				else
				{
					var typeName = element.Attribute("Type").Value;
					var name = GenerateUniqueName(typeName);
					element.Add(new XAttribute("Name", name));
				}
			}

			// The root object must be called 'this'
			var rootName = Root.Attribute("Name");
			if (rootName != null)
				rootName.Remove();

			Root.Add(new XAttribute("Name", "this"));
		}

		/// <summary>
		///     Generates a file-wide unique name for the given CLR type.
		/// </summary>
		/// <param name="clrType">The CLR type the name should be generated for.</param>
		private string GenerateUniqueName(string clrType)
		{
			Assert.ArgumentNotNull(clrType);

			var lastDot = clrType.LastIndexOf(".", StringComparison.Ordinal);
			clrType = clrType.Substring(lastDot + 1);
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
		///     Makes implicit style keys explicit.
		/// </summary>
		private void AddImplicitStyleKeys()
		{
			foreach (var element in Root.DescendantsAndSelf().Where(e => e.Name.LocalName.Contains("...Add")).ToArray())
			{
				var value = element.Elements(DefaultNamespace + "Style").SingleOrDefault();
				if (value == null)
					continue;

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
		///     Moves the dictionary key from the value element of the dictionary to the dictionary's Add element.
		/// </summary>
		private void MoveUpDictionaryKey()
		{
			foreach (var element in Root.DescendantsAndSelf().Where(e => e.Name.LocalName.Contains("...Add")))
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
		///     Recursively replaces Xaml's special list syntax with explicit calls to the list's Add method.
		/// </summary>
		private void ReplaceListSyntax(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				ReplaceListSyntax(child);

			if (element.Name.LocalName.Contains("."))
			{
				XamlProperty property;
				if (TryGetPropertyType(element.Name.LocalName, out property) && property.Type.IsList)
					element.ReplaceWith(element.Elements().Select(e => new XElement(element.Name + "..Add", e)));
			}
		}

		/// <summary>
		///     Recursively replaces Xaml's special dictionary syntax with explicit calls to the dictionary's Add method.
		/// </summary>
		private void ReplaceDictionarySyntax(XElement element)
		{
			foreach (var child in element.Elements().ToArray())
				ReplaceDictionarySyntax(child);

			if (element.Name.LocalName.Contains("."))
			{
				XamlProperty property;
				if (TryGetPropertyType(element.Name.LocalName, out property) && property.Type.IsDictionary)
					element.ReplaceWith(element.Elements().Select(e => new XElement(element.Name + "...Add", e)));
			}
		}

		/// <summary>
		///     Adds the target type defined on a control template to its template bindings and removes the target type from the
		///     file.
		/// </summary>
		private void PushDownControlTemplateTargetType()
		{
			foreach (var controlTemplate in GetNamedElements(Root, "ControlTemplate"))
			{
				var targetType = GetNamedElement(controlTemplate, "ControlTemplate.TargetType");
				if (targetType == null)
					continue;

				foreach (var binding in GetNamedElements(controlTemplate, "Binding"))
				{
					if (binding.Attribute("BindingType").Value != "Template")
						continue;

					var property = binding.Attribute("SourceProperty");
					if (!property.Value.Contains("."))
						property.SetValue(String.Format("{0}.{1}", targetType.Value, property.Value));
				}

				targetType.Remove();
			}
		}

		/// <summary>
		///     Rewrites the template binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteTemplateBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf()
											   .Where(e => !e.Elements().Any() && e.Value.StartsWith("{TemplateBinding"))
											   .ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{TemplateBinding ((Property=(?<property>.*))|(?<property>.*))\}");
				var match = regex.Match(binding);
				if (!match.Success)
					Log.Die("Unable to parse template binding '{0}'.", binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "Binding", new XAttribute("BindingType", "Template"),
					new XAttribute("SourceProperty", match.Groups["property"] + "Property"),
					new XAttribute("TargetProperty", bindingElement.Name.LocalName + "Property")));
			}
		}

		/// <summary>
		///     Rewrites the dynamic resource binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteDynamicResourceBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf()
											   .Where(e => !e.Elements().Any() && e.Value.StartsWith("{DynamicResource"))
											   .ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{DynamicResource ((Key=(?<key>.*))|(?<key>.*))\}");
				var match = regex.Match(binding);
				if (!match.Success)
					Log.Die("Unable to parse dynamic resource binding '{0}'.", binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "Binding", new XAttribute("BindingType", "DynamicResource"),
					new XAttribute("ResourceKey", match.Groups["key"]),
					new XAttribute("TargetProperty", bindingElement.Name.LocalName + "Property")));
			}
		}

		/// <summary>
		///     Rewrites the static resource binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteStaticResourceBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf()
											   .Where(e => !e.Elements().Any() && e.Value.StartsWith("{StaticResource"))
											   .ToArray())
			{
				var binding = bindingElement.Value;
				var regex = new Regex(@"\{StaticResource ((ResourceKey=(?<key>.*))|(?<key>.*))\}");
				var match = regex.Match(binding);
				if (!match.Success)
					Log.Die("Unable to parse static resource binding '{0}'.", binding);

				bindingElement.SetValue(String.Empty);
				bindingElement.ReplaceWith(new XElement(DefaultNamespace + "Binding", new XAttribute("BindingType", "StaticResource"),
					new XAttribute("ResourceKey", match.Groups["key"]),
					new XAttribute("TargetProperty", bindingElement.Name.LocalName + "Property")));
			}
		}

		/// <summary>
		///     Rewrites the data binding markup extension syntax to regular Xaml syntax.
		/// </summary>
		private void RewriteDataBindings()
		{
			foreach (var bindingElement in Root.DescendantsAndSelf()
											   .Where(e => !e.Elements().Any() && e.Value.StartsWith("{Binding"))
											   .ToArray())
			{
				var split = bindingElement.Name.LocalName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				Assert.That(split.Length == 2, "Expected property element to be of form 'TargetType.PropertyName'.");

				var binding = bindingElement.Value;
				var parser = new DataBindingParser();

				var dataBinding = parser.Parse(binding);
				if (dataBinding.Status != ReplyStatus.Success)
				{
					using (var text = TextString.Create(dataBinding.Errors.ErrorMessage))
						Log.Die("{0}", text);
				}

				IXamlType xamlType;
				if (!TryGetClrType(bindingElement.Name.Namespace + split[0], out xamlType))
					Log.Die("Unable to find class for type '{0}'.", split[0]);

				var classType = (XamlClass)xamlType;
				XamlEvent xamlEvent;
				if (!classType.TryFind(split[1], out xamlEvent))
				{
					// Property data binding
					var element = new XElement(DefaultNamespace + "Binding", new XAttribute("BindingType", "Data"),
						new XAttribute("Path", dataBinding.Result.Path),
						new XAttribute("TargetProperty", bindingElement.Name.LocalName + "Property"));

					if (dataBinding.Result.Converter != null)
					{
						var converter = dataBinding.Result.Converter;
						split = converter.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
						XName converterType = null;

						if (split.Length == 1)
							converterType = split[0];
						else if (split.Length == 2)
							converterType = String.Format("{{{0}}}{1}", split[0], split[1]);
						else
							Log.Die("Unable to parse converter type '{0}'.", converter);

						element.Add(new XAttribute("Converter", GetClrType(converterType).FullName));
					}

					if (dataBinding.Result.BindingMode != null)
						element.Add(new XAttribute("BindingMode", dataBinding.Result.BindingMode));

					bindingElement.SetValue(String.Empty);
					bindingElement.ReplaceWith(element);
				}
				else
				{
					// Event binding
					var eventElement = new XElement(DefaultNamespace + "Binding", new XAttribute("BindingType", "Event"),
						new XAttribute("Method", dataBinding.Result.Path),
						new XAttribute("TargetEvent", bindingElement.Name.LocalName + "Event"));

					bindingElement.ReplaceWith(eventElement);
				}
			}
		}

		/// <summary>
		///     Trims the literal values of all elements.
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
		///     Tries to get the type of the dependency property.
		/// </summary>
		/// <param name="dependencyProperty">The dependency property whose type should be determined.</param>
		/// <param name="property">Returns the the property.</param>
		private bool TryGetPropertyType(string dependencyProperty, out XamlProperty property)
		{
			property = null;
			var split = dependencyProperty.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

			IXamlType type;
			if (!TryGetClrType(split[0], out type))
				return false;

			var xamlClass = (XamlClass)type;
			return xamlClass.TryFind(split[1], out property);
		}

		/// <summary>
		///     Adds the target type defined on a style to its setters and removes the target type from the file.
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
		///     Removes all ignorable elements from the file.
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

			foreach (var ignorable in ignorableAttributes.ToArray())
				ignorable.Remove();

			foreach (var ignorable in ignorableElements.ToArray())
				ignorable.Remove();
		}

		/// <summary>
		///     Recursively removes all attributes from the file and replaces them with element syntax.
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
					name = attribute.Name;
				else
					name = element.Name + "." + attribute.Name.LocalName;

				element.AddFirst(new XElement(name, attribute.Value));
			}

			foreach (var subElement in element.Elements())
				ReplaceAttributesWithElements(subElement);
		}

		/// <summary>
		///     Makes the implicit content property elements explicit.
		/// </summary>
		private void AddImplicitContentElements()
		{
			foreach (var instantiation in GetInstantiations(Root))
			{
				var xamlType = GetClrType(instantiation.Name) as XamlClass;
				if (xamlType == null || !xamlType.HasContentProperty)
					continue;

				var contentProperty = xamlType.ContentProperty;

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
		///     Gets all Xaml object instantiations defined below (and including) the given root element.
		/// </summary>
		/// <param name="root">The root element all object instantiations are returned for.</param>
		private static IEnumerable<XElement> GetInstantiations(XElement root)
		{
			return root.DescendantsAndSelf().Where(e => !e.Name.LocalName.Contains("."));
		}

		/// <summary>
		///     Gets all elements with the given name, starting with and including the given root element.
		/// </summary>
		/// <param name="root">The root element all named elements are returned for.</param>
		/// <param name="name">The name of the elements that should be returned.</param>
		private static IEnumerable<XElement> GetNamedElements(XElement root, string name)
		{
			return root.DescendantsAndSelf().Where(e => e.Name.LocalName == name);
		}

		/// <summary>
		///     Gets the element with the given name directly below the root element. Returns null if no such element exists.
		/// </summary>
		/// <param name="root">The root element the named element is returned for.</param>
		/// <param name="name">The name of the element that should be returned.</param>
		private static XElement GetNamedElement(XElement root, string name)
		{
			return root.DescendantsAndSelf().SingleOrDefault(e => e.Name.LocalName == name);
		}

		/// <summary>
		///     Tries to get the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="typeName">The name of the type the CLR type should be returned for.</param>
		/// <param name="type">Returns the CLR type.</param>
		private bool TryGetClrType(XName typeName, out IXamlType type)
		{
			foreach (var typeNamespace in GetXamlNamespaces(typeName))
			{
				if (_typeInfoProvider.TryFind(typeNamespace.Namespace, typeName.LocalName, out type))
					return true;
			}

			type = null;
			return false;
		}

		/// <summary>
		///     Gets the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="typeName">The name of the type the CLR type should be returned for.</param>
		private IXamlType GetClrType(XName typeName)
		{
			IXamlType type;
			if (!TryGetClrType(typeName, out type))
				Log.Die("Unable to find CLR type for Xaml name '{0}'.", typeName);

			return type;
		}

		/// <summary>
		///     Gets the Xaml namespaces for the Xaml name.
		/// </summary>
		/// <param name="xamlName">The Xaml name the Xaml namespaces should be returned for.</param>
		private IEnumerable<XamlNamespace> GetXamlNamespaces(XName xamlName)
		{
			var xamlNamespace = xamlName.NamespaceName;
			if (xamlNamespace == String.Empty)
				xamlNamespace = DefaultNamespace.NamespaceName;
			else if (_namespaceShortcutMap.ContainsKey(xamlName.NamespaceName))
				xamlNamespace = _namespaceShortcutMap[xamlName.NamespaceName];

			if (!_namespaceMap.ContainsKey(xamlNamespace))
				Log.Die("Unknown Xaml namespace '{0}'.", xamlNamespace);

			return _namespaceMap[xamlNamespace];
		}

		/// <summary>
		///     Builds the namespace map for the Xaml root object.
		/// </summary>
		private void BuildNamespaceMap()
		{
			// Add the default namespaces
			_namespaceShortcutMap.Add(String.Empty, "http://schemas.microsoft.com/winfx/2006/xaml");
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml", new[]
			{
				new XamlNamespace("Pegasus.Framework.UserInterface")
			});

			_namespaceShortcutMap.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml/presentation", new[]
			{
				new XamlNamespace("Pegasus.Framework"),
				new XamlNamespace("Pegasus.Framework.UserInterface"),
				new XamlNamespace("Pegasus.Framework.UserInterface.Controls")
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

				var importedNamespace = semicolon == -1
					? attribute.Value.Substring(colon + 1)
					: attribute.Value.Substring(colon + 1, semicolon - colon - 1);

				_namespaceShortcutMap.Add(attribute.Name.LocalName, attribute.Value);
				_namespaceMap.Add(attribute.Value, new[] { new XamlNamespace(importedNamespace.Trim()) });
			}
		}
	}
}