using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using Assets;
	using CodeGeneration;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///   Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		/// <summary>
		///   The default Xaml namespace.
		/// </summary>
		private static readonly XNamespace DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		/// <summary>
		///   Maps a Xaml namespace to its corresponding CLR namespaces.
		/// </summary>
		private readonly Dictionary<string, List<string>> _namespaceMap = new Dictionary<string, List<string>>();

		/// <summary>
		///   The code writer that is used to generate the code.
		/// </summary>
		private CodeWriter _writer;

		// TODO: REMOVE
		public override bool Compile(IEnumerable<Asset> assets)
		{
			foreach (var xaml in assets.OfType<XamlAsset>())
				File.Delete(xaml.HashPath);
			return base.Compile(assets);
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(XamlAsset asset, BufferWriter buffer)
		{
			// Load the Xaml file
			var xaml = XElement.Parse(File.ReadAllText(asset.SourcePath), LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
			BuildNamespaceMap(xaml);

			_writer = new CodeWriter();
			WriteUsingDirectives();

			var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
			var namespaceName = asset.RelativePath.Substring(0, asset.RelativePath.Length - asset.FileName.Length - 1);
			GenerateClass(xaml, namespaceName.Replace("/", "."), className);

			buffer.Copy(Encoding.UTF8.GetBytes(_writer.ToString()));
		}

		/// <summary>
		///   Builds the namespace map.
		/// </summary>
		/// <param name="root">The root Xaml element.</param>
		private void BuildNamespaceMap(XElement root)
		{
			// Add the default namespaces
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml", new List<string> { "System.Windows.Markup" });
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml/presentation", new List<string>
			{
				"System.Windows",
				"System.Windows.Automation",
				"System.Windows.Controls",
				"System.Windows.Controls.Primitives",
				"System.Windows.Data",
				"System.Windows.Documents",
				"System.Windows.Forms.Integration",
				"System.Windows.Ink",
				"System.Windows.Input",
				"System.Windows.Media",
				"System.Windows.Media.Animation",
				"System.Windows.Media.Effects",
				"System.Windows.Media.Imaging",
				"System.Windows.Media.Media3D",
				"System.Windows.Media.TextFormatting",
				"System.Windows.Navigation",
				"System.Windows.Shapes"
			});

			// Add the CLR namespaces defined in the Xaml file
			const string clrNamespace = "clr-namespace:";
			foreach (var attribute in root.Attributes().Where(a => a.IsNamespaceDeclaration && a.Value.StartsWith(clrNamespace)))
			{
				var semicolon = attribute.Value.IndexOf(";", StringComparison.Ordinal);
				var importedNamespace = semicolon == -1
					? attribute.Value.Substring(clrNamespace.Length)
					: attribute.Value.Substring(clrNamespace.Length, semicolon - clrNamespace.Length);

				_namespaceMap.Add(attribute.Value, new List<string> { importedNamespace });
			}
		}

		/// <summary>
		///   Generates the required using directives.
		/// </summary>
		private void WriteUsingDirectives()
		{
			_writer.AppendLine("using System;");
			_writer.AppendLine("using Pegasus;");
			_writer.AppendLine("using Pegasus.Framework;");
			_writer.AppendLine("using Pegasus.Framework.UserInterface;");
			_writer.AppendLine("using Pegasus.Framework.UserInterface.Controls;");
			_writer.AppendLine("using Pegasus.Platform.Assets;");

			// Get the namespaces that must be imported
			foreach (var importedNamespace in _namespaceMap.Where(p => p.Key.StartsWith("clr")).SelectMany(p => p.Value).Distinct())
				_writer.AppendLine("using {0};", importedNamespace);

			_writer.Newline();
		}

		/// <summary>
		///   Generates the class that corresponds to the Xaml file.
		/// </summary>
		/// <param name="root">The root Xaml element.</param>
		/// <param name="namespaceName">The namespace of the generated class.</param>
		/// <param name="className">The name of the generated class.</param>
		private void GenerateClass(XElement root, string namespaceName, string className)
		{
			if (root.Name != DefaultNamespace + "UserControl")
				Log.Die("Unsupported Xaml root type '{0}'.", root.Name);

			_writer.AppendLine("namespace {0}", namespaceName);
			_writer.AppendBlockStatement(() =>
			{
				_writer.AppendLine("public class {0} : UserControl", className);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("public {0}(AssetsManager assets, ViewModel viewModel)", className);
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("Assert.ArgumentNotNull(assets);");
						_writer.AppendLine("Assert.ArgumentNotNull(viewModel);");
						_writer.Newline();

						_writer.AppendLine("this.ViewModel = viewModel;");
						HandlePropertyAttributes(root, "this");
						HandlePropertyElements(root, "this");
						HandleContentElement(root, "this");
					});
				});
			});
		}

		/// <summary>
		///   Handles the property attributes of the given element.
		/// </summary>
		private void HandlePropertyAttributes(XElement element, string obj)
		{
			foreach (var property in element.Attributes().Where(a => !a.IsNamespaceDeclaration && a.Name.NamespaceName == String.Empty))
				_writer.AppendLine("{0}.{1} = {2};", obj, property.Name.LocalName, property.Value.Trim());
		}

		/// <summary>
		///   Handles the property elements of the given element.
		/// </summary>
		private void HandlePropertyElements(XElement element, string obj)
		{
			foreach (var property in element.Elements().Where(e => e.Name.LocalName.Contains(".")))
			{
				var propertyName = property.Name.LocalName.Substring(property.Name.LocalName.IndexOf(".", StringComparison.Ordinal) + 1);
				_writer.AppendLine("{0}.{1} = {2};", obj, propertyName, property.Value.Trim());
			}
		}

		/// <summary>
		/// Handles the content element of the given element.
		/// </summary>
		private void HandleContentElement(XElement element, string obj)
		{
			_writer.AppendLine(GetClrType(element).FullName);
		}

		/// <summary>
		/// Gets the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="element">The Xaml element the CLR type should be returned for.</param>
		private Type GetClrType(XElement element)
		{
			foreach (var typeNamespace in _namespaceMap[element.Name.Namespace.NamespaceName])
			{
				var type = Type.GetType(typeNamespace + "." + element.Name.LocalName, false);
				if (type != null)
					return type;
			}

			return null;
		}

		private void WritePropertyValue()
		{
			
		}
	}
}