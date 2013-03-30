using System;

namespace Pegasus.AssetsCompiler.Effects
{
	/// <summary>
	///   Indicates that a method invocation should be mapped to an intrinsic shader function.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal class MapsToAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="intrinsic">The intrinsic shader function an invocation of the method should be mapped to.</param>
		public MapsToAttribute(Intrinsic intrinsic)
		{
			Intrinsic = intrinsic;
		}

		/// <summary>
		///   Gets the intrinsic shader function an invocation of the method should be mapped to.
		/// </summary>
		public Intrinsic Intrinsic { get; private set; }
	}
}