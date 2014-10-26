namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a namespace imported by a Xaml file.
	/// </summary>
	internal class XamlNamespace
	{
		/// <summary>
		///     Initializes a new instance of an ignored Xaml namespace.
		/// </summary>
		public XamlNamespace()
		{
			Ignored = true;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="clrNamespace">The name of the imported CLR namespace.</param>
		public XamlNamespace(string clrNamespace)
		{
			Assert.ArgumentNotNullOrWhitespace(clrNamespace);
			Namespace = clrNamespace;
		}

		/// <summary>
		///     Gets a value indicating whether this Xaml namespace should be ignored.
		/// </summary>
		public bool Ignored { get; private set; }

		/// <summary>
		///     The name of the imported CLR namespace.
		/// </summary>
		public string Namespace { get; private set; }
	}
}