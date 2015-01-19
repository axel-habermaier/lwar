namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Utilities;

	/// <summary>
	///     Represents a field of an effect class that defines the combination of shaders that should be set on the GPU to
	///     create a graphical effect.
	/// </summary>
	internal class EffectTechnique : EffectElement
	{
		/// <summary>
		///     The declaration of the field that represents the technique.
		/// </summary>
		private readonly FieldDeclaration _field;

		/// <summary>
		///     The shaders declared by the effect the technique belongs to.
		/// </summary>
		private readonly ShaderMethod[] _shaders;

		/// <summary>
		///     The variable that represents the technique.
		/// </summary>
		private readonly VariableInitializer _variable;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="field">The declaration of the field that represents the technique.</param>
		/// <param name="variable">The variable that represents the technique.</param>
		/// <param name="shaders">The shaders declared by the effect the technique belongs to.</param>
		public EffectTechnique(FieldDeclaration field, VariableInitializer variable, ShaderMethod[] shaders)
		{
			Assert.ArgumentNotNull(field);
			Assert.ArgumentNotNull(variable);
			Assert.ArgumentNotNull(shaders);

			_field = field;
			_variable = variable;
			_shaders = shaders;
		}

		/// <summary>
		///     Gets the documentation of the effect.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _field.GetDocumentation(); }
		}

		/// <summary>
		///     Gets the name of the technique.
		/// </summary>
		public string Name
		{
			get { return _variable.Name; }
		}

		/// <summary>
		///     Gets the vertex shader that should be bound.
		/// </summary>
		public ShaderMethod VertexShader { get; private set; }

		/// <summary>
		///     Gets the fragment shader that should be bound.
		/// </summary>
		public ShaderMethod FragmentShader { get; private set; }

		/// <summary>
		///     Invoked when the element should initialize itself.
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

					var shader = _shaders.SingleOrDefault(s => s.Name == (string)resolved.ConstantValue);
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
		///     Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///     initialization.
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

			// Check whether all shaders referenced by the declared techniques are actually declared
			foreach (var shader in from namedExpression in _variable.Descendants.OfType<NamedExpression>()
								   let shaderType = (ShaderType)Enum.Parse(typeof(ShaderType), namedExpression.Name)
								   let resolved = Resolver.Resolve(namedExpression.Expression)
								   where resolved.IsCompileTimeConstant
								   let name = (string)resolved.ConstantValue
								   where !String.IsNullOrWhiteSpace(name)
								   where !_shaders.Any(shader => shader.Name == name && shader.Type == shaderType)
								   select new { namedExpression.Expression, Type = shaderType })
			{
				switch (shader.Type)
				{
					case ShaderType.VertexShader:
						Error(shader.Expression, "Reference to unknown vertex shader.");
						break;
					case ShaderType.FragmentShader:
						Error(shader.Expression, "Reference to unknown fragment shader.");
						break;
					default:
						throw new InvalidOperationException("Unsupported shader type.");
				}
			}

			ValidateShaderSignatures();
		}

		/// <summary>
		///     Gets a value indicating whether the technique uses the given constant buffer.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be checked.</param>
		public bool Uses(ConstantBuffer constantBuffer)
		{
			return VertexShader.Uses(constantBuffer) || FragmentShader.Uses(constantBuffer);
		}

		/// <summary>
		///     Gets a value indicating whether the technique uses the given shader texture.
		/// </summary>
		/// <param name="texture">The shader texture that should be checked.</param>
		public bool Uses(ShaderTexture texture)
		{
			return VertexShader.Uses(texture) || FragmentShader.Uses(texture);
		}

		/// <summary>
		///     Checks whether the signatures of the vertex and fragment shaders match.
		/// </summary>
		private void ValidateShaderSignatures()
		{
			foreach (var input in from input in FragmentShader.Inputs
								  where VertexShader.Outputs.All(output => output.Semantics != input.Semantics)
								  select input)
			{
				Error(_variable, "Shader signature mismatch: Fragment shader input '{0}' is not set by the vertex shader.", input.Name);
			}
		}

		/// <summary>
		///     Checks whether the technique's shader of the given type is not declared, null, or an empty string.
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