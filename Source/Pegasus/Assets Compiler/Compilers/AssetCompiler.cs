namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Assets;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///     Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	/// <typeparam name="TAsset">The type of the asset that is compiled</typeparam>
	public abstract class AssetCompiler<TAsset> : IAssetCompiler
		where TAsset : Asset
	{
		/// <summary>
		///     Gets the additional assets created by the compiler.
		/// </summary>
		public virtual IEnumerable<Asset> AdditionalAssets
		{
			get { return Enumerable.Empty<Asset>(); }
		}

		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public virtual void Compile(IEnumerable<Asset> assets)
		{
			var tasks = assets.OfType<TAsset>().Select(Compile).ToArray();
			Task.WaitAll(tasks);
		}

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		public virtual void Clean(IEnumerable<Asset> assets)
		{
			foreach (var asset in assets.OfType<TAsset>())
			{
				File.Delete(asset.TempPath);
				File.Delete(asset.TargetPath);
				File.Delete(asset.HashPath);

				Clean(asset);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		///     Writes the asset file header into the given buffer.
		/// </summary>
		/// <param name="writer">The writer the asset file header should be written to.</param>
		/// <param name="assetType">The type of the asset that will subsequently be written into the buffer.</param>
		protected static void WriteAssetHeader(BufferWriter writer, byte assetType)
		{
			Assert.ArgumentNotNull(writer);

			writer.WriteByte((byte)'p');
			writer.WriteByte((byte)'g');
			writer.WriteUInt16(PlatformInfo.AssetFileVersion);
			writer.WriteByte(assetType);
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		protected Task Compile(TAsset asset)
		{
			var action = asset.GetRequiredAction();

			switch (action)
			{
				case CompilationAction.Skip:
					Log.Info("Skipping '{0}' (no changes detected).", asset.RelativePath);
					return Task.FromResult(true);
				case CompilationAction.Copy:
					Log.Info("Copying '{0}' to target directory (compilation skipped; no changes detected).", asset.RelativePath);
					File.Copy(asset.TempPath, asset.TargetPath, true);
					return Task.FromResult(true);
				case CompilationAction.Process:
					return Task.Factory.StartNew(() => CompileAsset(asset), TaskCreationOptions.LongRunning);
				default:
					throw new InvalidOperationException("Unknown action type.");
			}
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		private void CompileAsset(TAsset asset)
		{
			Log.Info("Compiling '{0}'...", asset.RelativePath);

			using (var writer = new AssetWriter(asset))
				CompileAndHandleExceptions(asset, writer.Writer);

			asset.WriteHash();
		}

		/// <summary>
		///     Compiles the asset, appending the compiled output to the given writer.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		internal void CompileSingle(TAsset asset, BufferWriter writer)
		{
			CompileAndHandleExceptions(asset, writer);
		}

		/// <summary>
		///     Compiles the asset and handles any exceptions that might occur during the compilation.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		private void CompileAndHandleExceptions(TAsset asset, BufferWriter writer)
		{
			try
			{
				Compile(asset, writer);
			}
			catch (Exception e)
			{
				Log.Error("Compiled of '{0}' failed: {1}", asset.RelativePath, e.Message);
				File.Delete(asset.HashPath);
				throw;
			}
		}

		/// <summary>
		///     Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		protected virtual void Compile(TAsset asset, BufferWriter writer)
		{
		}

		/// <summary>
		///     Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected virtual void Clean(TAsset asset)
		{
		}
	}
}