using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Collections.Generic;
	using System.Linq;
	using AssetsCompiler.Effects;
	using ICSharpCode.NRefactory.CSharp;
	using Platform.Graphics;

	/// <summary>
	///   Represents a C# method that is cross-compiled to GLSL or HLSL.
	/// </summary>
	internal class ShaderMethod : EffectElement
	{
		/// <summary>
		///   The declaration of the method that represents the shader.
		/// </summary>
		private readonly MethodDeclaration _method;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="method">The declaration of the method that represents the shader.</param>
		public ShaderMethod(MethodDeclaration method)
		{
			Assert.ArgumentNotNull(method);
			_method = method;
		}

		/// <summary>
		///   Gets the name of the shader.
		/// </summary>
		public string Name
		{
			get { return _method.Name; }
		}

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type
		{
			get
			{
				if (_method.Attributes.Contain<VertexShaderAttribute>(Resolver))
					return ShaderType.VertexShader;

				if (_method.Attributes.Contain<FragmentShaderAttribute>(Resolver))
					return ShaderType.FragmentShader;

				throw new InvalidOperationException("Unsupported shader type.");
			}
		}

		/// <summary>
		///   Gets the syntax tree of the shader method's body.
		/// </summary>
		public BlockStatement MethodBody
		{
			get { return _method.Body; }
		}

		/// <summary>
		///   Gets the input parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Inputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => !parameter.IsOutput); }
		}

		/// <summary>
		///   Gets the output parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Outputs
		{
			get { return GetChildElements<ShaderParameter>().Where(parameter => parameter.IsOutput); }
		}

		/// <summary>
		///   Gets all parameters declared by the shader method.
		/// </summary>
		public IEnumerable<ShaderParameter> Parameters
		{
			get { return GetChildElements<ShaderParameter>(); }
		}

		/// <summary>
		///   Gets the input layout for the shader.
		/// </summary>
		public IEnumerable<ShaderInput> InputLayout
		{
			get
			{
				Assert.That(Type == ShaderType.VertexShader, "The input layout is only required for vertex shaders.");
				foreach (var input in Inputs)
				{
					VertexDataFormat format;
					switch (input.Type)
					{
						case DataType.Float:
							format = VertexDataFormat.Float;
							break;
						case DataType.Vector2:
							format = VertexDataFormat.Vector2;
							break;
						case DataType.Vector3:
							format = VertexDataFormat.Vector3;
							break;
						case DataType.Vector4:
							if (input.Semantics.IsColor())
								format = VertexDataFormat.Color;
							else
								format = VertexDataFormat.Vector4;
							break;
						default:
							throw new InvalidOperationException("Unsupported data format.");
					}

					yield return new ShaderInput
					{
						Semantics = input.Semantics,
						Format = format
					};
				}
			}
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all shader parameters
			AddElements(from parameter in _method.Descendants.OfType<ParameterDeclaration>()
						select new ShaderParameter(parameter));
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			// Check whether both the vertex and fragment shader attributes are declared on the method
			var isVertexShader = _method.Attributes.Contain<VertexShaderAttribute>(Resolver);
			var isFragmentShader = _method.Attributes.Contain<FragmentShaderAttribute>(Resolver);

			if (isVertexShader && isFragmentShader)
				Error(_method.Attributes.First(), "Unexpected declaration of both '{0}' and '{1}'.",
					  typeof(VertexShaderAttribute).FullName, typeof(FragmentShaderAttribute).FullName);

			// Check whether the method returns void
			if (_method.ResolveType(Resolver).FullName != typeof(void).FullName)
				Error(_method.ReturnType, "Expected return type 'void'.");

			// Check whether 'public' is the only declared modifier 
			ValidateModifiers(_method, _method.ModifierTokens, new[] { Modifiers.Public });

			// Check whether the shader depends on any type arguments
			foreach (var parameter in _method.TypeParameters)
				Error(parameter, "Unexpected type parameter '{0}'.", parameter.Name);

			// Check whether the vertex shader declares an output parameter with the Position semantics
			if (Type == ShaderType.VertexShader && Outputs.All(output => output.Semantics != DataSemantics.Position))
				Error(_method, "Expected an output parameter with the '{0}' semantics.", DataSemantics.Position.ToDisplayString());

			// Check whether the fragment shader declares an output parameter with the Color semantics
			if (Type == ShaderType.FragmentShader && Outputs.All(output => !output.Semantics.IsColor()))
				Error(_method, "Expected an output parameter with the 'Color' semantics.");

			// Check whether the fragment shader declares any output parameters that do not have the color semantics
			if (Type == ShaderType.FragmentShader)
			{
				foreach (var output in from parameter in _method.Descendants.OfType<ParameterDeclaration>()
									   where parameter.ParameterModifier == ParameterModifier.Out
									   from attribute in parameter.GetSemantics(Resolver)
									   let semantics = attribute.ToSemanticsAttribute(Resolver)
									   where !semantics.Semantics.IsColor()
									   select new { Attribute = attribute, semantics.Semantics })
				{
					Error(output.Attribute, "Unexpected '{0}' semantics.", output.Semantics.ToDisplayString());
				}
			}

			// Check whether the all inputs and outputs have distinct semantics
			ValidateSemantics(Inputs, "input");
			ValidateSemantics(Outputs, "output");
		}

		/// <summary>
		///   Checks whether the given parameters are declared with distinct semantics.
		/// </summary>
		/// <param name="parameters">The parameters that should be checked.</param>
		/// <param name="direction">A description of the parameter direction.</param>
		private void ValidateSemantics(IEnumerable<ShaderParameter> parameters, string direction)
		{
			var groups = from parameter in parameters
						 group parameter by parameter.Semantics
						 into semantics
						 where semantics.Count() > 1
						 select semantics;

			foreach (var group in groups)
			{
				var semantics = group.First().Semantics;
				Error(_method, "Semantics '{0}' is applied to more than one {1} parameter.", semantics.ToDisplayString(), direction);
			}
		}
	}
}