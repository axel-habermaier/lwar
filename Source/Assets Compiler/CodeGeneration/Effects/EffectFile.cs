namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using AssetsCompiler.Effects;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using Platform.Graphics;
	using Platform.Memory;
	using Effect = AssetsCompiler.Effects.Effect;

	/// <summary>
	///   Represents a C# source code file that possibly contains one or more effect declarations.
	/// </summary>
	internal class EffectFile : EffectElement
	{
		/// <summary>
		///   The C# syntax tree of the effect file.
		/// </summary>
		private readonly SyntaxTree _syntaxTree;

		/// <summary>
		///   Initializes a new instance.
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
		///   Gets the effects that are declared in the file.
		/// </summary>
		private IEnumerable<EffectClass> Effects
		{
			get { return GetChildElements<EffectClass>(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			// Add all effect classes
			AddElements(from type in _syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>()
						where type.ClassType == ClassType.Class
						where type.IsDerivedFrom<Effect>(Resolver) || type.Attributes.Contain<EffectAttribute>(Resolver)
						select new EffectClass(type));
		}

		/// <summary>
		///   Compiles all effects declared in the file.
		/// </summary>
		/// <param name="generator">The C# code generator that should be used to generate the C# effect code.</param>
		public void Compile(CSharpCodeGenerator generator)
		{
			foreach (var effectShader in from effect in Effects
										 from shader in effect.Shaders
										 select new { Effect = effect, Shader = shader })
			{
				CompileShaderCode(effectShader.Effect, effectShader.Shader);
			}

			foreach (var effect in Effects)
				generator.GenerateCode(effect);
		}

		/// <summary>
		///   Generates the GLSL and HLSL shader code for the given shader method of the given effect.
		/// </summary>
		/// <param name="effect">The effect that declares the shader method that should be compiled.</param>
		/// <param name="shader">The shader method that should be compiled.</param>
		private void CompileShaderCode(EffectClass effect, ShaderMethod shader)
		{
			var content = new byte[2 * 1024 * 1024];

			using (var asset = new ShaderAsset(effect.FullName, shader.Name, shader.Type))
			using (var buffer = BufferWriter.Create(content))
			{
				if (shader.Type == ShaderType.VertexShader)
				{
					buffer.WriteByte((byte)shader.InputLayout.Count());
					foreach (var input in shader.InputLayout)
					{
						buffer.WriteByte((byte)input.Format);
						buffer.WriteByte((byte)input.Semantics);
					}
				}

				var codeWriter = new CodeWriter();
				var glslCompiler = new GlslCompiler();
				glslCompiler.Compile(effect, shader, codeWriter, Resolver);
				buffer.WriteString(codeWriter.ToString());

				codeWriter = new CodeWriter();
				var hlslCompiler = new HlslCompiler();
				hlslCompiler.Compile(effect, shader, codeWriter, Resolver);
				buffer.WriteString(codeWriter.ToString());

				var fileContent = content.Take(buffer.Count).ToArray();
				File.WriteAllBytes(asset.SourcePath, fileContent);
			}
		}
	}
}