namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using Utilities;

	/// <summary>
	///     Represents a C# source code file that possibly contains one or more effect declarations.
	/// </summary>
	internal class EffectFile : EffectElement
	{
		/// <summary>
		///     The C# syntax tree of the effect file.
		/// </summary>
		private readonly SyntaxTree _syntaxTree;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="errorReporter">The error reporter that should be used to report validation errors.</param>
		/// <param name="syntaxTree">The parsed syntax tree of the effect file.</param>
		/// <param name="resolver"> The C# AST resolver that should be used to resolve symbols of the effect file.</param>
		public EffectFile(IErrorReporter errorReporter, SyntaxTree syntaxTree, CSharpAstResolver resolver)
			: base(errorReporter, syntaxTree.FileName, resolver)
		{
			_syntaxTree = syntaxTree;
		}

		/// <summary>
		///     Gets the effects that are declared in the file.
		/// </summary>
		public IEnumerable<EffectClass> Effects
		{
			get { return GetChildElements<EffectClass>(); }
		}

		/// <summary>
		///     Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all effect classes
			AddElements(from type in _syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
						where type.ClassType == ClassType.Class
						where type.IsDerivedFrom<Effect>(Resolver) || type.Attributes.Contain<EffectAttribute>(Resolver)
						select new EffectClass(type) { Context = Context });
		}

		/// <summary>
		///     Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///     initialization.
		/// </summary>
		protected override void Validate()
		{
			if (!Effects.Any())
				Error(_syntaxTree, "Expected at least one effect class.");

			var secondEffect = (from type in _syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
								where type.ClassType == ClassType.Class
								where type.IsDerivedFrom<Effect>(Resolver) || type.Attributes.Contain<EffectAttribute>(Resolver)
								select type).Skip(1).FirstOrDefault();

			if (secondEffect != null)
				Error(secondEffect, "Unexpected second effect declaration. There can be only one effect declared in a C# file.");
		}

		/// <summary>
		///     Compiles all effects declared in the file.
		/// </summary>
		public void Compile()
		{
			var effectShaders = from effect in Effects
								from shader in effect.Shaders
								select new { Effect = effect, Shader = shader };

			foreach (var effectShader in effectShaders)
				CompileShaderCode(effectShader.Effect, effectShader.Shader);

			foreach (var effect in Effects)
			{
				using (var generator = new CSharpCodeGenerator())
					generator.GenerateCode(effect);
			}

			GenerateShaderSignatures();
		}

		/// <summary>
		///     Generates the GLSL and HLSL shader code for the given shader method of the given effect.
		/// </summary>
		/// <param name="effect">The effect that declares the shader method that should be compiled.</param>
		/// <param name="shader">The shader method that should be compiled.</param>
		private void CompileShaderCode(EffectClass effect, ShaderMethod shader)
		{
			string profile = null;

			switch (shader.Type)
			{
				case ShaderType.VertexShader:
					profile = "vs_4_0";
					break;
				case ShaderType.FragmentShader:
					profile = "ps_4_0";
					break;
				default:
					Log.Die("Unsupported shader type.");
					break;
			}

			Context.Writer.WriteBoolean(CompilationContext.CompileHlsl);
			Context.Writer.WriteBoolean(CompilationContext.CompileGlsl);

			var codeWriter = new CodeWriter();
			var glslCompiler = new GlslCompiler();
			glslCompiler.Compile(effect, shader, codeWriter, Resolver);
			CompileGlslShader(codeWriter.ToString());

			codeWriter = new CodeWriter();
			var hlslCompiler = new HlslCompiler();
			hlslCompiler.Compile(effect, shader, codeWriter, Resolver);
			CompileHlslShader(codeWriter.ToString(), profile);
		}

		/// <summary>
		///     Writes the generated GLSL shader code to the buffer.
		/// </summary>
		/// <param name="source">The GLSL shader source code.</param>
		private void CompileGlslShader(string source)
		{
			if (!CompilationContext.CompileGlsl)
				return;

			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + source;

			var code = Encoding.UTF8.GetBytes(shader);
			Context.Writer.WriteInt32(code.Length + 1);
			Context.Writer.Copy(code);
			Context.Writer.WriteByte(0);
		}

		/// <summary>
		///     Compiles the HLSL shader of the given profile and writes the generated code into the buffer.
		/// </summary>
		/// <param name="source">The HLSL shader source code.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		private void CompileHlslShader(string source, string profile)
		{
			if (!CompilationContext.CompileHlsl)
				return;

			var hlslFile = Context.TempPath + ".hlsl";
			File.WriteAllText(hlslFile, source);

			var byteCodePath = Context.TempPath + ".fxo";
			ExternalTool.Fxc(hlslFile, byteCodePath, profile);

			var byteCode = File.ReadAllBytes(byteCodePath);
			Context.Writer.WriteInt32(byteCode.Length);
			Context.Writer.Copy(byteCode);
		}

		/// <summary>
		///     Creates the vertex shader signatures.
		/// </summary>
		private void GenerateShaderSignatures()
		{
			var signatures = new List<ShaderSignature>();
			foreach (var shader in Effects.SelectMany(effect => effect.Shaders).Where(shader => shader.Type == ShaderType.VertexShader))
			{
				var signature = shader.InputLayout.ToArray();
				var inputs = new ShaderInput[signature.Length];

				var i = 0;
				foreach (var input in signature)
				{
					inputs[i].Format = input.Format;
					inputs[i].Semantics = input.Semantics;
					++i;
				}

				signatures.Add(new ShaderSignature(inputs));
			}

			signatures = signatures.Distinct().ToList();

			for (var i = 0; i < signatures.Count; ++i)
			{
				if (!CompilationContext.CompileHlsl)
				{
					signatures[i] = new ShaderSignature(signatures[i].Inputs, new byte[0]);
					continue;
				}

				// Create a dummy HLSL vertex shader for the signature
				var inputs = String.Join(", ", signatures[i].Inputs.Select((input, index) =>
					String.Format("{0} p{1} : {2}", ToHlsl(input.Format), index, HlslCompiler.ToHlsl(input.Semantics))));
				var hlslCode = String.Format("float4 Main({0}) : SV_POSITION {{  return float4(0, 0, 0, 0); }}", inputs);

				// Compile the shader
				if (!CompilationContext.CompileHlsl)
					return;

				var hlslFile = Context.TempPath + ".sig.hlsl";
				var fxoFile = Context.TempPath + ".sig.fxo";

				File.WriteAllText(hlslFile, hlslCode);
				ExternalTool.Fxc(hlslFile, fxoFile, "vs_4_0", generateDebugInfo: false);

				// Retrieve the compiled byte code
				signatures[i] = new ShaderSignature(signatures[i].Inputs, File.ReadAllBytes(fxoFile));
			}

			Context.Writer.WriteInt32(signatures.Count);
			foreach (var signature in signatures)
			{
				Context.Writer.WriteByte((byte)signature.Inputs.Length);
				foreach (var input in signature.Inputs)
				{
					Context.Writer.WriteByte((byte)input.Format);
					Context.Writer.WriteByte((byte)input.Semantics);
				}

				Context.Writer.WriteByteArray(signature.ByteCode);
			}
		}

		/// <summary>
		///     Gets the HLSL type that represents the vertex data format.
		/// </summary>
		/// <param name="format">The format that should be converted to HLSL.</param>
		private static string ToHlsl(VertexDataFormat format)
		{
			switch (format)
			{
				case VertexDataFormat.Float:
					return "float";
				case VertexDataFormat.Vector2:
					return "float2";
				case VertexDataFormat.Vector3:
					return "float3";
				case VertexDataFormat.Vector4:
				case VertexDataFormat.Color:
					return "float4";
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}
	}
}