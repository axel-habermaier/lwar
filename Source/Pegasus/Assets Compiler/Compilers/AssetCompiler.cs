﻿namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
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
		public virtual bool Compile(IEnumerable<Asset> assets)
		{
			var success = true;
			foreach (var asset in assets.OfType<TAsset>())
				success &= Compile(asset);
			return success;
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
		protected bool Compile(TAsset asset)
		{
			var action = asset.GetRequiredAction();

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
					bool success;

					using (var writer = new AssetWriter(asset))
						success = CompileAndLogExceptions(asset, writer.Writer);

					if (success)
						asset.WriteHash();

					return success;
				default:
					throw new InvalidOperationException("Unknown action type.");
			}
		}

		/// <summary>
		///     Compiles the asset, appending the compiled output to the given writer.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		internal void CompileSingle(TAsset asset, BufferWriter writer)
		{
			CompileAndLogExceptions(asset, writer);
		}

		/// <summary>
		///     Compiles the asset and logs the exception that might occur during the compilation.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="writer">The writer the compilation output should be appended to.</param>
		private bool CompileAndLogExceptions(TAsset asset, BufferWriter writer)
		{
			try
			{
				Compile(asset, writer);
				return true;
			}
			catch (PegasusException)
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