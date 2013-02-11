using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Security.Cryptography;
	using System.Threading;
	using System.Threading.Tasks;
	using Scripting;

	/// <summary>
	///   Automatically recompiles and reloads assets that have been changed while the application is running.
	/// </summary>
	internal class AssetsReloader : DisposableObject
	{
		/// <summary>
		///   The path of the asset directory containing the uncompiled source assets.
		/// </summary>
		private const string AssetPath = "../../Assets";

		/// <summary>
		///   The frequency of the file modification checks in Hz.
		/// </summary>
		private const int UpdateFrequency = 2;

		/// <summary>
		///   The cancellation token source that can be used to cancel the background process.
		/// </summary>
		private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

		/// <summary>
		///   Stores the file modification state of each monitored file.
		/// </summary>
		private readonly Dictionary<string, FileModificationInfo> _fileInfos = new Dictionary<string, FileModificationInfo>();

		/// <summary>
		///   A queue of assets that have recently been modified.
		/// </summary>
		private readonly ConcurrentQueue<string> _modifiedAssets = new ConcurrentQueue<string>();

		/// <summary>
		///   The assets compiler that is used to compile assets.
		/// </summary>
		private IAssetsCompiler _assetsCompiler;

		/// <summary>
		///   The task that represents the background operation.
		/// </summary>
		private Task _task;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		[Conditional("DEBUG")]
		internal void Initialize()
		{
			// Dynamically load the assets compiler assembly instead of adding a reference to it. That way, release builds
			// do not depend on the assembly at all.
			_assetsCompiler = Assembly.LoadFrom("Assets Compiler.exe")
									  .GetTypes()
									  .Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(IAssetsCompiler)))
									  .Select(Activator.CreateInstance)
									  .OfType<IAssetsCompiler>()
									  .Single();

#if Windows
			// Compile all assets on startup
			_assetsCompiler.Compile();

			// Periodically check the file system for modified files
			var token = _cancellation.Token;
			_task = Task.Factory.StartNew(() =>
				{
					while (!token.IsCancellationRequested)
					{
						UpdateFileInfos(new DirectoryInfo(AssetPath), "");
						Thread.Sleep(1000 / UpdateFrequency);
					}
				}, token);
#endif
		}

		/// <summary>
		///   Recursively updates the file modification information for all files in the directory and its sub-directories.
		/// </summary>
		/// <param name="directory">The current root directory.</param>
		/// <param name="path">The current path relative to the assets directory.</param>
		private void UpdateFileInfos(DirectoryInfo directory, string path)
		{
			Assert.ArgumentNotNull(directory, () => directory);
			Assert.ArgumentNotNull(path, () => path);

			foreach (var file in directory.GetFiles("*.*"))
				UpdateFileInfo(file, path);

			foreach (var subDirectory in directory.GetDirectories())
				UpdateFileInfos(subDirectory, Path.Combine(path, subDirectory.Name));
		}

		/// <summary>
		///   Updates the file modification information for the given file.
		/// </summary>
		/// <param name="file">The file for which the modification information should be updated.</param>
		/// <param name="path">The path of the file relative to the assets directory.</param>
		private void UpdateFileInfo(FileInfo file, string path)
		{
			Assert.ArgumentNotNull(file, () => file);

			var hash = ComputeFileHash(file);

			FileModificationInfo info;
			if (!_fileInfos.TryGetValue(file.FullName, out info))
				_fileInfos.Add(file.FullName, new FileModificationInfo(Path.Combine(path, file.Name), hash));
			else
			{
				if (info.LoadedHash == hash)
					return;

				if (info.ModifiedHash == hash)
				{
					_modifiedAssets.Enqueue(info.FileName);
					info.ModifiedHash = null;
					info.LoadedHash = hash;
				}
				else
					info.ModifiedHash = hash;
			}
		}

		/// <summary>
		///   Computes the hash of the given file.
		/// </summary>
		/// <param name="file">The file for which a hash should be computed.</param>
		private static string ComputeFileHash(FileInfo file)
		{
			Assert.ArgumentNotNull(file, () => file);

			using (var cryptoProvider = new SHA1CryptoServiceProvider())
			{
				var buffer = File.ReadAllBytes(file.FullName);
				return BitConverter.ToString(cryptoProvider.ComputeHash(buffer));
			}
		}

		/// <summary>
		///   Reloads all modified assets.
		/// </summary>
		[Conditional("DEBUG")]
		internal void ReloadModifiedAssets()
		{
			string asset;
			while (_modifiedAssets.TryDequeue(out asset))
			{
				try
				{
					var compiledAsset = _assetsCompiler.Compile(asset);
					Commands.ReloadAsset.Invoke(compiledAsset);
				}
				catch (Exception e)
				{
					Log.Error("Failed to compile and reload asset '{0}': {1}", asset, e.Message);
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_cancellation.Cancel();

			if (_task != null)
				_task.Wait();

			_cancellation.SafeDispose();
			_task.SafeDispose();
		}

		/// <summary>
		///   Contains the information about a file's modification state.
		/// </summary>
		private class FileModificationInfo
		{
			/// <summary>
			///   The full path of the file.
			/// </summary>
			public readonly string FileName;

			/// <summary>
			///   The hash of the currently loaded file.
			/// </summary>
			public string LoadedHash;

			/// <summary>
			///   The hash of the modified file. Just to be sure, we wait for more than one update cycle before reporting the file as
			///   modified (because most tools write files in batches, causing several updates and serveral different hashes).
			/// </summary>
			public string ModifiedHash;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="fileName">The full path of the file.</param>
			/// <param name="loadedHash">The hash of the currently loaded file.</param>
			public FileModificationInfo(string fileName, string loadedHash)
			{
				FileName = fileName;
				LoadedHash = loadedHash;
			}
		}
	}
}