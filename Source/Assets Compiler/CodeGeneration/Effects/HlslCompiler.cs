using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Collections.Generic;
	using System.Linq;
	using AssetsCompiler.Effects;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using Platform.Graphics;

	/// <summary>
	///   Cross-compiles a C# shader method to HLSL.
	/// </summary>
	internal sealed class HlslCompiler : CrossCompiler
	{
		/// <summary>
		///   The name of the shader output structure.
		/// </summary>
		public const string OutputStructName = Configuration.ReservedInternalIdentifierPrefix + "OUTPUT";

		/// <summary>
		///   The name of the shader input structure.
		/// </summary>
		public const string InputStructName = Configuration.ReservedInternalIdentifierPrefix + "INPUT";

		/// <summary>
		///   The name of the shader input variable.
		/// </summary>
		public const string InputVariableName = Configuration.ReservedInternalIdentifierPrefix + "input";

		/// <summary>
		///   The name of the shader input variable.
		/// </summary>
		public const string OutputVariableName = Configuration.ReservedInternalIdentifierPrefix + "output";

		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("static const {0} {1}", ToShaderType(literal.Type), Escape(literal.Name));

			if (literal.IsArray)
				Writer.Append("[]");

			Writer.Append(" = ");

			if (literal.IsArray)
			{
				Writer.Append("{{ ");
				Writer.Append(String.Join(", ", literal.Value.GetConstantValues(Resolver)));
				Writer.Append(" }}");
			}
			else if (literal.IsConstructed)
				literal.Value.AcceptVisitor(this);
			else
				Writer.Append(literal.Value.ToString().ToLower());

			Writer.AppendLine(";");
		}

		/// <summary>
		///   Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected override void GenerateConstantBuffer(ConstantBuffer constantBuffer)
		{
			Writer.Append("cbuffer {2}{0} : register(b{1})", constantBuffer.Name, constantBuffer.Slot,
						  Configuration.ReservedInternalIdentifierPrefix);
			Writer.AppendBlockStatement(() =>
			{
				foreach (var constant in constantBuffer.Constants)
				{
					if (constant.Type == DataType.Matrix)
						Writer.Append("column_major ");

					Writer.AppendLine("{0} {1};", ToShaderType(constant.Type), Escape(constant.Name));
				}
			});
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected override void GenerateTextureObject(ShaderTexture texture)
		{
			Writer.AppendLine("{0} {1} : register(t{2});", ToShaderType(texture.Type), Escape(texture.Name), texture.Slot);
			Writer.AppendLine("SamplerState {0} : register(s{1});", GetSamplerName(texture.Name), texture.Slot);
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader inputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateVertexShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			GenerateInputs(inputs);
		}

		/// <summary>
		///   Generates the shader outputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateVertexShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			Writer.AppendLine("struct {0}", OutputStructName);
			Writer.AppendBlockStatement(() =>
			{
				var position = outputs.Single(output => output.Semantics == DataSemantics.Position);
				foreach (var output in outputs.Except(new[] { position }))
					Writer.AppendLine("{0} {1} : {2};", ToShaderType(output.Type), Escape(output.Name), ToHlsl(output.Semantics));

				Writer.AppendLine("{0} {1} : SV_Position;", ToShaderType(position.Type), Escape(position.Name));
			}, true);
		}

		/// <summary>
		///   Generates the shader inputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateFragmentShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			GenerateInputs(inputs);
		}

		/// <summary>
		///   Generates the shader outputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateFragmentShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			Writer.AppendLine("struct {0}", OutputStructName);
			Writer.AppendBlockStatement(() =>
			{
				foreach (var output in outputs)
				{
					var index = output.Semantics - DataSemantics.Color0;
					var semantics = "SV_Target" + index;

					Assert.InRange(index, 0, SemanticsAttribute.MaximumIndex);
					Writer.AppendLine("{0} {1} : {2};", ToShaderType(output.Type), Escape(output.Name), semantics);
				}
			}, true);
		}

		/// <summary>
		///   Generates the shader inputs.
		/// </summary>
		private void GenerateInputs(IEnumerable<ShaderParameter> inputs)
		{
			Writer.AppendLine("struct {0}", InputStructName);
			Writer.AppendBlockStatement(() =>
			{
				foreach (var input in inputs)
					Writer.AppendLine("{0} {1} : {2};", ToShaderType(input.Type), Escape(input.Name), ToHlsl(input.Semantics));
			}, true);
		}

		/// <summary>
		///   Generates the shader entry point.
		/// </summary>
		protected override void GenerateMainMethod()
		{
			Writer.AppendLine("{0} Main({1} {2})", OutputStructName, InputStructName, InputVariableName);
			Writer.AppendBlockStatement(() =>
			{
				Writer.AppendLine("{0} {1};", OutputStructName, OutputVariableName);
				Writer.Newline();

				Shader.MethodBody.AcceptVisitor(this);

				Writer.Newline();
				Writer.AppendLine("return {0};", OutputVariableName);
			});
		}

		/// <summary>
		///   Gets the corresponding HLSL type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		protected override string ToShaderType(DataType type)
		{
			switch (type)
			{
				case DataType.Boolean:
					return "bool";
				case DataType.Integer:
					return "int";
				case DataType.Float:
					return "float";
				case DataType.Vector2:
					return "float2";
				case DataType.Vector3:
					return "float3";
				case DataType.Vector4:
					return "float4";
				case DataType.Matrix:
					return "float4x4";
				case DataType.Texture2D:
					return "Texture2D";
				case DataType.CubeMap:
					return "TextureCube";
				default:
					throw new NotSupportedException("Unsupported data type.");
			}
		}

		/// <summary>
		///   Gets the corresponding HLSL semantics.
		/// </summary>
		/// <param name="semantics">The data semantics that should be converted.</param>
		private static string ToHlsl(DataSemantics semantics)
		{
			switch (semantics)
			{
				case DataSemantics.Position:
					return "POSITION";
				case DataSemantics.Color0:
					return "COLOR0";
				case DataSemantics.Color1:
					return "COLOR1";
				case DataSemantics.Color2:
					return "COLOR2";
				case DataSemantics.Color3:
					return "COLOR3";
				case DataSemantics.Normal:
					return "NORMAL";
				case DataSemantics.TexCoords0:
					return "TEXCOORD0";
				case DataSemantics.TexCoords1:
					return "TEXCOORD1";
				case DataSemantics.TexCoords2:
					return "TEXCOORD2";
				case DataSemantics.TexCoords3:
					return "TEXCOORD3";
				default:
					throw new NotSupportedException("Unsupported semantics.");
			}
		}

		/// <summary>
		///   Gets the token for the given intrinsic function.
		/// </summary>
		/// <param name="intrinsic">The intrinsic function for which the token should be returned.</param>
		protected override string GetToken(Intrinsic intrinsic)
		{
			switch (intrinsic)
			{
				case Intrinsic.InverseSquareRoot:
					return "rsqrt";
				default:
					return base.GetToken(intrinsic);
			}
		}

		/// <summary>
		///   Gets the sampler name for the given texture name.
		/// </summary>
		/// <param name="textureName">The texture name that should be converted.</param>
		private static string GetSamplerName(string textureName)
		{
			return String.Format("{0}{1}Sampler", Configuration.ReservedInternalIdentifierPrefix, textureName);
		}

		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			var local = Resolver.Resolve(identifierExpression) as LocalResolveResult;
			if (local != null)
			{
				if (local.IsParameter)
				{
					var parameter = Shader.Parameters.Single(p => p.Name == local.Variable.Name);
					if (parameter.IsOutput)
						Writer.Append("{0}.{1}", OutputVariableName, Escape(identifierExpression.Identifier));
					else
						Writer.Append("{0}.{1}", InputVariableName, Escape(identifierExpression.Identifier));

					return;
				}
			}

			base.VisitIdentifierExpression(identifierExpression);
		}

		public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			if (binaryOperatorExpression.Operator == BinaryOperatorType.Multiply)
			{
				var leftType = Resolver.Resolve(binaryOperatorExpression.Left).Type.ToDataType();
				var rightType = Resolver.Resolve(binaryOperatorExpression.Left).Type.ToDataType();

				if (leftType == DataType.Matrix || rightType == DataType.Matrix)
				{
					Writer.Append("mul(");
					binaryOperatorExpression.Left.AcceptVisitor(this);
					Writer.Append(", ");
					binaryOperatorExpression.Right.AcceptVisitor(this);
					Writer.Append(")");

					return;
				}
			}

			base.VisitBinaryOperatorExpression(binaryOperatorExpression);
		}

		public override void VisitReturnStatement(ReturnStatement returnStatement)
		{
			Writer.Append("return {0}", OutputVariableName);
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			var intrinsic = invocationExpression.ResolveIntrinsic(Resolver);
			if (intrinsic != Intrinsic.Sample && intrinsic != Intrinsic.SampleLevel)
			{
				base.VisitInvocationExpression(invocationExpression);
				return;
			}

			var target = (IdentifierExpression)((MemberReferenceExpression)invocationExpression.Target).Target;
			Writer.Append("{0}.", Escape(target.Identifier));

			if (intrinsic == Intrinsic.Sample)
				Writer.Append("Sample(");

			if (intrinsic == Intrinsic.SampleLevel)
				Writer.Append("SampleLevel(");

			Writer.Append(GetSamplerName(target.Identifier));

			if (invocationExpression.Arguments.Count > 0)
			{
				Writer.Append(", ");
				invocationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			}

			Writer.Append(")");
		}

		/// <summary>
		///   Extracts the column and row indices from the list of indexer arguments.
		/// </summary>
		/// <param name="indexerArguments">The list of indexer arguments.</param>
		/// <param name="first">The expression that should be used as the first index.</param>
		/// <param name="second">The expression that should be used as the second index.</param>
		protected override void GetMatrixIndices(AstNodeCollection<Expression> indexerArguments, out Expression first,
												 out Expression second)
		{
			second = indexerArguments.Skip(1).Single();
			first = indexerArguments.First();
		}
	}
}