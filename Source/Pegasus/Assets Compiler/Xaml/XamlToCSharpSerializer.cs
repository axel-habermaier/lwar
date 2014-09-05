namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using CSharp;
	using Platform.Logging;

	/// <summary>
	///     Serializes a preprocessed Xaml file to equivalent C# code.
	/// </summary>
	internal class XamlToCSharpSerializer
	{
		/// <summary>
		///     Provides type information about types referenced by the Xaml file.
		/// </summary>
		private readonly XamlTypeInfoProvider _typeInfoProvider;

		/// <summary>
		///     The code writer that is used to write the generated C# code.
		/// </summary>
		private readonly CodeWriter _writer = new CodeWriter();

		/// <summary>
		///     The root of the preprocessed Xaml file that is serialized.
		/// </summary>
		private XElement _xamlRoot;

		/// <summary>
		/// </summary>
		/// <param name="typeInfoProvider">Provides type information about types referenced by the Xaml file.</param>
		public XamlToCSharpSerializer(XamlTypeInfoProvider typeInfoProvider)
		{
			Assert.ArgumentNotNull(typeInfoProvider);

			_typeInfoProvider = typeInfoProvider;
			_writer.WriterHeader();
		}

		/// <summary>
		///     Serializes the Xaml file to C# code.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that should be serialized.</param>
		/// <param name="namespaceName">The namespace of the generated class.</param>
		/// <param name="className">The name of the generated class.</param>
		public void SerializeToCSharp(XamlFile xamlFile, string namespaceName, string className)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(namespaceName);
			Assert.ArgumentNotNullOrWhitespace(className);

			_xamlRoot = xamlFile.Root;

			if (String.IsNullOrWhiteSpace(namespaceName))
				_writer.AppendLine("namespace {0}", Configuration.AssetsProject.RootNamespace);
			else
				_writer.AppendLine("namespace {0}.{1}", Configuration.AssetsProject.RootNamespace, namespaceName);

			_writer.AppendBlockStatement(() =>
			{
				_writer.AppendLine("using System;");
				_writer.AppendLine("using Pegasus.Platform.Graphics;");
				foreach (var xamlNamespace in xamlFile.Namespaces)
					_writer.AppendLine("using {0};", xamlNamespace);
				_writer.NewLine();

				var baseType = _xamlRoot.Attribute("Type").Value;
				_writer.AppendLine("partial class {0} : {1}", className, baseType);
				_writer.AppendBlockStatement(() =>
				{
					GenerateMembers();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Initializes a new instance.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public {0}()", className);
					_writer.AppendBlockStatement(() => _writer.AppendLine("LoadContent();"));
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Invoked once the UI element's content has been fully loaded.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("partial void OnLoaded();");
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Loads and initializes the UI element's content.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("private void LoadContent()");
					_writer.AppendBlockStatement(() =>
					{
						GenerateCode(_xamlRoot);
						_writer.AppendLine("OnLoaded();");
					});
				});
			});

			_writer.NewLine();
		}

		/// <summary>
		///     Generates the private member fields for named Xaml elements.
		/// </summary>
		private void GenerateMembers()
		{
			var hasMembers = false;
			foreach (var element in _xamlRoot.Descendants().Where(e => e.Name.LocalName == "Create" || e.Name.LocalName == "Delegate"))
			{
				if (element.Attribute("GenerateMember").Value.ToLower() == "false")
					continue;

				var type = element.Attribute("Type").Value;
				var name = element.Attribute("Name").Value;

				_writer.AppendLine("private {0} {1};", type, name);
				hasMembers = true;
			}

			if (hasMembers)
				_writer.NewLine();
		}

		/// <summary>
		///     Generates the code for all children of the given element.
		/// </summary>
		/// <param name="element">The element the code should be generated for.</param>
		private void GenerateCode(XElement element)
		{
			foreach (var child in element.Elements())
			{
				switch (child.Name.LocalName)
				{
					case "Create":
						InstantiateObject(child);
						break;
					case "Set":
					case "SetAttached":
						var value = GenerateValue(child.Element(XamlFile.DefaultNamespace + "Value"));
						var parentName = child.Parent.Attribute("Name").Value;

						if (child.Name.LocalName == "Set")
							_writer.AppendLine("{0}.{1} = {2};", parentName, child.Attribute("Property").Value, value);
						else
						{
							_writer.AppendLine("{0}.Set{1}({2}, {3});", child.Attribute("Type").Value,
								child.Attribute("Property").Value, parentName, value);
						}
						break;
					case "Invoke":
						InvokeMethod(child);
						break;
					case "Binding":
						CreateBinding(child);
						break;
					case "Delegate":
						InstantiateDelegate(child);
						break;
					case "Return":
					case "Parameter":
					case "TypeParameter":
						break;
					default:
						Log.Die("Unknown element '{0}'.", child.Name.LocalName);
						return;
				}
			}
		}

		/// <summary>
		///     Instantiates a delegate.
		/// </summary>
		/// <param name="element">The element representing the delegate that should be instantiated.</param>
		private void InstantiateDelegate(XElement element)
		{
			var variableName = element.Attribute("Name").Value;
			var delegateType = element.Attribute("Type").Value;
			var parameters = element.Elements(XamlFile.DefaultNamespace + "Parameter").Select(e => e.Attribute("Name").Value);

			if (element.Attribute("GenerateMember").Value.ToLower() == "false")
				_writer.Append("var ");
			_writer.Append("{0} = new {1}(", variableName, delegateType);
			_writer.AppendLine("({0}) =>", String.Join(", ", parameters));
			_writer.IncreaseIndent();
			_writer.AppendLine("{{");
			_writer.IncreaseIndent();

			GenerateCode(element);

			var returnObject = element.Element(XamlFile.DefaultNamespace + "Return");
			if (returnObject != null)
				_writer.AppendLine("return {0};", returnObject.Value);

			_writer.DecreaseIndent();
			_writer.AppendLine("}});");
			_writer.DecreaseIndent();
		}

		/// <summary>
		///     Creates a data, template, or resource binding.
		/// </summary>
		/// <param name="element">The element representing the binding that should be created.</param>
		private void CreateBinding(XElement element)
		{
			var bindingType = element.Attribute("BindingType").Value;
			switch (bindingType)
			{
				case "Data":
				{
					var parentName = element.Parent.Attribute("Name").Value;
					var bindingModeAttribute = element.Attribute("BindingMode");
					var bindingMode = bindingModeAttribute == null ? "Default" : bindingModeAttribute.Value;
					_writer.Append("{0}.CreateDataBinding({1}, BindingMode.{2}, ", parentName, element.Attribute("TargetProperty").Value, bindingMode);

					var path = element.Attribute("Path").Value;
					var properties = path.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					if (properties.Length < 1 || properties.Length > 2)
						Log.Die("Invalid property path '{0}': One or two properties must be accessed.", path);

					_writer.Append("\"{0}\"", properties[0]);
					if (properties.Length > 1)
						_writer.Append(", \"{0}\"", properties[1]);

					var converter = element.Attribute("Converter");
					if (converter != null)
						_writer.Append(", converter: {0}.Instance", converter.Value);

					_writer.AppendLine(");");
					break;
				}
				case "Event":
				{
					var parentName = element.Parent.Attribute("Name").Value;
					_writer.AppendLine("{0}.CreateEventBinding({1}, \"{2}\");",
						parentName, element.Attribute("TargetEvent").Value, element.Attribute("Method").Value);
					break;
				}
				case "Template":
				{
					var parentName = element.Parent.Attribute("Name").Value;
					_writer.AppendLine("{0}.CreateTemplateBinding(templatedControl, {1}, {2});",
						parentName, element.Attribute("SourceProperty").Value, element.Attribute("TargetProperty").Value);
					break;
				}
				case "StaticResource":
				case "DynamicResource":
				{
					var parentName = element.Parent.Attribute("Name").Value;
					_writer.AppendLine("{0}.CreateResourceBinding(\"{1}\", {2});",
						parentName, element.Attribute("ResourceKey").Value, element.Attribute("TargetProperty").Value);
					break;
				}
				default:
					Log.Die("Unknown binding type '{0}'.", bindingType);
					break;
			}
		}

		/// <summary>
		///     Invokes a method.
		/// </summary>
		/// <param name="element">The element representing the method invocation.</param>
		private void InvokeMethod(XElement element)
		{
			var parameters = element.Elements(XamlFile.DefaultNamespace + "Parameter").Select(GenerateValue).ToArray();
			var parent = element.Parent.Attribute("Name").Value;

			_writer.AppendLine("{0}.{3}.{1}({2});", parent, element.Attribute("Method").Value,
				String.Join(", ", parameters), element.Attribute("TargetProperty").Value);
			_writer.NewLine();
		}

		/// <summary>
		///     Instantiates an object.
		/// </summary>
		/// <param name="element">The element representing the object that should be instantiated.</param>
		private void InstantiateObject(XElement element)
		{
			var parameters = element.Elements(XamlFile.DefaultNamespace + "Parameter").Select(GenerateValue).ToArray();

			var type = element.Attribute("Type").Value;
			if (element.Attribute("GenerateMember").Value.ToLower() == "false")
				_writer.Append("var ");
			_writer.Append("{0} = new {1}", element.Attribute("Name").Value, type);

			var typeParameters = element.Elements(XamlFile.DefaultNamespace + "TypeParameter").Select(e => e.Value).ToArray();
			if (typeParameters.Length != 0)
				_writer.Append("<{0}>", String.Join(", ", typeParameters));

			_writer.AppendLine("({0});", String.Join(", ", parameters));
			GenerateCode(element);
		}

		/// <summary>
		///     Generates the code of a value.
		/// </summary>
		/// <param name="element">The element representing the value.</param>
		private string GenerateValue(XElement element)
		{
			if (!element.HasElements)
			{
				var type = element.Attribute("Type").Value;

				IXamlType xamlType;
				_typeInfoProvider.TryFind(type, out xamlType);

				return XamlTypeConverter.Convert(xamlType, element.Value);
			}

			var child = element.Elements().Single();
			Assert.That(child.Name.LocalName == "Create" || child.Name.LocalName == "Delegate", "Expected an object or delegate instantiation.");

			GenerateCode(element);
			return child.Attribute("Name").Value;
		}

		/// <summary>
		///     Gets the generated code.
		/// </summary>
		public string GetGeneratedCode()
		{
			return _writer.ToString();
		}
	}
}