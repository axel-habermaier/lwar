using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;

	/// <summary>
	///   Represents a registry declaration.
	/// </summary>
	internal class Registry : RegistryElement
	{
		/// <summary>
		///   The declaration of the interface that represents the registry.
		/// </summary>
		private readonly TypeDeclaration _type;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The declaration of the interface that represents the registry.</param>
		public Registry(TypeDeclaration type)
		{
			_type = type;
		}

		/// <summary>
		///   Gets the name of the registry.
		/// </summary>
		public string Name
		{
			get
			{
				if (_type.Name.StartsWith("I"))
					return _type.Name.Substring(1);

				return _type.Name;
			}
		}

		/// <summary>
		///   Gets the namespace the registry is declared in.
		/// </summary>
		public string Namespace
		{
			get
			{
				var resolved = (TypeResolveResult)Resolver.Resolve(_type);
				return resolved.Type.Namespace;
			}
		}

		/// <summary>
		///   Gets the namespaces that are imported by the registry.
		/// </summary>
		public IEnumerable<string> ImportedNamespaces
		{
			get
			{
				var imports = from ancestor in _type.AncestorsAndSelf
							  from import in ancestor.Descendants.OfType<UsingDeclaration>()
							  let importedNamespace = import.Import.ToString()
							  where importedNamespace != "System"
							  orderby importedNamespace
							  select importedNamespace;

				return imports.Distinct();
			}
		}

		/// <summary>
		///   Gets the cvars declared by the registry.
		/// </summary>
		public IEnumerable<Cvar> Cvars
		{
			get { return GetChildElements<Cvar>(); }
		}

		/// <summary>
		///   Gets the commands declared by the registry.
		/// </summary>
		public IEnumerable<Command> Commands
		{
			get { return GetChildElements<Command>(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all cvars
			AddElements(from property in _type.Descendants.OfType<PropertyDeclaration>()
						let attributes = property.Attributes.SelectMany(section => section.Attributes)
						where attributes.Any(attribute => attribute.Type.ToString() == "Cvar") ||
							  attributes.Any(attribute => attribute.Type.ToString() == "CvarAttribute")
						select new Cvar(property));

			// Add all commands
			AddElements(from method in _type.Descendants.OfType<MethodDeclaration>()
						let attributes = method.Attributes.SelectMany(section => section.Attributes)
						where attributes.Any(attribute => attribute.Type.ToString() == "Command") ||
							  attributes.Any(attribute => attribute.Type.ToString() == "CommandAttribute")
						select new Command(method));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check if there are any properties without the cvar attribute
			foreach (var property in from property in _type.Descendants.OfType<PropertyDeclaration>()
									 let attributes = property.Attributes.SelectMany(section => section.Attributes)
									 where attributes.All(attribute => attribute.Type.ToString() != "Cvar") &&
										   attributes.All(attribute => attribute.Type.ToString() != "CvarAttribute")
									 select property)
			{
				Error(property, "Expected 'Cvar' attribute to be declared.");
			}

			// Check if there are any methods without the command attribute
			foreach (var method in from method in _type.Descendants.OfType<MethodDeclaration>()
								   let attributes = method.Attributes.SelectMany(section => section.Attributes)
								   where attributes.All(attribute => attribute.Type.ToString() != "Command") &&
										 attributes.All(attribute => attribute.Type.ToString() != "CommandAttribute")
								   select method)
			{
				Error(method, "Expected 'Command' attribute to be declared.");
			}

			// Check if there are any indexer declaration
			foreach (var indexer in _type.Descendants.OfType<IndexerDeclaration>())
				Error(indexer, "Unexpected indexer declaration.");

			// Check if there are any event declarations
			foreach (var declaration in _type.Descendants.OfType<EventDeclaration>())
				Error(declaration, "Unexpected event declaration.");
		}
	}
}