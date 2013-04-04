using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a parameter of a command method.
	/// </summary>
	internal class CommandParameter : RegistryElement
	{
		/// <summary>
		///   The declaration of the parameter that represents the command parameter.
		/// </summary>
		private readonly ParameterDeclaration _parameter;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parameter">The declaration of the parameter that represents the command parameter.</param>
		public CommandParameter(ParameterDeclaration parameter)
		{
			Assert.ArgumentNotNull(parameter, () => parameter);
			_parameter = parameter;
		}

		/// <summary>
		///   Gets the name of the parameter.
		/// </summary>
		public string Name
		{
			get { return _parameter.Name; }
		}

		/// <summary>
		///   Gets the type of the parameter.
		/// </summary>
		public string Type
		{
			get { return _parameter.Type.ToString(); }
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