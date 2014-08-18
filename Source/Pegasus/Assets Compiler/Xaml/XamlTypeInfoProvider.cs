namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///     Provides type information about all types that can be referenced in a Xaml file.
	/// </summary>
	internal class XamlTypeInfoProvider
	{
		/// <summary>
		///     The files that provide the type information.
		/// </summary>
		private readonly List<XamlTypeInfoFile> _files = new List<XamlTypeInfoFile>();

		/// <summary>
		///     The primitive Xaml types.
		/// </summary>
		private readonly XamlPrimitiveType[] _primitiveTypes =
		{
			new XamlPrimitiveType("object"),
			new XamlPrimitiveType("bool"),
			new XamlPrimitiveType("char"),
			new XamlPrimitiveType("byte"),
			new XamlPrimitiveType("sbyte"),
			new XamlPrimitiveType("short"),
			new XamlPrimitiveType("ushort"),
			new XamlPrimitiveType("int"),
			new XamlPrimitiveType("uint"),
			new XamlPrimitiveType("long"),
			new XamlPrimitiveType("ulong"),
			new XamlPrimitiveType("single"),
			new XamlPrimitiveType("double"),
			new XamlPrimitiveType("string"),
			new XamlPrimitiveType("System", "Type"),
			new XamlPrimitiveType("Pegasus.AssetCompiler.Xaml", "XamlLiteral")
		};

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="typeInfoFile">The path to the type info file that stores the type information.</param>
		public XamlTypeInfoProvider(string typeInfoFile)
		{
			Assert.ArgumentNotNullOrWhitespace(typeInfoFile);

			LoadRecursive(typeInfoFile);
			EnsureDistinct();
			ResolveTypes();
		}

		/// <summary>
		///     Gets all defined types.
		/// </summary>
		public IEnumerable<IXamlType> AllTypes
		{
			get { return _files.SelectMany(f => f.Types).Cast<IXamlType>().Union(_primitiveTypes); }
		}

		/// <summary>
		///     Recursively loads the given file and all of its includes.
		/// </summary>
		/// <param name="fileName">The file that should be loaded.</param>
		private void LoadRecursive(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			var file = new XamlTypeInfoFile(fileName);
			_files.Add(file);

			foreach (var includedFile in file.IncludedFiles)
				LoadRecursive(Path.Combine(Configuration.SourceDirectory, includedFile));
		}

		/// <summary>
		///     Ensures that all loaded type names are distinct.
		/// </summary>
		private void EnsureDistinct()
		{
			var groups = _files.SelectMany(f => f.Types)
							   .GroupBy(t => t.FullName)
							   .Where(g => g.Count() > 1);

			foreach (var group in groups)
				group.First().File.Report(LogType.Fatal, group.First().Element,
										  "Type information for type '{0}' has been specified more than once.", group.Key);
		}

		/// <summary>
		///     Resolves the base types of all classes as well as all property types.
		/// </summary>
		private void ResolveTypes()
		{
			foreach (var file in _files)
			{
				foreach (var xamlClass in file.Classes)
				{
					if (xamlClass.HasParent)
					{
						IXamlType parent;
						if (!TryFind(xamlClass.ParentName, out parent))
							xamlClass.File.Report(LogType.Fatal, xamlClass.Element, "Unknown parent type.");
						else
						{
							var parentClass = parent as XamlClass;
							if (parentClass == null)
								xamlClass.File.Report(LogType.Fatal, xamlClass.Element, "Invalid parent type.");

							xamlClass.Parent = parentClass;
						}
					}

					foreach (var property in xamlClass.Properties)
					{
						IXamlType type;
						if (!TryFind(property.TypeName, out type))
							xamlClass.File.Report(LogType.Fatal, property.Element, "Unknown property type.");
						else
							property.Type = type;
					}
				}
			}
		}

		/// <summary>
		///     Tries to find the type information for the type with the given full name.
		/// </summary>
		/// <param name="fullName">The full name of the type.</param>
		/// <param name="type">Returns the type if one has been found.</param>
		public bool TryFind(string fullName, out IXamlType type)
		{
			type = AllTypes.SingleOrDefault(t => t.FullName == fullName);
			return type != null;
		}

		/// <summary>
		///     Tries to find the type information for the type with the given name.
		/// </summary>
		/// <param name="namespaceName">The namespace the type is defined in.</param>
		/// <param name="typeName">The name of the type.</param>
		/// <param name="type">Returns the type if one has been found.</param>
		public bool TryFind(string namespaceName, string typeName, out IXamlType type)
		{
			type = AllTypes.SingleOrDefault(t => t.Namespace == namespaceName && t.Name == typeName);
			return type != null;
		}
	}
}