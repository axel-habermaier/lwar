namespace Pegasus.AssetsCompiler.Xaml
{
	using System;

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
		/// <param name="clrNamespace">The name of the imported CLR namespace.</param>
		/// <param name="assemblyName">The assembly that defines the imported CLR namespace.</param>
		public XamlNamespace(string clrNamespace, string assemblyName)
		{
			Assert.ArgumentNotNullOrWhitespace(clrNamespace);

			Namespace = clrNamespace;
			AssemblyName = assemblyName;
		}

		/// <summary>
		///   Gets a value indicating whether this Xaml namespace should be ignored.
		/// </summary>
		public bool Ignored { get; private set; }

		/// <summary>
		///   The name of the imported CLR namespace.
		/// </summary>
		public string Namespace { get; private set; }

		/// <summary>
		///   The assembly that defines the imported CLR namespace.
		/// </summary>
		public string AssemblyName { get; private set; }
	}
}