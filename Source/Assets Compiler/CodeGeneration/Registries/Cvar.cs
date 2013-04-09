using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a cvar property of the registry interface.
	/// </summary>
	internal class Cvar : RegistryElement
	{
		/// <summary>
		///   The declaration of the property that represents the cvar.
		/// </summary>
		private readonly PropertyDeclaration _property;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The declaration of the property that represents the cvar.</param>
		public Cvar(PropertyDeclaration property)
		{
			Assert.ArgumentNotNull(property, () => property);
			_property = property;
		}

		/// <summary>
		///   Gets the name of the cvar.
		/// </summary>
		public string Name
		{
			get { return _property.Name; }
		}

		/// <summary>
		///   Gets the type of the cvar.
		/// </summary>
		public string Type
		{
			get { return _property.ReturnType.ToString(); }
		}

		/// <summary>
		///   Gets the documentation of the cvar.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _property.GetDocumentation(); }
		}

		/// <summary>
		///   Gets the validators that have been declared for the cvar.
		/// </summary>
		public IEnumerable<Validator> Validators
		{
			get { return GetChildElements<Validator>(); }
		}

		/// <summary>
		///   Gets the default value of the cvar.
		/// </summary>
		public string DefaultValue
		{
			get
			{
				var attribute = GetAttribute(_property.Attributes, "Cvar");
				if (attribute.Arguments.Count() != 1)
					return String.Format("default({0})", Type);

				return attribute.Arguments.First().ToString();
			}
		}

		/// <summary>
		///   Gets a value indicating whether the cvar's value is persisted across sessions.
		/// </summary>
		public bool Persistent
		{
			get { return GetAttribute(_property.Attributes, "Persistent") != null; }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all validators declared for the cvar
			AddElements(from section in _property.Attributes
						from attribute in section.Attributes
						where !AttributeHasType(attribute, "Cvar") && !AttributeHasType(attribute, "Persistent")
						select new Validator(attribute));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check if the cvar attribute is applied to the property
			if (GetAttribute(_property.Attributes, "Cvar") == null)
				Error(_property, "Expected 'Cvar' attribute to be declared.");
		}
	}
}