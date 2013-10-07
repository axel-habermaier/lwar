namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Xml.Linq;

	/// <summary>
	///   Normalizes the contents of a XAML object.
	/// </summary>
	internal interface INormalizationRequired
	{
		/// <summary>
		///   Normalizes the contents of the given XAML element.
		/// </summary>
		/// <param name="xamlElement">The element that should be normalized.</param>
		void Normalize(XElement xamlElement);
	}
}