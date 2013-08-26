using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	/// <summary>
	///   Provides metadata for the 'Style' UI class.
	/// </summary>
	[ContentProperty("Setters")]
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal class Style
	{
		[IgnoreAtRuntime]
		public Type TargetType { get; set; }

		public SetterCollection Setters { get; set; }
	}
}