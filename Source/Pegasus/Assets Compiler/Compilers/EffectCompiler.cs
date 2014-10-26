namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using Assets;
	using CSharp;
	using Effects;
	using Effects.Compilation;
	using Pegasus.Assets;
	using Platform.Graphics;
	using Platform.Logging;
	using Utilities;
	using BinaryWriter = AssetsCompiler.BinaryWriter;

	/// <summary>
	///     Compiles effects written in C#.
	/// </summary>
	[UsedImplicitly]
	internal class EffectCompiler : AssetCompiler<ShaderAsset>
	{
		/// <summary>
		///     The shader assets that are compiled.
		/// </summary>
		private readonly ShaderAsset[] _shaderAssets;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public EffectCompiler()
		{
			_shaderAssets = CreateAssets().ToArray();
		}

		/// <summary>
		///     Gets the additional assets created by the compiler.
		/// </summary>
		public override IEnumerable<Asset> AdditionalAssets
		{
			get { return _shaderAssets; }
		}

		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public override void Compile(IEnumerable<Asset> assets)
		{
			var csharpAssets = assets.OfType<EffectAsset>().ToArray();

			if (DetermineAction(_shaderAssets, csharpAssets) == CompilationAction.Skip)
				Log.Info("Skipping effect compilation (no changes detected).");
			else
			{
				Log.Info("Compiling effects...");

				foreach (var asset in csharpAssets)
					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);

				var csharpFiles = csharpAssets.Select(asset => new CSharpFile(Configuration.SourceDirectory, asset.RelativePath));
				using (var project = new EffectsProject { CSharpFiles = csharpFiles })
				{
					if (!project.Compile())
						throw new InvalidOperationException("Effect project compilation failed.");
				}

				CreateShaderSignatureFile();
			}

			base.Compile(_shaderAssets);
		}

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		public override void Clean(IEnumerable<Asset> assets)
		{
			var shaderAssets = CreateAssets().ToArray();
			try
			{
				base.Clean(shaderAssets);

				foreach (var asset in shaderAssets)
					File.Delete(asset.SourcePath);

				foreach (var asset in assets.OfType<EffectAsset>())
					File.Delete(asset.HashPath);
			}
			finally
			{
				foreach (var asset in shaderAssets)
					asset.Dispose();
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			foreach (var asset in _shaderAssets)
				asset.Dispose();
		}

		/// <summary>
		///     Checks whether any of the C# effect assets have changed.
		/// </summary>
		/// <param name="shaderAssets">The shader assets that should be checked to determine the compilation action.</param>
		/// <param name="csharpAssets">The C# assets that should be checked to determine the compilation action.</param>
		private static CompilationAction DetermineAction(IEnumerable<ShaderAsset> shaderAssets,
														 IEnumerable<EffectAsset> csharpAssets)
		{
			foreach (var asset in csharpAssets)
			{
				if (!File.Exists(asset.HashPath))
					return CompilationAction.Process;

				var oldHash = Hash.FromFile(asset.HashPath);
				var newHash = Hash.Compute(asset.SourcePath);

				if (oldHash != newHash)
					return CompilationAction.Process;
			}

			if (shaderAssets.Any(asset => asset.GetRequiredAction() == CompilationAction.Process))
				return CompilationAction.Process;

			return CompilationAction.Skip;
		}

		/// <summary>
		///     Creates the assets for the shaders declared in the assets assembly.
		/// </summary>
		private static IEnumerable<ShaderAsset> CreateAssets()
		{
			return from type in Configuration.AssetListAssembly.GetTypes()
				   where type.BaseType == typeof(Effect)
				   from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				   let attribute = method.GetCustomAttributes(false).OfType<ShaderAttribute>().SingleOrDefault()
				   where attribute != null
				   select CreateShaderAsset(type.FullName, method.Name, attribute.ShaderType);
		}

		/// <summary>
		///     Initializes a shader asset instance.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		/// <param name="type">The type of the shader.</param>
		private static ShaderAsset CreateShaderAsset(string effect, string name, ShaderType type)
		{
			ShaderAsset asset = null;
			if (type == ShaderType.FragmentShader)
				asset = new FragmentShaderAsset(effect, name);
			else if (type == ShaderType.VertexShader)
				asset = new VertexShaderAsset(effect, name);
			else
				Log.Die("Unsupported shader type.");

			return asset;
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(ShaderAsset asset, BinaryWriter writer)
		{
			using (var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(asset.SourcePath))))
			{
				string profile;

				switch (asset.Type)
				{
					case ShaderType.VertexShader:
						WriteAssetHeader(writer, (byte)AssetType.VertexShader);

						// Skip over the shader signature
						var count = reader.ReadByte();
						for (var i = 0; i < 2 * count; ++i)
							reader.ReadByte();

						profile = "vs_4_0";
						break;
					case ShaderType.FragmentShader:
						WriteAssetHeader(writer, (byte)AssetType.FragmentShader);
						profile = "ps_4_0";
						break;
					default:
						throw new InvalidOperationException("Unsupported shader type.");
				}

				var length = reader.ReadInt32();
				var code = Encoding.UTF8.GetString(reader.ReadBytes(length));
				CompileGlslShader(writer, code);

				length = reader.ReadInt32();
				code = Encoding.UTF8.GetString(reader.ReadBytes(length));
				CompileHlslShader(asset, writer, code, profile);
			}
		}

		/// <summary>
		///     Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(ShaderAsset asset)
		{
			File.Delete(GetHlslPath(asset));
			File.Delete(GetCompiledHlslShaderPath(asset));
		}

		/// <summary>
		///     Writes the generated GLSL shader code to the buffer.
		/// </summary>
		/// <param name="writer">The buffer the compilation output should be appended to.</param>
		/// <param name="source">The GLSL shader source code.</param>
		private static void CompileGlslShader(BinaryWriter writer, string source)
		{
			var shader = "#version 330\n#extension GL_ARB_shading_language_420pack : enable\n" +
						 "#extension GL_ARB_separate_shader_objects : enable\n" + source;

			var code = Encoding.UTF8.GetBytes(shader);
			writer.WriteInt32(code.Length + 1);
			writer.Copy(code);
			writer.WriteByte(0);
		}

		/// <summary>
		///     Compiles the HLSL shader of the given profile and writes the generated code into the buffer.
		/// </summary>
		/// <param name="asset">The asset that contains the shader source code.</param>
		/// <param name="writer">The buffer the compilation output should be appended to.</param>
		/// <param name="source">The HLSL shader source code.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		private static void CompileHlslShader(Asset asset, BinaryWriter writer, string source, string profile)
		{
			if (!Configuration.CompileHlsl)
				return;

			var hlslFile = GetHlslPath(asset);
			File.WriteAllText(hlslFile, source);

			var byteCode = GetCompiledHlslShaderPath(asset);
			ExternalTool.Fxc(hlslFile, byteCode, profile);

			writer.Copy(File.ReadAllBytes(byteCode));
		}

		/// <summary>
		///     Gets the path of the temporary HLSL shader file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetHlslPath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".hlsl";
		}

		/// <summary>
		///     Gets the path of the temporary compile HLSL shader file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetCompiledHlslShaderPath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".fxo";
		}

		/// <summary>
		///     Creates the C# code for the vertex shader signatures.
		/// </summary>
		private void CreateShaderSignatureFile()
		{
			var signatures = new List<ShaderSignature>();
			foreach (var shader in _shaderAssets.Where(asset => asset.AssetType == (byte)AssetType.VertexShader))
			{
				using (var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(shader.SourcePath))))
				{
					var count = reader.ReadByte();
					var inputs = new ShaderInput[count];

					for (var i = 0; i < count; ++i)
					{
						inputs[i].Format = (VertexDataFormat)reader.ReadByte();
						inputs[i].Semantics = (DataSemantics)reader.ReadByte();
					}

					signatures.Add(new ShaderSignature(inputs));
				}
			}

			signatures = signatures.Distinct().ToList();

			for (var i = 0; i < signatures.Count; ++i)
			{
				if (!Configuration.CompileHlsl)
				{
					signatures[i] = new ShaderSignature(signatures[i].Inputs, new byte[0]);
					continue;
				}

				// Create a dummy HLSL vertex shader for the signature
				var inputs = String.Join(", ", signatures[i].Inputs.Select((input, index) =>
					String.Format("{0} p{1} : {2}", ToHlsl(input.Format), index, HlslCompiler.ToHlsl(input.Semantics))));
				var hlslCode = String.Format("float4 Main({0}) : SV_POSITION {{  return float4(0, 0, 0, 0); }}", inputs);

				// Compile the shader
				if (!Configuration.CompileHlsl)
					return;

				var hlslFile = Path.Combine(Configuration.TempDirectory, "sigshader.hlsl");
				var fxoFile = Path.Combine(Configuration.TempDirectory, "sigshader.fxo");

				File.WriteAllText(hlslFile, hlslCode);
				ExternalTool.Fxc(hlslFile, fxoFile, "vs_4_0", generateDebugInfo: false);

				// Retrieve the compiled byte code
				signatures[i] = new ShaderSignature(signatures[i].Inputs, File.ReadAllBytes(fxoFile));
			}

			// Generate the signature file
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("namespace {0}.Effects", Configuration.AssetsProject.RootNamespace);
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("using System;");
				writer.AppendLine("using System.Collections.Generic;");
				writer.AppendLine("using Pegasus.Utilities;");
				writer.AppendLine("using Pegasus.Platform.Graphics;");
				writer.NewLine();

				writer.AppendLine("/// <summary>");
				writer.AppendLine("///     Provides shader signatures.");
				writer.AppendLine("/// </summary>");
				writer.AppendLine("[UsedImplicitly]");
				writer.AppendLine("internal class ShaderSignatures : IShaderSignatureProvider");
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Gets the provided shader signatures.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("public IEnumerable<ShaderSignature> GetShaderSignatures()");
					writer.AppendBlockStatement(() =>
					{
						foreach (var signature in signatures)
						{
							var byteCode = String.Join(", ", signature.ByteCode);
							var inputs = String.Join(", ", signature.Inputs.Select(input =>
								String.Format("new ShaderInput(VertexDataFormat.{0}, DataSemantics.{1})", input.Format, input.Semantics)));

							writer.AppendLine("yield return new ShaderSignature(new [] {{ {0} }}, new byte[] {{ {1} }});", inputs, byteCode);
						}

						writer.AppendLine("yield break;");
					});
				});
			});

			File.WriteAllText(Configuration.CSharpShaderSignatureFile, writer.ToString());
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