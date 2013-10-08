using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	/// <summary>
	///   Provides metadata for the 'ContentPresenter' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface.Controls")]
	[ContentProperty("Content")]
	internal class ContentPresenter : Control
	{
		public object Content { get; set; }
	}
}