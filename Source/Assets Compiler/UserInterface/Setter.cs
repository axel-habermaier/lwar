using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	/// <summary>
	///   Provides metadata for the 'Setter' UI class.
	/// </summary>
	[ContentProperty("Setters")]
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal class Setter
	{
		public string Property { get; set; }
		public object Value { get; set; }
	}
}