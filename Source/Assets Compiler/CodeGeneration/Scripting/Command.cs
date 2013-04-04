using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a command method of the registry interface.
	/// </summary>
	internal class Command : RegistryElement
	{
		/// <summary>
		///   The declaration of the method that represents the command.
		/// </summary>
		private readonly MethodDeclaration _method;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="method">The declaration of the method that represents the command.</param>
		public Command(MethodDeclaration method)
		{
			Assert.ArgumentNotNull(method, () => method);
			_method = method;
		}

		/// <summary>
		///   Gets the name of the command.
		/// </summary>
		public string Name
		{
			get { return _method.Name; }
		}

		/// <summary>
		///   Gets the type arguments of the command.
		/// </summary>
		public string TypeArguments
		{
			get
			{
				var types = String.Join(", ", Parameters.Select(parameter => parameter.Type));
				if (String.IsNullOrWhiteSpace(types))
					return String.Empty;

				return String.Format("<{0}>", types);
			}
		}

		/// <summary>
		///   Gets the command's parameters.
		/// </summary>
		public IEnumerable<CommandParameter> Parameters
		{
			get { return GetChildElements<CommandParameter>(); }
		}

		/// <summary>
		///   Gets the documentation of the cvar.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _method.GetDocumentation(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all parameters
			AddElements(from parameter in _method.Descendants.OfType<ParameterDeclaration>()
						select new CommandParameter(parameter));
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