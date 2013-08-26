using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	using Markup;

	/// <summary>
	///   Provides metadata for the 'Setter' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal class Setter
	{
		[DeferredEvaluationOrder(1)]
		public XamlDeferredValue Property { get; set; }

		[DeferredEvaluationOrder(2)]
		public XamlDeferredValue Value { get; set; }
	}
}