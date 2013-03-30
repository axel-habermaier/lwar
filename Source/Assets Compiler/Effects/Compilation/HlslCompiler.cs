﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using Semantics;

	/// <summary>
	///   Cross-compiles a C# shader method to HLSL.
	/// </summary>
	internal sealed class HlslCompiler : CrossCompiler
	{
		/// <summary>
		///   The name of the shader output structure.
		/// </summary>
		public const string OutputStructName = Configuration.ReservedVariablePrefix + "OUTPUT";

		/// <summary>
		///   The name of the shader input structure.
		/// </summary>
		public const string InputStructName = Configuration.ReservedVariablePrefix + "INPUT";

		/// <summary>
		///   The name of the shader input variable.
		/// </summary>
		public const string InputVariableName = Configuration.ReservedVariablePrefix + "input";

		/// <summary>
		///   The name of the shader input variable.
		/// </summary>
		public const string OutputVariableName = Configuration.ReservedVariablePrefix + "output";

		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("static const {0} {1}", ToShaderType(literal.Type), literal.Name);

			if (literal.IsArray)
				Writer.Append("[]");

			Writer.Append(" = ");

			if (literal.IsArray)
			{
				Writer.Append("{{ ");
				Writer.Append(String.Join(", ", (object[])literal.Value));
				Writer.Append(" }}");
			}
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
			Writer.Append("cbuffer {0} : register(b{1})", constantBuffer.Name, constantBuffer.Slot);
			Writer.AppendBlockStatement(() =>
				{
					foreach (var constant in constantBuffer.Constants)
					{
						if (constant.Type == DataType.Matrix)
							Writer.Append("column_major ");

						Writer.AppendLine("{0} {1};", ToShaderType(constant.Type), constant.Name);
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
			Writer.AppendLine("{0} {1} : register(t{2});", ToShaderType(texture.Type), texture.Name, texture.Slot);
			Writer.AppendLine("SamplerState {0}Sampler : register(s{1});", texture.Name, texture.Slot);
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
						Writer.AppendLine("{0} {1} : {2};", ToShaderType(output.Type), output.Name, ToHlsl(output.Semantics));

					Writer.AppendLine("{0} {1} : SV_Position;", ToShaderType(position.Type), position.Name);
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
						Writer.AppendLine("{0} {1} : {2};", ToShaderType(output.Type), output.Name, semantics);
					}
				});
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
						Writer.AppendLine("{0} {1} : {2};", ToShaderType(input.Type), input.Name, ToHlsl(input.Semantics));
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

					Shader.MethodBody.Statements.AcceptVisitor(this);

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
					return "unknown-type";
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
					return "unknown-semantics";
			}
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
						Writer.Append("{0}.{1}", OutputVariableName, identifierExpression.Identifier);
					else
						Writer.Append("{0}.{1}", InputVariableName, identifierExpression.Identifier);
				}
			}
			else
				base.VisitIdentifierExpression(identifierExpression);
		}

		//public override void VisitVariableReference<T>(VariableReference<T> variableReference)
		//{
		//	if (typeof(T) == typeof(ShaderParameter))
		//	{
		//		var parameter = (ShaderParameter)(object)variableReference.Variable;
		//		if (parameter.IsOutput)
		//			Writer.Append("{0}.{1}", OutputVariableName, parameter.Name);
		//		else
		//			Writer.Append("{0}.{1}", InputVariableName, parameter.Name);
		//	}
		//	else
		//		base.VisitVariableReference(variableReference);
		//}

		//public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		//{
		//	if (binaryOperatorExpression.Operator == BinaryOperatorType.Multiply &&
		//		(binaryOperatorExpression.LeftType == DataType.Matrix | binaryOperatorExpression.RightType == DataType.Matrix))
		//	{
		//		Writer.Append("mul(");
		//		binaryOperatorExpression.Left.AcceptVisitor(this);
		//		Writer.Append(", ");
		//		binaryOperatorExpression.Right.AcceptVisitor(this);
		//		Writer.Append(")");
		//	}
		//	else
		//		base.VisitBinaryOperatorExpression(binaryOperatorExpression);
		//}
	}
}