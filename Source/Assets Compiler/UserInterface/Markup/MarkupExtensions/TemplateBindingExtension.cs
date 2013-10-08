namespace Pegasus.AssetsCompiler.UserInterface.Markup.MarkupExtensions
{
	using System;

	/// <summary>
	///   Represents markup extension to setup template bindings in Xaml.
	/// </summary>
	internal class TemplateBindingExtension : MarkupExtension
	{
		/// <summary>
		///   Parses the content of the markup extension.
		/// </summary>
		/// <param name="content">The content of the markup extension.</param>
		protected override XamlObject Parse(string content)
		{
			return null;
		}
	}
}