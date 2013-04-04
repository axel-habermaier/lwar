using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
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
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all cvars
			AddElements(from property in _type.Descendants.OfType<PropertyDeclaration>()
						let attributes = property.Attributes.SelectMany(section => section.Attributes)
						let attribute = attributes.SingleOrDefault(attribute => attribute.Type.ToString() == "Cvar")
						where attribute != null
						select new Cvar(property));
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
									 let attribute = attributes.SingleOrDefault(attribute => attribute.Type.ToString() == "Cvar")
									 where attribute == null
									 select property)
			{
				Error(property, "Expected 'Cvar' attribute to be declared.");
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