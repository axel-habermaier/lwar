using System;

namespace Pegasus.AssetsCompiler.Xaml
{
	using System.Xml.Linq;
	using CodeGeneration;

	/// <summary>
	///   Serializes a preprocessed Xaml file to equivalent C# code.
	/// </summary>
	internal class XamlToCSharpSerializer
	{
		/// <summary>
		///   The code writer that is used to write the generated C# code.
		/// </summary>
		private readonly CodeWriter _writer = new CodeWriter();

		/// <summary>
		///   The root of the preprocessed Xaml file that is serialized.
		/// </summary>
		private XElement _xamlRoot;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that should be serialized.</param>
		/// <param name="namespaceName">The namespace of the generated class.</param>
		/// <param name="className">The name of the generated class.</param>
		public XamlToCSharpSerializer(XamlFile xamlFile, string namespaceName, string className)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNullOrWhitespace(namespaceName);
			Assert.ArgumentNotNullOrWhitespace(className);

			_xamlRoot = xamlFile.Root;

			// Generated the imports for the default namespaces
			_writer.WriterHeader("//");
			_writer.AppendLine("using System;");
			_writer.Newline();

			_writer.AppendLine("namespace {0}.{1}", Configuration.AssetsProject.RootNamespace, namespaceName);
			_writer.AppendBlockStatement(() =>
			{
				_writer.AppendLine("public class {0} : {1}", className, _xamlRoot.Name.LocalName);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("public {0}()", className);
					_writer.AppendBlockStatement(() => GenerateCode(_xamlRoot));
				});
			});
		}

		/// <summary>
		///   Generates the code for all children of the given element.
		/// </summary>
		/// <param name="element">The element the code should be generated for.</param>
		private void GenerateCode(XElement element)
		{
			foreach (var child in element.Elements())
			{

			}
		}

		/// <summary>
		///   Returns the generated code.
		/// </summary>
		public string GetGeneratedCode()
		{
			return _writer.ToString();
		}
	}
}