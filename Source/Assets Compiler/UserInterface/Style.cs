using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides metadata for the 'Style' UI class.
	/// </summary>
	[ContentProperty("Setters")]
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal class Style
	{
		public List<Setter> Setters { get; set; }
	}
}