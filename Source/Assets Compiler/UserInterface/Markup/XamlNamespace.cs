using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	/// <summary>
	///   Represents a namespace imported by a Xaml file.
	/// </summary>
	internal class XamlNamespace
	{
		/// <summary>
		///   Initializes a new instance of an ignored Xaml namespace.
		/// </summary>
		public XamlNamespace()
		{
			Ignored = true;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="runtimeNamespace">The name of the imported CLR namespace at runtime.</param>
		/// <param name="assemblyName">The assembly that defines the imported CLR namespace.</param>
		public XamlNamespace(string runtimeNamespace, string assemblyName)
		{
			Assert.ArgumentNotNullOrWhitespace(runtimeNamespace);
			Assert.ArgumentNotNullOrWhitespace(assemblyName);

			RuntimeNamespace = runtimeNamespace;
			CompilationNamespace = runtimeNamespace;
			AssemblyName = assemblyName;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="runtimeNamespace">The name of the imported CLR namespace at runtime.</param>
		/// <param name="compilationNamespace">The name of the imported CLR namespace at asset compilation time.</param>
		/// <param name="assemblyName">The assembly that defines the imported CLR namespace.</param>
		public XamlNamespace(string runtimeNamespace, string compilationNamespace, string assemblyName)
			: this(runtimeNamespace, assemblyName)
		{
			Assert.ArgumentNotNullOrWhitespace(compilationNamespace);
			CompilationNamespace = compilationNamespace;
		}

		/// <summary>
		///   Gets a value indicating whether this Xaml namespace should be ignored.
		/// </summary>
		public bool Ignored { get; private set; }

		/// <summary>
		///   The name of the imported CLR namespace at runtime.
		/// </summary>
		public string RuntimeNamespace { get; private set; }

		/// <summary>
		///   The name of the imported CLR namespace at asset compilation time.
		/// </summary>
		public string CompilationNamespace { get; private set; }

		/// <summary>
		///   The assembly that defines the imported CLR namespace.
		/// </summary>
		public string AssemblyName { get; private set; }
	}
}