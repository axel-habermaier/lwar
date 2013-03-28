using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
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
		/// <param name="assets">The assets that should be compiled.</param>
		public bool Compile(IEnumerable<Asset> assets)
		{
			var success = true;
			foreach (var asset in assets.OfType<TAsset>())
				success &= Compile(asset);
			return success;
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		private bool Compile(TAsset asset)
		{
			var action = asset.GetRequiredAction();
			action.Describe(asset);

			switch (action)
			{
				case CompilationAction.Skip:
					return true;
				case CompilationAction.Copy:
					File.Copy(asset.TempPath, asset.TargetPath, true);
					return true;
				case CompilationAction.Process:
					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);
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
	}
}