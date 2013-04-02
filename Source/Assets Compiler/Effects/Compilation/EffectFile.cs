using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
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

			ShaderAssets = Enumerable.Empty<Asset>();
		}

		/// <summary>
		///   Gets the shader assets that have been generated during the compilation process.
		/// </summary>
		public IEnumerable<Asset> ShaderAssets { get; private set; }

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

			ShaderAssets = elements.Select(element => element.Asset).ToArray();

			foreach (var element in elements)
			{
				var writer = new CodeWriter();
				CompileShaderCode(element.Effect, element.Shader, writer);
				writer.WriteToFile(element.Asset.SourcePath);
			}

			foreach (var effect in Effects)
				generator.GenerateCode(effect, Path.GetDirectoryName(_file));
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
			switch (shader.Type)
			{
				case ShaderType.VertexShader:
					return new VertexShaderAsset(String.Format("{0}.vs", path), Configuration.TempDirectory, shader.InputLayout.ToArray());
				case ShaderType.FragmentShader:
					return new FragmentShaderAsset(String.Format("{0}.fs", path), Configuration.TempDirectory);
				default:
					throw new InvalidOperationException("Unsupported shader type.");
			}
		}

		/// <summary>
		///   Generates the GLSL and HLSL shader code for the given shader method of the given effect.
		/// </summary>
		/// <param name="effect">The effect that declares the shader method that should be compiled.</param>
		/// <param name="shader">The shader method that should be compiled.</param>
		/// <param name="writer">The writer that should be used to write the compiled output.</param>
		private void CompileShaderCode(EffectClass effect, ShaderMethod shader, CodeWriter writer)
		{
			var hlslCompiler = new HlslCompiler();
			var glslCompiler = new GlslCompiler();

			glslCompiler.Compile(effect, shader, writer, Resolver);

			writer.Newline();
			writer.AppendLine(Configuration.ShaderSeparator);
			writer.Newline();

			hlslCompiler.Compile(effect, shader, writer, Resolver);
		}
	}
}