using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using Assets;
	using CodeGeneration.Effects;
	using Effects;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using Framework.Platform.Memory;
	using Effect = Effects.Effect;

	/// <summary>
	///   Compiles effects written in C#.
	/// </summary>
	[UsedImplicitly]
	internal class EffectCompiler : AssetCompiler<ShaderAsset>
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type. Returns true to indicate that the compilation of all assets
		///   has been successful.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public override bool Compile(IEnumerable<Asset> assets)
		{
			var shaderAssets = CreateAssets().ToArray();

			try
			{
				var csharpAssets = assets.OfType<CSharpAsset>().ToArray();

				if (DetermineAction(shaderAssets, csharpAssets) == CompilationAction.Skip)
					Log.Info("Skipping effect compilation (no changes detected).");
				else
				{
					Log.Info("Compiling effects...");

					foreach (var asset in csharpAssets)
						Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);

					using (var project = new EffectsProject { CSharpFiles = csharpAssets })
					{
						if (!project.Compile())
							return false;
					}
				}

				var success = true;
				foreach (var asset in shaderAssets)
					success &= Compile(asset);

				return success;
			}
			catch (Exception e)
			{
				Log.Error("Effect cross-compilation failed: {0}", e.Message);
				return false;
			}
			finally
			{
				shaderAssets.SafeDisposeAll();
			}
		}

		/// <summary>
		///   Removes the compiled assets and all temporary files written by the compiler.
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

				foreach (var asset in assets.OfType<CSharpAsset>())
					File.Delete(asset.HashPath);
			}
			finally
			{
				shaderAssets.SafeDisposeAll();
			}
		}

		/// <summary>
		///   Checks whether any of the C# effect assets have changed.
		/// </summary>
		/// <param name="shaderAssets">The shader assets that should be checked to determine the compilation action.</param>
		/// <param name="csharpAssets">The C# assets that should be checked to determine the compilation action.</param>
		private static CompilationAction DetermineAction(IEnumerable<ShaderAsset> shaderAssets,
														 IEnumerable<CSharpAsset> csharpAssets)
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
		///   Creates the assets for the shaders declared in the assets assembly.
		/// </summary>
		private static IEnumerable<ShaderAsset> CreateAssets()
		{
			return from type in Configuration.AssetListAssembly.GetTypes()
				   where type.BaseType == typeof(Effect)
				   from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				   let attribute = method.GetCustomAttributes(false).OfType<ShaderAttribute>().SingleOrDefault()
				   where attribute != null
				   select new ShaderAsset(type.FullName, method.Name, attribute.ShaderType);
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(ShaderAsset asset, BufferWriter buffer)
		{
			using (var reader = BufferReader.Create(File.ReadAllBytes(asset.SourcePath)))
			{
				string profile;

				switch (asset.Type)
				{
					case ShaderType.VertexShader:
						var count = reader.ReadByte();
						buffer.WriteByte(count);

						for (var i = 0; i < count * 2; ++i)
							buffer.WriteByte(reader.ReadByte());

						profile = "vs_4_0";
						break;
					case ShaderType.FragmentShader:
						profile = "ps_4_0";
						break;
					default:
						throw new InvalidOperationException("Unsupported shader type.");
				}

				CompileGlslShader(buffer, reader.ReadString());
				CompileHlslShader(asset, buffer, reader.ReadString(), profile);

				Assert.That(reader.EndOfBuffer, "Not all shader code has been read.");
			}
		}

		/// <summary>
		///   Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected override void Clean(ShaderAsset asset)
		{
			File.Delete(GetHlslPath(asset));
			File.Delete(GetCompiledHlslShaderPath(asset));
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
		private static void CompileHlslShader(Asset asset, BufferWriter buffer, string source, string profile)
		{
			if (!Configuration.CompileHlsl)
				return;

			var hlslFile = GetHlslPath(asset);
			File.WriteAllText(hlslFile, source);

			var byteCode = GetCompiledHlslShaderPath(asset);
			ExternalTool.Fxc(hlslFile, byteCode, profile);

			buffer.Copy(File.ReadAllBytes(byteCode));
		}

		/// <summary>
		///   Gets the path of the temporary HLSL shader file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetHlslPath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".hlsl";
		}

		/// <summary>
		///   Gets the path of the temporary compile HLSL shader file.
		/// </summary>
		/// <param name="asset">The asset the path should be returned for.</param>
		private static string GetCompiledHlslShaderPath(Asset asset)
		{
			return asset.TempPathWithoutExtension + ".fxo";
		}
	}
}