using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	/// <summary>
	///   Provides metadata for the 'ContentControl' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface.Controls")]
	[ContentProperty("Content")]
	internal class ContentControl : Control
	{
		public object Content { get; set; }
	}
}