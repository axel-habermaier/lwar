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
		///   Gets a value indicating whether the parameter has a default value.
		/// </summary>
		public bool HasDefaultValue
		{
			get { return !_parameter.DefaultExpression.IsNull; }
		}

		/// <summary>
		///   Gets the default value of the parameter, if any.
		/// </summary>
		public string DefaultValue
		{
			get { return _parameter.DefaultExpression.ToString(); }
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the parameter has any modifiers set
			if (_parameter.ParameterModifier != ParameterModifier.None)
				Error(_parameter, "Unexpected modifier '{0}'.", _parameter.ParameterModifier.ToString().ToLower());

			if (_parameter.Type is ComposedType)
				Error(_parameter, "Unexepcted array declaration.");
		}
	}
}