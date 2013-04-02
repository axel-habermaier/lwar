using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Represents a C# source code file that possibly contains one or more effect declarations.
	/// </summary>
	internal class EffectFile : CompiledElement
	{
		/// <summary>
		///   The name of the effect file.
		/// </summary>
		private readonly string _file;

		/// <summary>
		///   The C# syntax tree of the effect file.
		/// </summary>
		private readonly SyntaxTree _syntaxTree;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="syntaxTree">The parsed syntax tree of the effect file.</param>
		/// <param name="resolver"> The C# AST resolver that should be used to resolve symbols of the effect file.</param>
		public EffectFile(SyntaxTree syntaxTree, CSharpAstResolver resolver)
			: base(syntaxTree.FileName, resolver)
		{
			_syntaxTree = syntaxTree;
			_file = syntaxTree.FileName;
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
			var elements = (from effect in Effects
							from shader in effect.Shaders
							let assetPath = String.Format("{0}.{1}", effect.FullName, shader.Name)
							select new { Effect = effect, Shader = shader, Asset = CreateAsset(shader, assetPath) }).ToArray();

			var assets = elements.Select(element => element.Asset).ToArray();
			try
			{
				foreach (var element in elements)
					CompileShaderCode(element.Asset, element.Effect, element.Shader);

				foreach (var effect in Effects)
					generator.GenerateCode(effect, Path.GetDirectoryName(_file));
			}
			finally
			{
				assets.SafeDisposeAll();
			}
		}

		/// <summary>
		///   Creates the appropriate asset for the given shader type and source path.
		/// </summary>
		/// <param name="shader">The shader for which the asset should be created.</param>
		/// <param name="path">The source path of the asset.</param>
		private Asset CreateAsset(ShaderMethod shader, string path)
		{
			if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName(_file)))
				path = Path.Combine(Path.GetDirectoryName(_file), path);

			path = path.Replace("\\", "/");
			return new ShaderAsset(String.Format("{0}.{1}", path, shader.Type), Configuration.TempDirectory);
		}

		/// <summary>
		///   Generates the GLSL and HLSL shader code for the given shader method of the given effect.
		/// </summary>
		/// <param name="asset">The asset that represents the compiled shader.</param>
		/// <param name="effect">The effect that declares the shader method that should be compiled.</param>
		/// <param name="shader">The shader method that should be compiled.</param>
		private void CompileShaderCode(Asset asset, EffectClass effect, ShaderMethod shader)
		{
			using (var writer = new AssetWriter(asset))
			{
				var buffer = writer.Writer;
				string profile;

				switch (shader.Type)
				{
					case ShaderType.VertexShader:
						buffer.WriteByte((byte)shader.InputLayout.Count());
						foreach (var input in shader.InputLayout)
						{
							buffer.WriteByte((byte)input.Format);
							buffer.WriteByte((byte)input.Semantics);
						}

						profile = "vs_4_0";
						break;
					case ShaderType.FragmentShader:
						profile = "ps_4_0";
						break;
					default:
						throw new InvalidOperationException("Unsupported shader type.");
				}

				var codeWriter = new CodeWriter();
				var glslCompiler = new GlslCompiler();
				glslCompiler.Compile(effect, shader, codeWriter, Resolver);
				CompileGlslShader(buffer, codeWriter.ToString());

				codeWriter = new CodeWriter();
				var hlslCompiler = new HlslCompiler();
				hlslCompiler.Compile(effect, shader, codeWriter, Resolver);
				CompileHlslShader(asset, buffer, codeWriter.ToString(), profile);
			}
		}

		/// <summary>
		///   Writes the generated GLSL shader code to the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		/// <param name="source">The GLSL shader source code.</param>
		private static void CompileGlslShader(BufferWriter buffer, string source)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + source;

			var code = Encoding.UTF8.GetBytes(shader);
			buffer.WriteInt32(code.Length + 1);
			buffer.Copy(code);
			buffer.WriteByte(0);
		}

		/// <summary>
		///   Compiles the HLSL shader of the given profile and writes the generated code into the buffer.
		/// </summary>
		/// <param name="asset">The asset that contains the shader source code.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		/// <param name="source">The HLSL shader source code.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		protected static void CompileHlslShader(Asset asset, BufferWriter buffer, string source, string profile)
		{
			if (!Configuration.CompileHlsl)
				return;

			var hlslFile = asset.TempPathWithoutExtension + ".hlsl";
			File.WriteAllText(hlslFile, source);

			var byteCode = asset.TempPathWithoutExtension + ".fxo";
			ExternalTool.Fxc(hlslFile, byteCode, profile);

			buffer.Copy(File.ReadAllBytes(byteCode));
		}
	}
}