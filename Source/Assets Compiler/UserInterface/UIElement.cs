using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	using System.Drawing;

	/// <summary>
	///   Provides metadata for the 'UIElement' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal abstract class UIElement
	{
		[IgnoreAtRuntime]
		public string Name { get; set; }

		public ResourceDictionary Resources { get; set; }

		public double Width { get; set; }
		public double Height { get; set; }
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }
		public double MaxWidth { get; set; }
		public double MaxHeight { get; set; }

		public Color Background { get; set; }
		public Color Foreground { get; set; }
	}
}