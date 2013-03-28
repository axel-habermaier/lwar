using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a C# method that is cross-compiled to GLSL or HLSL.
	/// </summary>
	internal class ShaderMethod
	{
		/// <summary>
		///   The declaration of the method that represents the shader.
		/// </summary>
		private readonly MethodDeclaration _method;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="method">The declaration of the method that represents the shader.</param>
		/// <param name="type">The type of the shader.</param>
		public ShaderMethod(MethodDeclaration method, ShaderType type)
		{
			Assert.ArgumentNotNull(method, () => method);
			Assert.ArgumentInRange(type, () => type);

			_method = method;
			Type = type;
		}

		/// <summary>
		///   Gets the cross-compiled shader asset that can subsequently compiled into the binary format.
		/// </summary>
		public Asset Asset { get; private set; }

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type { get; private set; }

		/// <summary>
		///   Gets the name of the shader.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the shader inputs.
		/// </summary>
		public ShaderParameter[] Inputs { get; private set; }

		/// <summary>
		///   Gets the shader outputs.
		/// </summary>
		public ShaderParameter[] Outputs { get; private set; }

		/// <summary>
		///   Gets the C# shader code.
		/// </summary>
		public AstNode ShaderCode
		{
			get { return _method.Body; }
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} ({1})", Name, Type);
		}

		/// <summary>
		///   Compiles the shader method.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			Name = _method.Name;
			GetParameters(context);

			if (_method.GetType(context).FullName != typeof(void).FullName)
				context.Error(_method, "Shader method '{0}' must have return type 'void'.", Name);

			if (_method.TypeParameters.Any() || _method.Modifiers != Modifiers.Public)
				context.Error(_method, "Shader '{0}' must be a public, non-static, non-partial, non-abstract, non-sealed, " +
									   "non-virtual method without any type arguments.", Name);

			switch (Type)
			{
				case ShaderType.VertexShader:
					if (Outputs.All(o => o.Semantics != DataSemantics.Position))
						context.Error(_method, "Vertex shader '{0}' must declare an output parameter with the '{1}' semantics.",
									  Name, DataSemantics.Position.ToDisplayString());

					for (var i = 0; i < Outputs.Length; ++i)
					{
						if (Outputs[i].Semantics == DataSemantics.Position)
						{
							var output = Outputs[i];
							var lastIndex = Outputs.Length - 1;
							Outputs[i] = Outputs[lastIndex];
							Outputs[lastIndex] = output;
							break;
						}
					}
					break;
				case ShaderType.FragmentShader:
					if (Outputs.All(o => !o.Semantics.IsColor()))
						context.Error(_method, "Fragment shader '{0}' must declare an output parameter with the 'Color' semantics.", Name);
					break;
				default:
					throw new InvalidOperationException("Unsupported shader type.");
			}

			CheckDistinctSemantics(context, Inputs, "input");
			CheckDistinctSemantics(context, Outputs, "output");
		}

		/// <summary>
		///   Gets the parameters of the shader.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetParameters(CompilationContext context)
		{
			var parameters = _method.Descendants.OfType<ParameterDeclaration>()
									.Select(parameter =>
										{
											var shaderParameter = new ShaderParameter(parameter);
											shaderParameter.Compile(context);

											if (shaderParameter.IsOutput && Type == ShaderType.FragmentShader && !shaderParameter.Semantics.IsColor())
												context.Error(parameter, "Fragment shader '{0}' cannot assign '{2}' semantics to output parameter '{1}'.",
															  Name, shaderParameter.Name, shaderParameter.Semantics.ToDisplayString());

											return shaderParameter;
										})
									.ToArray();

			Inputs = parameters.Where(parameter => !parameter.IsOutput).ToArray();
			Outputs = parameters.Where(parameter => parameter.IsOutput).ToArray();
		}

		/// <summary>
		///   Checks whether the given parameters are declared with distinct semantics.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="parameters">The parameters that should be checked.</param>
		/// <param name="direction">A description of the parameter direction.</param>
		private void CheckDistinctSemantics(CompilationContext context, IEnumerable<ShaderParameter> parameters, string direction)
		{
			var groups = parameters.GroupBy(parameter => parameter.Semantics).Where(group => group.Count() > 1);
			foreach (var semanticsGroup in groups)
			{
				context.Error(_method, "Shader '{0}' declares multiple {2} parameters with the '{1}' semantics.",
							  Name, semanticsGroup.First().Semantics.ToDisplayString(), direction);
			}
		}

		/// <summary>
		///   Generates the code for the shader.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="effect">The effect the shader belongs to.</param>
		public void GenerateCode(CompilationContext context, EffectClass effect)
		{
			var assetPath = String.Format("{0}_{1}_{2}", context.File.Asset.RelativePath, effect.FullName, Name);
			switch (Type)
			{
				case ShaderType.VertexShader:
					Asset = new VertexShaderAsset(String.Format("{0}.vs", assetPath), Configuration.TempDirectory);
					break;
				case ShaderType.FragmentShader:
					Asset = new FragmentShaderAsset(String.Format("{0}.fs", assetPath), Configuration.TempDirectory);
					break;
				default:
					throw new InvalidOperationException("Unsupported shader type.");
			}

			var writer = new CodeWriter();
			new GlslCrossCompiler().GenerateCode(context, effect, this, writer);

			writer.Newline();
			writer.AppendLine(Configuration.ShaderSeparator);
			writer.Newline();

			new HlslCrossCompiler().GenerateCode(context, effect, this, writer);

			writer.WriteToFile(Asset.SourcePath);
		}
	}
}