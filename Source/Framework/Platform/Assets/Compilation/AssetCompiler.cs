using System;

namespace Pegasus.Framework.Platform.Assets.Compilation
{
	using System.IO;
	using System.Security.Cryptography;

	/// <summary>
	///   Represents a compiler that compiles a source asset into a binary format that the runtime can load more efficiently.
	/// </summary>
	public abstract class AssetCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		protected AssetCompiler(string asset)
		{
			Asset = new SourceAsset(asset);
		}

		/// <summary>
		///   Gets a description of the asset type that the compiler supports.
		/// </summary>
		internal abstract string AssetType { get; }

		/// <summary>
		///   Gets the asset that the compiler compiles.
		/// </summary>
		internal SourceAsset Asset { get; private set; }

		/// <summary>
		///   Gets the buffer the processed output should be written to.
		/// </summary>
		protected BufferWriter Buffer { get; private set; }

		/// <summary>
		///   Gets a value indicating which action the compiler has to take.
		/// </summary>
		private CompilationAction GetRequiredAction()
		{
			if (!File.Exists(Asset.TempPath))
				return CompilationAction.Process;

			if (!File.Exists(Asset.HashPath))
				return CompilationAction.Process;

			var oldHash = File.ReadAllBytes(Asset.HashPath);
			var newHash = ComputeHash();

			for (var i = 0; i < oldHash.Length; ++i)
			{
				if (oldHash[i] != newHash[i])
					return CompilationAction.Process;
			}

			if (!File.Exists(Asset.TargetPath))
				return CompilationAction.Copy;

			return CompilationAction.Skip;
		}

		/// <summary>
		///   Compiles the asset and returns true to indicate that the compiled asset at the target location has changed.
		/// </summary>
		internal bool Compile()
		{
			EnsurePathsExist(Path.GetDirectoryName(Asset.TargetPath));
			EnsurePathsExist(Path.GetDirectoryName(Asset.TempPathWithoutExtension));

			var action = GetRequiredAction();

			switch (action)
			{
				case CompilationAction.Skip:
					Log.Info("   Skipping '{0}' (no changes detected).", Asset.RelativePath);
					return false;
				case CompilationAction.Copy:
					Log.Info("   Copying '{0}' to target directory (compilation skipped; no changes detected).", Asset.RelativePath);
					File.Copy(Asset.TempPath, Asset.TargetPath, true);
					return true;
				case CompilationAction.Process:
					Log.Info("   Compiling '{0}'...", Asset.RelativePath);

					File.WriteAllBytes(Asset.HashPath, ComputeHash());
					using (var writer = new AssetWriter(Asset.TempPath, Asset.TargetPath))
					{
						Buffer = writer.Writer;
						CompileCore();
					}
					return true;
				default:
					throw new InvalidOperationException("Unknown action type.");
			}
		}

		/// <summary>
		///   Compiles the asset, appending the compiled output to the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		internal void Compile(BufferWriter buffer)
		{
			EnsurePathsExist(Path.GetDirectoryName(Asset.TargetPath));
			EnsurePathsExist(Path.GetDirectoryName(Asset.TempPathWithoutExtension));

			Asset = Asset;
			Buffer = buffer;
			CompileCore();
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected abstract void CompileCore();

		/// <summary>
		///   Computes the hash of the current source file.
		/// </summary>
		private byte[] ComputeHash()
		{
			using (var cryptoProvider = new MD5CryptoServiceProvider())
			using (var file = new FileStream(Asset.SourcePath, FileMode.Open, FileAccess.Read))
				return cryptoProvider.ComputeHash(file);
		}

		/// <summary>
		///   Ensures that the given paths exist.
		/// </summary>
		/// <param name="path">The path that should exist.</param>
		private static void EnsurePathsExist(string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path, () => path);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		///   Removes the hash file of the asset, as well as its compiled outputs in the temp and target directories.
		/// </summary>
		internal void Clean()
		{
			File.Delete(Asset.TempPath);
			File.Delete(Asset.TargetPath);
			File.Delete(Asset.HashPath);
		}

		/// <summary>
		///   Indicates the action that the Compiler must take.
		/// </summary>
		private enum CompilationAction
		{
			/// <summary>
			///   Indicates that the Compiler can skip the asset as the latest version of the processed asset is already at the target
			///   location.
			/// </summary>
			Skip,

			/// <summary>
			///   Indicates that the Compiler does not have to process the asset, but must copy the latest version of the processed
			///   asset to the target location.
			/// </summary>
			Copy,

			/// <summary>
			///   Indicates that the Compiler has to process the asset.
			/// </summary>
			Process,
		}
	}
}