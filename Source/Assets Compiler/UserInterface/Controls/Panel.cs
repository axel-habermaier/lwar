using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	/// <summary>
	///   Provides metadata for the 'Panel' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface.Controls")]
	[ContentProperty("Children")]
	internal abstract class Panel : UIElement
	{
		public UIElementCollection Children { get; private set; }
	}
}