using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using Platform.Logging;

	/// <summary>
	///   Represents a Xaml file.
	/// </summary>
	internal class XamlFile
	{
		/// <summary>
		///   The default Xaml namespace.
		/// </summary>
		public static readonly XNamespace DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		/// <summary>
		///   Maps an Xml namespace to the corresponding Xaml namespaces.
		/// </summary>
		private readonly Dictionary<string, XamlNamespace[]> _namespaceMap = new Dictionary<string, XamlNamespace[]>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The file name of the Xaml file.</param>
		public XamlFile(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			var rootElement = XElement.Parse(File.ReadAllText(fileName), LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
			BuildNamespaceMap(rootElement);

			RootObject = new XamlObject(this, rootElement);
		}

		/// <summary>
		///   Gets the root Xaml object of the Xaml file.
		/// </summary>
		public XamlObject RootObject { get; private set; }

		/// <summary>
		///   Builds the namespace map.
		/// </summary>
		private void BuildNamespaceMap(XElement rootElement)
		{
			// Add the default namespaces
			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml", new[]
			{
				new XamlNamespace("Pegasus.Framework.UserInterface", "Pegasus.AssetCompiler.UserInterface.Markup", GetType().Assembly.FullName)
			});

			_namespaceMap.Add("http://schemas.microsoft.com/winfx/2006/xaml/presentation", new[]
			{
				new XamlNamespace("Pegasus.Framework.UserInterface", "Pegasus.AssetsCompiler.UserInterface", GetType().Assembly.FullName),
				new XamlNamespace("Pegasus.Framework.UserInterface.Controls", "Pegasus.AssetsCompiler.UserInterface.Controls",
								  GetType().Assembly.FullName)
			});

			// Ignored namespaces
			_namespaceMap.Add("http://schemas.openxmlformats.org/markup-compatibility/2006", new[] { new XamlNamespace() });
			_namespaceMap.Add("http://schemas.microsoft.com/expression/blend/2008", new[] { new XamlNamespace() });

			// Add the CLR namespaces defined in the Xaml file
			const string clrNamespace = "clr-namespace";
			foreach (var attribute in rootElement.Attributes().Where(a => a.IsNamespaceDeclaration && a.Value.Trim().StartsWith(clrNamespace)))
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

		/// <summary>
		///   Gets a value indicating whether the given Xaml element should be ignored.
		/// </summary>
		/// <param name="xamlElement">The Xaml element that should be checked.</param>
		public bool ShouldBeIgnored(XElement xamlElement)
		{
			Assert.ArgumentNotNull(xamlElement);
			return ShouldBeIgnored(xamlElement.Name);
		}

		/// <summary>
		///   Gets a value indicating whether the given Xaml attribute should be ignored.
		/// </summary>
		/// <param name="xamlAttribute">The Xaml attribute that should be checked.</param>
		public bool ShouldBeIgnored(XAttribute xamlAttribute)
		{
			Assert.ArgumentNotNull(xamlAttribute);
			return ShouldBeIgnored(xamlAttribute.Name);
		}

		/// <summary>
		///   Gets a value indicating whether the given Xaml name belongs to a namespace that should be ignored.
		/// </summary>
		/// <param name="xamlName">The Xaml name that should be checked.</param>
		private bool ShouldBeIgnored(XName xamlName)
		{
			return GetXamlNamespaces(xamlName)[0].Ignored;
		}

		/// <summary>
		///   Gets the CLR type corresponding to the Xaml element.
		/// </summary>
		/// <param name="xamlElement">The Xaml element the CLR type should be returned for.</param>
		public Type GetClrType(XElement xamlElement)
		{
			foreach (var typeNamespace in GetXamlNamespaces(xamlElement.Name))
			{
				var typeName = String.Format("{0}.{1}, {2}", typeNamespace.CompilationNamespace, xamlElement.Name.LocalName, typeNamespace.AssemblyName);
				var type = Type.GetType(typeName, false);

				if (type != null)
					return type;
			}

			Log.Die("Unable to find CLR type for Xaml name '{0}'.", xamlElement.Name);
			return null;
		}

		/// <summary>
		///   Gets the Xaml namespaces for the Xaml name.
		/// </summary>
		/// <param name="xamlName">The Xaml name the Xaml namespaces should be returned for.</param>
		private XamlNamespace[] GetXamlNamespaces(XName xamlName)
		{
			var xamlNamespace = xamlName.NamespaceName;
			if (xamlNamespace == String.Empty)
				xamlNamespace = DefaultNamespace.NamespaceName;

			if (!_namespaceMap.ContainsKey(xamlNamespace))
				Log.Die("Unknown Xaml namespace '{0}'.", xamlNamespace);

			return _namespaceMap[xamlNamespace];
		}
	}
}