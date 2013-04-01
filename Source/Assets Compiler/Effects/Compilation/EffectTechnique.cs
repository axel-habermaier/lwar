using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;

	/// <summary>
	///   Represents a field of an effect class that defines the combination of shaders that should be set on the GPU to create
	///   a graphical effect.
	/// </summary>
	internal class EffectTechnique : CompiledElement
	{
		/// <summary>
		///   The declaration of the field that represents the technique.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///   The variable that represents the technique.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the technique.</param>
		/// <param name="variable">The variable that represents the technique.</param>
		public EffectTechnique(FieldDeclaration field, VariableInitializer variable)
		{
			Assert.ArgumentNotNull(field, () => field);
			Assert.ArgumentNotNull(variable, () => variable);

			_field = field;
			_variable = variable;

			VertexShader = String.Empty;
			FragmentShader = String.Empty;
		}

		/// <summary>
		///   Gets the name of the technique.
		/// </summary>
		public string Name
		{
			get { return _variable.Name; }
		}

		/// <summary>
		///   Gets the vertex shader that should be bound.
		/// </summary>
		public string VertexShader { get; private set; }

		/// <summary>
		///   Gets the fragment shader that should be bound.
		/// </summary>
		public string FragmentShader { get; private set; }

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			var initializer = _variable.Initializer as ObjectCreateExpression;
			if (initializer != null)
			{
				foreach (var expression in initializer.Descendants.OfType<NamedExpression>())
				{
					var resolved = Resolver.Resolve(expression.Expression);
					if (!resolved.IsCompileTimeConstant)
						continue;

					var shader = (string)resolved.ConstantValue;
					if (expression.Name == "VertexShader")
						VertexShader = shader;
					else if (expression.Name == "FragmentShader")
						FragmentShader = shader;
					else
						Assert.That(false, "Unexpected Technique property.");
				}
			}
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether the name is reserved
			ValidateIdentifier(_variable.NameToken);

			// Check whether the technique is an array type
			if (_field.ResolveType(Resolver).Kind == TypeKind.Array)
				Error(_field, "Unexpected array declaration.");

			// Check whether the declared modifiers match the expected ones
			ValidateModifiers(_field, _field.ModifierTokens, new[] { Modifiers.Public, Modifiers.Readonly });

			// Check whether the technique is initialized
			var initializer = _variable.Initializer as ObjectCreateExpression;
			if (initializer == null)
				Error(_variable, "Technique '{0}' must be initialized.", Name);
			else
			{
				var resolved = Resolver.Resolve(initializer) as InvocationResolveResult;
				if (resolved == null || resolved.Type.FullName != typeof(Technique).FullName)
					Error(_variable, "Expected invocation of '{0}' constructor.", typeof(Technique).FullName);

				foreach (var value in from expression in initializer.Descendants.OfType<NamedExpression>()
									  where !(expression.Expression is NullReferenceExpression)
									  let resolvedExpression = Resolver.Resolve(expression.Expression)
									  where !resolvedExpression.IsCompileTimeConstant
									  select expression.Expression)
				{
					Error(value, "Expected a compile-time constant value.");
				}

				ValidateShaderDeclaration(ShaderType.VertexShader);
				ValidateShaderDeclaration(ShaderType.FragmentShader);
			}
		}

		/// <summary>
		/// Checks whether the technique's shader of the given type is not declared, null, or an empty string.
		/// </summary>
		/// <param name="shaderType">The type of the shader that should be checked.</param>
		private void ValidateShaderDeclaration(ShaderType shaderType)
		{
			var declaration = _variable.Descendants.OfType<NamedExpression>()
									   .SingleOrDefault(expression => expression.Name == shaderType.ToString());

			if (declaration == null)
				Error(_variable, "Property '{0}' must be initialized.", shaderType);

			var resolved = Resolver.Resolve(declaration.Expression);
			var isNull = declaration.Expression is NullReferenceExpression;
			var isEmpty = resolved.IsCompileTimeConstant && String.IsNullOrWhiteSpace((string)resolved.ConstantValue);

			if (isNull || isEmpty)
				Error(declaration.Expression, "Expected the name of a shader.");
		}
	}
}