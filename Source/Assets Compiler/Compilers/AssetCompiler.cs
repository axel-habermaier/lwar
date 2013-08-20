using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform.Logging;
	using Framework.Platform.Memory;

	/// <summary>
	///   Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	/// <typeparam name="TAsset">The type of the asset that is compiled</typeparam>
	public abstract class AssetCompiler<TAsset> : DisposableObject, IAssetCompiler
		where TAsset : Asset
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type.
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
		///   Removes the compiled assets and all temporary files written by the compiler.
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
		///   Compiles the asset.
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
		///   Compiles the asset, appending the compiled output to the given buffer.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		internal void CompileSingle(TAsset asset, BufferWriter buffer)
		{
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
				Compile(asset, buffer);
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
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected virtual void Compile(TAsset asset, BufferWriter buffer)
		{
		}

		/// <summary>
		///   Removes the compiled asset and all temporary files written by the compiler.
		/// </summary>
		/// <param name="asset">The asset that should be cleaned.</param>
		protected virtual void Clean(TAsset asset)
		{
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}