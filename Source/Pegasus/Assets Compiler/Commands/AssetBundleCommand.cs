namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using Assets;
	using CommandLine;
	using Compilers;
	using CSharp;
	using Utilities;

	/// <summary>
	///     Compiles or cleans an asset bundle.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class AssetBundleCommand : ICommand
	{
		/// <summary>
		///     Gets the path of the source file.
		/// </summary>
		[Option("bundle", Required = true, HelpText = "The path to the bundle file.")]
		public string BundleFile { get; set; }

		/// <summary>
		///     Gets the path of a directory where the compiled asset bundle should be stored.
		/// </summary>
		[Option("target", Required = true, HelpText = "The target path where the compiled asset bundle should be stored.")]
		public string TargetPath { get; set; }

		/// <summary>
		///     Gets a value indicating which platform the bundle is compiled for.
		/// </summary>
		[Option("platform", Required = true, HelpText = "Indicates which platform the bundle is compiled for.")]
		public PlatformType Platform { get; set; }

		/// <summary>
		/// Gets the paths where dependent assemblies can be found.
		/// </summary>
		[Option("assemblies", Required = true, HelpText = "The paths where dependent assemblies can be found.")]
		public string AssemblyPath { get; set; }

		/// <summary>
		///     Gets a value indicating whether the texture is a cube map.
		/// </summary>
		[Option("debug", Required = false, HelpText = "Indicates whether assets should be compiled in debug mode.")]
		public bool Debug { get; set; }

		/// <summary>
		///     Gets or sets the action that should be performed on the asset bundle.
		/// </summary>
		public CompilationActions Actions { get; set; }

		/// <summary>
		///     Executes the command.
		/// </summary>
		public void Execute()
		{
			Configuration.Debug = Debug;
			Configuration.Platform = Platform;
			Configuration.BasePath = Path.GetDirectoryName(BundleFile);
			Configuration.TempDirectory = Path.Combine("obj", "AssetBundles", Path.GetFileNameWithoutExtension(BundleFile), Debug ? "Debug" : "Release");

			var document = XDocument.Load(BundleFile);
			var root = document.Root;
			root.Add(new XAttribute("File", BundleFile));

			var bundleAsset = new BundleAsset(root);
			var assets = root.Elements().Where(e => e.Name != "Assembly").ToArray();

			Configuration.EffectCodePath = Path.Combine(Configuration.BasePath, bundleAsset.EffectCodePath);
			Configuration.BundleCodePath = Path.Combine(Configuration.BasePath, bundleAsset.BundleCodePath);
			Configuration.RootNamespace = bundleAsset.Namespace;
			Configuration.Visibility = bundleAsset.Visibility;

			Directory.CreateDirectory(Configuration.TempDirectory);

			if (!String.IsNullOrWhiteSpace(Configuration.EffectCodePath))
				Directory.CreateDirectory(Configuration.EffectCodePath);

			if (!String.IsNullOrWhiteSpace(Configuration.BundleCodePath))
				Directory.CreateDirectory(Configuration.BundleCodePath);

			try
			{
				foreach (var assembly in root.Elements().Where(e => e.Name == "Assembly"))
					Assembly.LoadFrom(Path.Combine(AssemblyPath, assembly.Attribute("File").Value));
			}
			catch (Exception)
			{
				if (Actions.HasFlag(CompilationActions.Compile))
					throw;
			}

			var compilers = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
							 from type in assembly.GetTypes()
							 where type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IAssetCompiler))
							 select (IAssetCompiler)Activator.CreateInstance(type)).ToArray();

			if (Actions.HasFlag(CompilationActions.Clean))
			{
				foreach (var xmlAsset in assets)
					foreach (var compiler in compilers)
						compiler.Clean(xmlAsset);

				Log.Info("Cleaned asset bundle '{0}'.", Path.GetFileNameWithoutExtension(bundleAsset.OutputFileName));
			}

			if (Actions.HasFlag(CompilationActions.Compile))
			{
				var watch = new Stopwatch();
				watch.Start();

				var tasks = compilers.Select(compiler => compiler.Compile(assets)).ToArray();
				Task.WaitAll(tasks);

				var newAssets = tasks.Any(task => task.Result);
				if (newAssets || bundleAsset.RequiresCompilation)
				{
					File.WriteAllBytes(bundleAsset.TempPath, CreateBundle(bundleAsset, compilers));
					bundleAsset.WriteMetadata();
				}

				var outPath = Path.Combine(TargetPath, bundleAsset.OutputFileName);
				if (!File.Exists(outPath) || File.GetLastWriteTime(bundleAsset.TempPath) > File.GetLastWriteTime(outPath))
					File.Copy(bundleAsset.TempPath, outPath, overwrite: true);

				var elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;
				Log.Info("Compiled asset bundle '{0}' ({1:F2}s).", bundleAsset.Name, elapsedSeconds);
			}
		}

		/// <summary>
		///     Creates the asset bundle using the given compilers.
		/// </summary>
		/// <param name="bundleAsset">The bundle asset that should be created.</param>
		/// <param name="compilers">The compilers that should be used to create the bundle.</param>
		private byte[] CreateBundle(BundleAsset bundleAsset, IAssetCompiler[] compilers)
		{
			var assets = compilers.SelectMany(compiler => compiler.Assets).ToArray();
			var hash = bundleAsset.Hash;

			if (assets.Length == 0)
				Log.Die("Asset bundle '{0}' does not contain any assets.", bundleAsset.Name);

			byte[] rawData;
			byte[] compressedData;

			// Generate the raw bundle data
			using (var writer = new AssetWriter())
			{
				foreach (var asset in assets)
					writer.Copy(File.ReadAllBytes(asset.TempPath));

				var targetPath = Path.GetDirectoryName(TargetPath);
				if (!String.IsNullOrWhiteSpace(targetPath))
					Directory.CreateDirectory(targetPath);

				GenerateSource(bundleAsset, hash, assets);

				if (!String.IsNullOrWhiteSpace(TargetPath))
					Directory.CreateDirectory(TargetPath);

				rawData = writer.ToArray();
			}

			// Compress the bundle data
			using (var memory = new MemoryStream())
			{
				using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
					gzip.Write(rawData, 0, rawData.Length);

				compressedData = memory.ToArray();
			}

			// Generate the actual bundle data: the uncompressed header (the hash), followed by the compressed content
			using (var writer = new AssetWriter())
			{
				writer.Copy(hash);
				writer.WriteInt32(rawData.Length);
				writer.WriteByteArray(compressedData);

				return writer.ToArray();
			}
		}

		/// <summary>
		///     Generates the C# file for the asset bundle.
		/// </summary>
		/// <param name="bundleAsset">The bundle asset the class should be generated for.</param>
		/// <param name="hash">The asset bundle hash.</param>
		/// <param name="assets">The assets contained in the bundle.</param>
		private void GenerateSource(BundleAsset bundleAsset, Byte[] hash, Asset[] assets)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("namespace {0}", Configuration.RootNamespace);
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("using System;");
				writer.AppendLine("using System.Collections.Generic;");
				writer.AppendLine("using Pegasus.Rendering;");
				writer.AppendLine("using Pegasus.Utilities;");
				writer.AppendLine("using Pegasus.Platform.Graphics;");
				writer.AppendLine("using Pegasus.Platform.Memory;");
				writer.NewLine();

				writer.AppendLine("{1} class {0} : Pegasus.Rendering.AssetBundle", bundleAsset.ClassName, bundleAsset.Visibility);
				writer.AppendBlockStatement(() =>
				{
					foreach (var asset in assets)
						writer.AppendLine("private {0} {1};", asset.RuntimeType, GetFieldName(asset));

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Initializes a new instance.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("/// <param name=\"renderContext\">The render context the asset bundle belongs to.</param>");
					writer.AppendLine("public {0}(RenderContext renderContext)", bundleAsset.ClassName);
					writer.IncreaseIndent();
					writer.AppendLine(": base(renderContext, new Guid(new byte[] {{ {0} }}), \"{1}\", {2})",
						String.Join(", ", hash.Select(b => b.ToString())), bundleAsset.OutputFileName, assets.Length);
					writer.DecreaseIndent();
					writer.AppendBlockStatement(() => { });
					writer.NewLine();

					foreach (var asset in assets)
					{
						writer.AppendLine("public {0} {1}", asset.RuntimeType, asset.RuntimeName);
						writer.AppendBlockStatement(() =>
						{
							writer.AppendLine("get");
							writer.AppendBlockStatement(() =>
							{
								writer.AppendLine("Assert.That(LoadingCompleted, \"The asset bundle has not yet been fully loaded.\");");
								writer.AppendLine("return {0};", GetFieldName(asset));
							});
						});
						writer.NewLine();
					}

					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Gets the fonts contained in the asset bundle.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("protected override IEnumerable<Pegasus.UserInterface.Font> Fonts");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("get");
						writer.AppendBlockStatement(() =>
						{
							var fonts = assets.OfType<FontAsset>().ToArray();
							foreach (var asset in fonts)
								writer.AppendLine("yield return {0};", asset.RuntimeName);

							if (fonts.Length == 0)
								writer.AppendLine("yield break;");
						});
					});

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Creates the asset with the given number.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("/// <param name=\"graphicsDevice\">The graphics device that should be used to initialize the asset.</param>");
					writer.AppendLine("/// <param name=\"buffer\">The buffer the asset should be initialized from.</param>");
					writer.AppendLine("/// <param name=\"assetNumber\">The number of the asset that should be initialized.</param>");
					writer.AppendLine("protected override void CreateAsset(GraphicsDevice graphicsDevice, ref BufferReader buffer, ushort assetNumber)");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("switch (assetNumber)");
						writer.AppendBlockStatement(() =>
						{
							var index = 0;
							foreach (var asset in assets)
							{
								writer.AppendLine("case {0}:", index++);
								writer.IncreaseIndent();
								writer.AppendLine("CheckAssetType({0}, \"{1}\");", asset.AssetType, asset.RuntimeType);
								writer.AppendLine("{0} = {1}.Create(graphicsDevice, ref buffer);", GetFieldName(asset), asset.RuntimeType);
								writer.AppendLine("{0}.SetName(\"[{1}] {2}\");", GetFieldName(asset), bundleAsset.Name, asset.RuntimeName);
								writer.AppendLine("break;");
								writer.DecreaseIndent();
							}

							writer.AppendLine("default:");
							writer.IncreaseIndent();
							writer.AppendLine("throw new InvalidOperationException(\"Unsupported number of assets.\");");
							writer.DecreaseIndent();
						});
					});

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Reloads the asset with the given number.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("/// <param name=\"buffer\">The buffer the asset should be initialized from.</param>");
					writer.AppendLine("/// <param name=\"assetNumber\">The number of the asset that should be initialized.</param>");
					writer.AppendLine("protected override void ReloadAsset(ref BufferReader buffer, ushort assetNumber)");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("switch (assetNumber)");
						writer.AppendBlockStatement(() =>
						{
							var index = 0;
							foreach (var asset in assets)
							{
								writer.AppendLine("case {0}:", index++);
								writer.IncreaseIndent();
								writer.AppendLine("CheckAssetType({0}, \"{1}\");", asset.AssetType, asset.RuntimeType);
								writer.AppendLine("{0}.Load(ref buffer);", GetFieldName(asset));
								writer.AppendLine("break;");
								writer.DecreaseIndent();
							}

							writer.AppendLine("default:");
							writer.IncreaseIndent();
							writer.AppendLine("throw new InvalidOperationException(\"Unsupported number of assets.\");");
							writer.DecreaseIndent();
						});
					});

					writer.NewLine();
					writer.AppendLine("/// <summary>");
					writer.AppendLine("///     Disposes the object, releasing all managed and unmanaged resources.");
					writer.AppendLine("/// </summary>");
					writer.AppendLine("protected override void OnDisposing()");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("base.OnDisposing();");
						writer.NewLine();

						foreach (var asset in assets)
							writer.AppendLine("{0}.SafeDispose();", GetFieldName(asset));
					});
				});
			});

			File.WriteAllText(bundleAsset.CodeFilePath, writer.ToString());
		}

		/// <summary>
		///     Gets the name of the asset field.
		/// </summary>
		/// <param name="asset">The asset whose field name should be returned.</param>
		private static string GetFieldName(Asset asset)
		{
			return "_" + Char.ToLower(asset.RuntimeName[0]) + asset.RuntimeName.Substring(1);
		}
	}
}