using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a cvar property of the registry interface.
	/// </summary>
	internal class Cvar : RegistryElement
	{
		/// <summary>
		///   The declaration of the property that represents the cvar.
		/// </summary>
		private PropertyDeclaration _property;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The declaration of the property that represents the cvar.</param>
		public Cvar(PropertyDeclaration property)
		{
			_property = property;
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			base.Validate();
		}
	}
}