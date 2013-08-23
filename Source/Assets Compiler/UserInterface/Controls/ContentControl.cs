using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	/// <summary>
	///   Provides metadata for the 'ContentControl' UI class.
	/// </summary>
	[ContentProperty("Content")]
	public class ContentControl : Control
	{
		public object Content { get; set; }
	}
}