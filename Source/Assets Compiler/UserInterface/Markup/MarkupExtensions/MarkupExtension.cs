namespace Pegasus.AssetsCompiler.UserInterface.Markup.MarkupExtensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///   Abstract base class for Xaml markup extensions.
	/// </summary>
	internal abstract class MarkupExtension
	{
		/// <summary>
		///   Maps the name of a markup extension to the extension instance.
		/// </summary>
		private static Dictionary<string, MarkupExtension> _markupExtensions;

		/// <summary>
		///   Parses the content of the markup extension.
		/// </summary>
		/// <param name="content">The content of the markup extension.</param>
		protected abstract XamlObject Parse(string content);

		/// <summary>
		///   Parses the content of the markup extension.
		/// </summary>
		/// <param name="markupExtensionName">The name of the markup extension.</param>
		/// <param name="content">The content of the markup extension.</param>
		public XamlObject Parse(string markupExtensionName, string content)
		{
			Assert.ArgumentNotNullOrWhitespace(markupExtensionName);
			Assert.ArgumentNotNullOrWhitespace(content);

			if (_markupExtensions == null)
				_markupExtensions = Configuration.GetReflectionTypes()
												 .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MarkupExtension)))
												 .Select(Activator.CreateInstance)
												 .Cast<MarkupExtension>()
												 .ToDictionary(c => c.GetType().FullName, c => c);

			if (!markupExtensionName.EndsWith("Extension"))
				markupExtensionName += "Extension";

			MarkupExtension markupExtension;
			if (!_markupExtensions.TryGetValue(markupExtensionName, out markupExtension))
				Log.Die("Unknown markup extension '{0}'.", markupExtensionName);

			return markupExtension.Parse(content);
		}
	}
}