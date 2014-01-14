namespace Pegasus.AssetsCompiler.CSharp
{
	using System;
	using System.IO;

	/// <summary>
	///     Represents a C# file of a C# project.
	/// </summary>
	internal struct CSharpFile
	{
		/// <summary>
		///     The absolute path to the source directory of all the C# files.
		/// </summary>
		private readonly string _sourceDirectory;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sourceDirectory">The absolute path to the source directory of all the C# files.</param>
		/// <param name="relativePath">The path to the C# file relative to the source directory, i.e., CSharp/File.cs.</param>
		public CSharpFile(string sourceDirectory, string relativePath)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(relativePath);

			_sourceDirectory = sourceDirectory;
			RelativePath = relativePath;
		}

		/// <summary>
		///     Gets the path to the C# file relative to the source directory, i.e., CSharp/File.cs.
		/// </summary>
		public string RelativePath { get; private set; }

		/// <summary>
		///     Gets the absolute path to the C# file in the source directory, i.e. C:/AssetsSources/CSharp/File.cs.
		/// </summary>
		public string SourcePath
		{
			get { return Path.Combine(_sourceDirectory, RelativePath); }
		}
	}
}