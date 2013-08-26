using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	/// <summary>
	///   When applied to a property of type XamlDerviedValue, specifies the order of evaluation of the property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class DeferredEvaluationOrderAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="evaluationOrder">Determines the order of evaluation of the deferred value.</param>
		public DeferredEvaluationOrderAttribute(int evaluationOrder)
		{
			EvaluationOrder = evaluationOrder;
		}

		/// <summary>
		///   Determines the order of evaluation of the deferred value.
		/// </summary>
		public int EvaluationOrder { get; private set; }
	}
}