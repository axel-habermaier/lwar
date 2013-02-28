using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using Assets;
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	/// <typeparam name="TAsset">The type of the asset that is compiled</typeparam>
	internal abstract class AssetCompiler<TAsset> : IAssetCompiler
		where TAsset : Asset
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The asset that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			var success = true;
			foreach (var asset in assets.OfType<TAsset>())
				success &= Compile(asset);
			return success;
		}

		/// <summary>
		///   Gets a value indicating which action the compiler has to take.
		/// </summary>
		/// <param name="asset">The asset for which the required action should be determined.</param>
		private static CompilationAction GetRequiredAction(TAsset asset)
		{
			if (!File.Exists(asset.TempPath))
				return CompilationAction.Process;

			if (!File.Exists(asset.HashPath))
				return CompilationAction.Process;

			var oldHash = File.ReadAllBytes(asset.HashPath);
			var newHash = ComputeHash(asset);

			for (var i = 0; i < oldHash.Length; ++i)
			{
				if (oldHash[i] != newHash[i])
					return CompilationAction.Process;
			}

			if (!File.Exists(asset.TargetPath))
				return CompilationAction.Copy;

			return CompilationAction.Skip;
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		private bool Compile(TAsset asset)
		{
			EnsurePathsExist(Path.GetDirectoryName(asset.TargetPath));
			EnsurePathsExist(Path.GetDirectoryName(asset.TempPathWithoutExtension));

			var action = GetRequiredAction(asset);

			switch (action)
			{
				case CompilationAction.Skip:
					Log.Info("Skipping '{0}' (no changes detected).", asset.RelativePath);
					return true;
				case CompilationAction.Copy:
					Log.Info("Copying '{0}' to target directory (compilation skipped; no changes detected).", asset.RelativePath);
					File.Copy(asset.TempPath, asset.TargetPath, true);
					return true;
				case CompilationAction.Process:
					Log.Info("Compiling '{0}'...", asset.RelativePath);

					File.WriteAllBytes(asset.HashPath, ComputeHash(asset));
					using (var writer = new AssetWriter(asset.TempPath, asset.TargetPath))
						return CompileAndLogExceptions(asset, writer.Writer);
				default:
					throw new InvalidOperationException("Unknown action type.");
			}
		}

		/// <summary>
		///   Compiles the asset, appending the compiled output to the given buffer.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		internal void Compile(TAsset asset, BufferWriter buffer)
		{
			EnsurePathsExist(Path.GetDirectoryName(asset.TargetPath));
			EnsurePathsExist(Path.GetDirectoryName(asset.TempPathWithoutExtension));

			CompileAndLogExceptions(asset, buffer);
		}

		/// <summary>
		///   Compiles the asset and logs the exception that might occur during the compilation.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private bool CompileAndLogExceptions(TAsset asset, BufferWriter buffer)
		{
			try
			{
				CompileCore(asset, buffer);
				return true;
			}
			catch (ApplicationAbortedException)
			{
			}
			catch (Exception e)
			{
				Log.Error("{0}", e.Message);
			}

			File.Delete(asset.HashPath);
			return false;
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected abstract void CompileCore(TAsset asset, BufferWriter buffer);

		/// <summary>
		///   Computes the hash of the current source file.
		/// </summary>
		/// <param name="asset">The asset for which the hash should be computed.</param>
		private static byte[] ComputeHash(Asset asset)
		{
			using (var cryptoProvider = new MD5CryptoServiceProvider())
			using (var file = new FileStream(asset.SourcePath, FileMode.Open, FileAccess.Read))
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