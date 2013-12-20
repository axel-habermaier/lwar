namespace Pegasus.AssetsCompiler.Registries
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///     Represents a command method of the registry interface.
	/// </summary>
	internal class Command : RegistryElement
	{
		/// <summary>
		///     The declaration of the method that represents the command.
		/// </summary>
		private readonly MethodDeclaration _method;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="method">The declaration of the method that represents the command.</param>
		public Command(MethodDeclaration method)
		{
			Assert.ArgumentNotNull(method);
			_method = method;
		}

		/// <summary>
		///     Gets the name of the command.
		/// </summary>
		public string Name
		{
			get { return _method.Name; }
		}

		/// <summary>
		///     Gets the command's parameters.
		/// </summary>
		public IEnumerable<CommandParameter> Parameters
		{
			get { return GetChildElements<CommandParameter>(); }
		}

		/// <summary>
		///     Gets the documentation of the cvar.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _method.GetDocumentation(); }
		}

		/// <summary>
		///     Gets a value indicating whether the command can only be invoked by the system and not via the console.
		/// </summary>
		public bool SystemOnly
		{
			get { return GetAttribute(_method.Attributes, "SystemOnly") != null; }
		}

		/// <summary>
		///     Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all parameters
			AddElements(from parameter in _method.Descendants.OfType<ParameterDeclaration>()
						select new CommandParameter(parameter));
		}

		/// <summary>
		///     Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///     initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the method returns void
			if (_method.ResolveType(Resolver).FullName != typeof(void).FullName)
				Error(_method, "Expected return type 'void'.");

			// Check if the command attribute is applied to the method
			if (GetAttribute(_method.Attributes, "Command") == null)
				Error(_method, "Expected 'Command' attribute to be declared.");

			// Check if there are any type parameters
			foreach (var parameter in _method.TypeParameters)
				Error(parameter, "Unexpected type parameter '{0}'.", parameter.Name);
		}
	}
}